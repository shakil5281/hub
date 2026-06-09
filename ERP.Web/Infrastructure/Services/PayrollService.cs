using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class PayrollService : IPayrollService
{
    private const decimal MedicalAllowanceFixed = 750m;
    private const decimal FoodAllowanceFixed = 1250m;
    private const decimal ConveyanceAllowanceFixed = 450m;
    private const decimal FixedAllowanceTotal = 2450m;
    private const decimal OtHoursDivisor = 208m;

    private readonly IPayrollRepository _payrollRepository;
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public PayrollService(
        IPayrollRepository payrollRepository,
        AppDbContext context,
        ICurrentUserService currentUserService)
    {
        _payrollRepository = payrollRepository;
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<PayrollPeriodVm>> GetPeriodsAsync()
    {
        var periods = await _payrollRepository.GetPeriodsAsync(_currentUserService.CompanyId);
        return periods.Select(p => new PayrollPeriodVm(
            p.Id, p.Name, p.StartDate, p.EndDate, p.PayrollStatus.ToString())).ToList();
    }

    public async Task<(bool Success, string? Error, int? SheetId)> GeneratePayrollAsync(int periodId)
    {
        var companyId = _currentUserService.CompanyId;
        var period = await _payrollRepository.GetPeriodByIdAsync(periodId, companyId);
        if (period == null) return (false, "Payroll period not found.", null);

        var existingSheet = await _context.PayrollSheets
            .FirstOrDefaultAsync(s => s.PayrollPeriodId == periodId && s.CompanyId == companyId);
        if (existingSheet != null && existingSheet.PayrollStatus == PayrollStatus.Locked)
            return (false, "Payroll sheet is locked.", null);

        if (existingSheet != null)
        {
            var oldDetails = await _context.PayrollDetails
                .Where(d => d.PayrollSheetId == existingSheet.Id)
                .ToListAsync();
            foreach (var detail in oldDetails)
                detail.IsDeleted = true;
        }
        else
        {
            existingSheet = new PayrollSheet
            {
                CompanyId = companyId,
                PayrollPeriodId = periodId,
                PayrollStatus = PayrollStatus.Generated,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            };
            await _payrollRepository.AddSheetAsync(existingSheet);
            await _payrollRepository.SaveChangesAsync();
        }

        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();

        var attendances = await _context.DailyAttendances
            .Where(a => a.CompanyId == companyId
                && a.AttendanceDate >= period.StartDate.Date
                && a.AttendanceDate <= period.EndDate.Date)
            .ToListAsync();

        var attendanceByEmployee = attendances.GroupBy(a => a.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());

        var billRates = await _context.BillRateConfigs
            .Where(b => b.CompanyId == companyId)
            .ToListAsync();

        var advances = await _context.AdvanceSalaries
            .Where(a => a.CompanyId == companyId
                && a.AdvanceStatus == AdvanceSalaryStatus.Approved
                && a.RemainingBalance > 0)
            .ToListAsync();

        var totalWorkingDays = CountWorkingDays(period.StartDate, period.EndDate);
        decimal totalNet = 0;

        foreach (var employee in employees)
        {
            attendanceByEmployee.TryGetValue(employee.Id, out var empAttendance);
            empAttendance ??= new List<DailyAttendance>();

            var payableDays = empAttendance.Count(a => !a.IsAbsent && a.InTime.HasValue);
            var absentDays = empAttendance.Count(a => a.IsAbsent);
            var otMinutes = empAttendance.Sum(a => a.OvertimeMinutes);
            var nightDays = empAttendance.Count(a => a.IsNightShift && !a.IsAbsent);
            var holidayDays = empAttendance.Count(a => a.IsHoliday && !a.IsAbsent);

            var breakdown = CalculateSalaryBreakdown(employee.GrossSalary, totalWorkingDays, absentDays);

            var nightRate = billRates.FirstOrDefault(b => b.BillType == BillType.Night)?.Amount ?? 0m;
            var holidayRate = billRates.FirstOrDefault(b => b.BillType == BillType.Holiday)?.Amount ?? 0m;

            breakdown.OvertimeAmount = Math.Round(breakdown.OtRate * (otMinutes / 60m), 2);
            breakdown.NightBillAmount = nightDays * nightRate;
            breakdown.HolidayBillAmount = holidayDays * holidayRate;

            var advance = advances.FirstOrDefault(a => a.EmployeeId == employee.Id);
            breakdown.AdvanceDeduction = advance != null
                ? Math.Min(advance.MonthlyDeduction, advance.RemainingBalance)
                : 0m;

            breakdown.NetPayableSalary = breakdown.GrossSalary
                - breakdown.AbsentDeduction
                + breakdown.OvertimeAmount
                + breakdown.NightBillAmount
                + breakdown.HolidayBillAmount
                - breakdown.AdvanceDeduction;

            if (advance != null && breakdown.AdvanceDeduction > 0)
            {
                advance.RemainingBalance -= breakdown.AdvanceDeduction;
                if (advance.RemainingBalance <= 0)
                    advance.AdvanceStatus = AdvanceSalaryStatus.Completed;
                advance.UpdatedAt = DateTime.UtcNow;
                advance.UpdatedBy = _currentUserService.UserName;
            }

            await _payrollRepository.AddDetailAsync(new PayrollDetail
            {
                CompanyId = companyId,
                PayrollSheetId = existingSheet.Id,
                EmployeeId = employee.Id,
                GrossSalary = breakdown.GrossSalary,
                BasicSalary = breakdown.BasicSalary,
                HouseRent = breakdown.HouseRent,
                MedicalAllowance = breakdown.MedicalAllowance,
                FoodAllowance = breakdown.FoodAllowance,
                ConveyanceAllowance = breakdown.ConveyanceAllowance,
                AttendancePayableDays = payableDays,
                AbsentDeduction = breakdown.AbsentDeduction,
                OvertimeAmount = breakdown.OvertimeAmount,
                NightBillAmount = breakdown.NightBillAmount,
                HolidayBillAmount = breakdown.HolidayBillAmount,
                AdvanceDeduction = breakdown.AdvanceDeduction,
                NetPayableSalary = breakdown.NetPayableSalary,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });

            totalNet += breakdown.NetPayableSalary;
        }

        existingSheet.TotalNetPayable = totalNet;
        existingSheet.PayrollStatus = PayrollStatus.Generated;
        existingSheet.UpdatedAt = DateTime.UtcNow;
        existingSheet.UpdatedBy = _currentUserService.UserName;
        period.PayrollStatus = PayrollStatus.Generated;

        await _payrollRepository.SaveChangesAsync();
        return (true, null, existingSheet.Id);
    }

    public async Task<IReadOnlyList<PayrollDetailVm>> GetSheetDetailsAsync(int sheetId)
    {
        var details = await _payrollRepository.GetDetailsBySheetAsync(sheetId, _currentUserService.CompanyId);
        return details.Select(MapDetailVm).ToList();
    }

    public async Task<IReadOnlyList<PayrollSummaryVm>> GetSummaryAsync(int sheetId)
    {
        var details = await _payrollRepository.GetDetailsBySheetAsync(sheetId, _currentUserService.CompanyId);

        return details
            .GroupBy(d => d.Employee?.Department?.Name ?? "Unassigned")
            .Select(g => new PayrollSummaryVm(
                g.Key,
                g.Count(),
                g.Sum(x => x.GrossSalary),
                g.Sum(x => x.NetPayableSalary),
                g.Sum(x => x.OvertimeAmount),
                g.Sum(x => x.NightBillAmount),
                g.Sum(x => x.HolidayBillAmount)))
            .OrderBy(s => s.Department)
            .ToList();
    }

    public async Task<PayrollDetailVm?> GetPayslipAsync(int detailId)
    {
        var detail = await _context.PayrollDetails
            .Include(d => d.Employee)
            .FirstOrDefaultAsync(d => d.Id == detailId && d.CompanyId == _currentUserService.CompanyId);
        return detail == null ? null : MapDetailVm(detail);
    }

    public async Task<IReadOnlyList<AdvanceSalaryVm>> GetAdvancesAsync()
    {
        return await _context.AdvanceSalaries
            .Include(a => a.Employee)
            .Where(a => a.CompanyId == _currentUserService.CompanyId)
            .OrderByDescending(a => a.RequestDate)
            .Select(a => new AdvanceSalaryVm(
                a.Id,
                a.EmployeeId,
                a.Employee!.FullName,
                a.Amount,
                a.MonthlyDeduction,
                a.RemainingBalance,
                a.AdvanceStatus.ToString(),
                a.Reason))
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> SaveAdvanceAsync(AdvanceSalaryVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            _context.AdvanceSalaries.Add(new AdvanceSalary
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                Amount = model.Amount,
                MonthlyDeduction = model.MonthlyDeduction,
                RemainingBalance = model.Amount,
                RequestDate = DateTime.UtcNow,
                Reason = model.Reason?.Trim(),
                AdvanceStatus = AdvanceSalaryStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var advance = await _context.AdvanceSalaries
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.CompanyId == companyId);
            if (advance == null) return (false, "Advance record not found.");

            advance.MonthlyDeduction = model.MonthlyDeduction;
            advance.Reason = model.Reason?.Trim();
            advance.UpdatedAt = DateTime.UtcNow;
            advance.UpdatedBy = _currentUserService.UserName;
        }

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ApproveAdvanceAsync(int id)
    {
        var advance = await _context.AdvanceSalaries
            .FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == _currentUserService.CompanyId);
        if (advance == null) return (false, "Advance record not found.");

        advance.AdvanceStatus = AdvanceSalaryStatus.Approved;
        advance.ApprovedDate = DateTime.UtcNow;
        advance.RemainingBalance = advance.Amount;
        advance.UpdatedAt = DateTime.UtcNow;
        advance.UpdatedBy = _currentUserService.UserName;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<SalaryIncrementVm>> GetIncrementsAsync()
    {
        return await _context.SalaryIncrements
            .Include(i => i.Employee)
            .Where(i => i.CompanyId == _currentUserService.CompanyId)
            .OrderByDescending(i => i.EffectiveDate)
            .Select(i => new SalaryIncrementVm(
                i.Id,
                i.EmployeeId,
                i.Employee!.FullName,
                i.PreviousGross,
                i.NewGross,
                i.EffectiveDate,
                i.Reason))
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> SaveIncrementAsync(SalaryIncrementVm model)
    {
        var companyId = _currentUserService.CompanyId;
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == model.EmployeeId && e.CompanyId == companyId);
        if (employee == null) return (false, "Employee not found.");

        if (model.Id == 0)
        {
            _context.SalaryIncrements.Add(new SalaryIncrement
            {
                CompanyId = companyId,
                EmployeeId = model.EmployeeId,
                PreviousGross = employee.GrossSalary,
                NewGross = model.NewGross,
                EffectiveDate = model.EffectiveDate.Date,
                Reason = model.Reason?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });

            employee.GrossSalary = model.NewGross;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = _currentUserService.UserName;
        }
        else
        {
            var increment = await _context.SalaryIncrements
                .FirstOrDefaultAsync(i => i.Id == model.Id && i.CompanyId == companyId);
            if (increment == null) return (false, "Increment record not found.");

            increment.NewGross = model.NewGross;
            increment.EffectiveDate = model.EffectiveDate.Date;
            increment.Reason = model.Reason?.Trim();
            increment.UpdatedAt = DateTime.UtcNow;
            increment.UpdatedBy = _currentUserService.UserName;
        }

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<BillRateConfigVm>> GetBillRatesAsync()
    {
        return await _context.BillRateConfigs
            .Include(b => b.Shift)
            .Where(b => b.CompanyId == _currentUserService.CompanyId)
            .Select(b => new BillRateConfigVm(
                b.Id,
                (int)b.BillType,
                (int)b.RateType,
                b.Amount,
                b.ShiftId,
                b.Shift != null ? b.Shift.Name : null))
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> SaveBillRateAsync(BillRateConfigVm model)
    {
        var companyId = _currentUserService.CompanyId;

        if (model.Id == 0)
        {
            _context.BillRateConfigs.Add(new BillRateConfig
            {
                CompanyId = companyId,
                BillType = (BillType)model.BillType,
                RateType = (BillRateType)model.RateType,
                Amount = model.Amount,
                ShiftId = model.ShiftId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            });
        }
        else
        {
            var config = await _context.BillRateConfigs
                .FirstOrDefaultAsync(b => b.Id == model.Id && b.CompanyId == companyId);
            if (config == null) return (false, "Bill rate not found.");

            config.BillType = (BillType)model.BillType;
            config.RateType = (BillRateType)model.RateType;
            config.Amount = model.Amount;
            config.ShiftId = model.ShiftId;
            config.UpdatedAt = DateTime.UtcNow;
            config.UpdatedBy = _currentUserService.UserName;
        }

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<IReadOnlyList<DailySalarySheetVm>> GetDailySalarySheetAsync(DateTime date)
    {
        var companyId = _currentUserService.CompanyId;
        var dateOnly = date.Date;
        var month = new DateTime(dateOnly.Year, dateOnly.Month, 1);
        var monthEnd = month.AddMonths(1).AddDays(-1);
        var workingDays = CountWorkingDays(month, monthEnd);

        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.CompanyId == companyId)
            .ToListAsync();

        var attendances = await _context.DailyAttendances
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date == dateOnly)
            .ToListAsync();

        var attMap = attendances.ToDictionary(a => a.EmployeeId);

        return employees.Select(e =>
        {
            attMap.TryGetValue(e.Id, out var att);
            var dailyRate = workingDays > 0 ? Math.Round(e.GrossSalary / workingDays, 2) : 0m;
            var isPresent = att != null && !att.IsAbsent && att.InTime.HasValue;
            var isAbsent = att == null || att.IsAbsent;
            return new DailySalarySheetVm(
                e.EmployeeCode, e.FullName, e.Department?.Name ?? "-",
                e.GrossSalary, workingDays, dailyRate, dateOnly,
                isPresent, isAbsent, att?.IsHoliday ?? false,
                isPresent ? dailyRate : 0m);
        })
        .OrderBy(v => v.Department).ThenBy(v => v.EmployeeCode)
        .ToList();
    }

    public async Task<IReadOnlyList<DailySalarySummaryVm>> GetDailySalarySummaryAsync(DateTime from, DateTime to)
    {
        var companyId = _currentUserService.CompanyId;
        var fromDate = from.Date;
        var toDate = to.Date;

        var employees = await _context.Employees.Where(e => e.CompanyId == companyId).ToListAsync();
        var attendances = await _context.DailyAttendances
            .Where(a => a.CompanyId == companyId && a.AttendanceDate.Date >= fromDate && a.AttendanceDate.Date <= toDate)
            .ToListAsync();

        var result = new List<DailySalarySummaryVm>();
        for (var d = fromDate; d <= toDate; d = d.AddDays(1))
        {
            var dayAtt = attendances.Where(a => a.AttendanceDate.Date == d).ToList();
            var workingDays = CountWorkingDays(new DateTime(d.Year, d.Month, 1), new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month)));
            var totalPayable = employees.Sum(e => workingDays > 0 ? e.GrossSalary / workingDays : 0m);
            var presentCount = dayAtt.Count(a => !a.IsAbsent && a.InTime.HasValue);
            var absentCount = dayAtt.Count(a => a.IsAbsent);
            var holidayCount = dayAtt.Count(a => a.IsHoliday);
            var absentDeduction = employees.Sum(e => workingDays > 0 ? (dayAtt.Any(a => a.EmployeeId == e.Id && a.IsAbsent) ? e.GrossSalary / workingDays : 0m) : 0m);
            result.Add(new DailySalarySummaryVm(d, presentCount, absentCount, holidayCount,
                Math.Round(totalPayable * presentCount / Math.Max(employees.Count, 1), 2),
                Math.Round(absentDeduction, 2)));
        }
        return result;
    }

    private SalaryBreakdown CalculateSalaryBreakdown(
        decimal grossSalary,
        int totalWorkingDays,
        int absentDays)
    {
        var medical = MedicalAllowanceFixed;
        var food = FoodAllowanceFixed;
        var conveyance = ConveyanceAllowanceFixed;
        var basic = Math.Round((grossSalary - FixedAllowanceTotal) / 1.5m, 2);
        var houseRent = grossSalary - basic - FixedAllowanceTotal;
        var otRate = Math.Round(basic / OtHoursDivisor * 2m, 2);

        var dailyRate = totalWorkingDays > 0 ? grossSalary / totalWorkingDays : 0m;
        var absentDeduction = Math.Round(dailyRate * absentDays, 2);

        return new SalaryBreakdown
        {
            GrossSalary = grossSalary,
            BasicSalary = basic,
            HouseRent = houseRent,
            MedicalAllowance = medical,
            FoodAllowance = food,
            ConveyanceAllowance = conveyance,
            OtRate = otRate,
            AbsentDeduction = absentDeduction
        };
    }

    private static int CountWorkingDays(DateTime start, DateTime end)
    {
        var count = 0;
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Friday)
                count++;
        }
        return Math.Max(count, 1);
    }

    private static PayrollDetailVm MapDetailVm(PayrollDetail d) => new(
        d.Id,
        d.EmployeeId,
        d.Employee?.FullName ?? string.Empty,
        d.Employee?.EmployeeCode ?? string.Empty,
        d.GrossSalary,
        d.BasicSalary,
        d.HouseRent,
        d.MedicalAllowance,
        d.FoodAllowance,
        d.ConveyanceAllowance,
        d.AttendancePayableDays,
        d.AbsentDeduction,
        d.OvertimeAmount,
        d.NightBillAmount,
        d.HolidayBillAmount,
        d.AdvanceDeduction,
        d.NetPayableSalary);

    private sealed class SalaryBreakdown
    {
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRent { get; set; }
        public decimal MedicalAllowance { get; set; }
        public decimal FoodAllowance { get; set; }
        public decimal ConveyanceAllowance { get; set; }
        public decimal OtRate { get; set; }
        public decimal AbsentDeduction { get; set; }
        public decimal OvertimeAmount { get; set; }
        public decimal NightBillAmount { get; set; }
        public decimal HolidayBillAmount { get; set; }
        public decimal AdvanceDeduction { get; set; }
        public decimal NetPayableSalary { get; set; }
    }
}
