using ERP.Web.Core.Entities;
using ERP.Web.Core.Enums;
using ERP.Web.Core.Interfaces.Repositories;
using ERP.Web.Core.Interfaces.Services;

namespace ERP.Web.Infrastructure.Services;

public class PunchRecordService : IPunchRecordService
{
    private readonly IPunchRecordRepository _punchRecordRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PunchRecordService(
        IPunchRecordRepository punchRecordRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _punchRecordRepository = punchRecordRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<PunchRecordVm>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var records = await _punchRecordRepository.GetByDateRangeAsync(
            _currentUserService.CompanyId, from.Date, to.Date.AddDays(1).AddTicks(-1));

        return records.Select(p => new PunchRecordVm(
            p.Id,
            p.Employee?.FullName ?? string.Empty,
            p.Employee?.PunchNumber ?? string.Empty,
            p.PunchTime,
            p.PunchType.ToString(),
            p.Source.ToString())).ToList();
    }

    public async Task<(bool Success, string? Error)> CreateManualAsync(PunchRecordCreateVm model)
    {
        var punchType = model.PunchType == (int)PunchType.Out ? PunchType.Out : PunchType.In;

        await _punchRecordRepository.AddAsync(new PunchRecord
        {
            CompanyId = _currentUserService.CompanyId,
            EmployeeId = model.EmployeeId,
            PunchTime = model.PunchTime,
            PunchType = punchType,
            Source = PunchSource.Manual,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserName
        });

        await _unitOfWork.SaveChangesAsync();
        return (true, null);
    }
}
