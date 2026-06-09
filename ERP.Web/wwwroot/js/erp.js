window.erpInitDataTable = function (selector, options) {
    if (!window.jQuery || !jQuery.fn.DataTable) return;
    const node = document.querySelector(selector);
    if (!node) return;
    node.classList.add('erp-table');
    if (typeof DataTable !== 'undefined' && DataTable.isDataTable(node)) {
        return DataTable.api(node);
    }
    const defaults = {
        pageLength: 10,
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, 'All']],
        responsive: true,
        order: [],
        searching: false,
        dom: 'rt<"erp-dt-footer"<"erp-dt-footer-left"li><"erp-dt-footer-right"p>>',
        language: {
            lengthMenu: 'Rows per page _MENU_',
            info: '_START_–_END_ of _TOTAL_ row(s)',
            infoEmpty: '0 row(s)',
            paginate: { first: 'First', last: 'Last', next: 'Next', previous: 'Previous' }
        }
    };
    const config = Object.assign({}, defaults, options || {});
    const actionHeaderIndex = Array.from(node.querySelectorAll('thead th'))
        .findIndex(th => th.textContent.trim().toLowerCase() === 'actions');
    if (actionHeaderIndex >= 0) {
        config.columnDefs = [
            ...(config.columnDefs || []),
            { orderable: false, targets: [actionHeaderIndex] }
        ];
    }
    return new DataTable(selector, config);
};

window.erpSaveFilterPreset = async function (module, name, formEl) {
    if (!name) { alert('Enter a preset name'); return; }
    const formData = new FormData(formEl);
    const params = Object.fromEntries(formData.entries());
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const body = new FormData();
    body.append('module', module);
    body.append('name', name);
    body.append('filterJson', JSON.stringify(params));
    body.append('isDefault', 'false');
    if (token) body.append('__RequestVerificationToken', token);
    const res = await fetch('/Filter/SavePreset', { method: 'POST', body });
    const data = await res.json();
    alert(data.success ? 'Filter preset saved' : (data.error || 'Save failed'));
    if (data.success) erpLoadFilterPresets(module, 'preset-list-' + module);
};

window.erpLoadFilterPresets = async function (module, containerId) {
    const el = document.getElementById(containerId);
    if (!el) return;
    try {
        const res = await fetch('/Filter/Presets?module=' + encodeURIComponent(module));
        const presets = await res.json();
        if (!presets.length) {
            el.innerHTML = '<p class="text-muted">No saved presets yet. Apply filters and click Save Preset.</p>';
            return;
        }
        el.innerHTML = presets.map(p => {
            const json = encodeURIComponent(p.filterJson || p.FilterJson || '{}');
            const name = p.name || p.Name || 'Preset';
            return `<div class="flex items-center justify-between rounded-md border border-border px-3 py-2">
                <span class="font-medium text-foreground">${name}</span>
                <button type="button" class="erp-btn-ghost text-xs" data-filter="${json}" onclick="erpApplyPresetFromBtn(this)">Apply</button>
            </div>`;
        }).join('');
    } catch {
        el.innerHTML = '<p class="text-muted">Could not load presets.</p>';
    }
};

window.erpApplyPresetFromBtn = function (btn) {
    try {
        const params = JSON.parse(decodeURIComponent(btn.getAttribute('data-filter')));
        window.location.search = '?' + new URLSearchParams(params).toString();
    } catch { alert('Invalid preset data'); }
};

window.erpApplyPreset = function (module, filterJson) {
    try {
        const params = JSON.parse(filterJson);
        const qs = new URLSearchParams(params).toString();
        window.location.search = qs;
    } catch { alert('Invalid preset data'); }
};

window.erpInitDatePickers = function (context) {
    if (!window.jQuery || !jQuery.fn.datepicker) return;
    const $root = context ? $(context) : $(document);
    $root.find('input[type="date"], input[data-datepicker]').each(function () {
        const $input = $(this);
        if ($input.data('erp-datepicker')) return;

        const val = $input.val();
        const placeholder = $input.attr('placeholder') || 'yyyy-mm-dd';

        const triggerBtn =
            '<button type="button" class="erp-date-trigger" tabindex="-1" title="Open calendar">' +
            '<svg class="h-4 w-4" fill="none" stroke="currentColor" stroke-width="1.75" viewBox="0 0 24 24">' +
            '<rect x="3" y="4" width="18" height="18" rx="2"/><path d="M16 2v4M8 2v4M3 10h18"/></svg></button>';
        if (!$input.parent().hasClass('erp-date-field')) {
            $input.wrap('<div class="erp-date-field"></div>');
            $input.after(triggerBtn);
        } else if ($input.siblings('.erp-date-trigger').length === 0) {
            $input.after(triggerBtn);
        }

        if (!$input.hasClass('erp-input')) $input.addClass('erp-input');
        $input.addClass('erp-datepicker-input');
        $input.attr({ type: 'text', autocomplete: 'off', placeholder: placeholder });

        $input.datepicker({
            dateFormat: 'yy-mm-dd',
            changeMonth: true,
            changeYear: true,
            yearRange: '-80:+10',
            showButtonPanel: true,
            showAnim: 'fadeIn',
            beforeShow: function (input, inst) {
                setTimeout(function () { inst.dpDiv.addClass('erp-datepicker-popup'); }, 0);
            },
            onClose: function () { $(this).trigger('change'); }
        });

        if (val) {
            try { $input.datepicker('setDate', $.datepicker.parseDate('yy-mm-dd', val)); } catch { $input.val(val); }
        }

        $input.siblings('.erp-date-trigger').on('click', function (e) {
            e.preventDefault();
            $input.datepicker('show');
        });

        $input.data('erp-datepicker', true);
    });
};

window.erpInitSignaturePad = function (canvasId, hiddenInputId, existingData) {
    const canvas = document.getElementById(canvasId);
    const hidden = document.getElementById(hiddenInputId);
    if (!canvas || !hidden) return null;

    const ctx = canvas.getContext('2d');
    let drawing = false;
    let loaded = false;

    function syncSize() {
        const rect = canvas.getBoundingClientRect();
        const data = hidden.value || existingData;
        canvas.width = Math.max(rect.width, 300);
        canvas.height = 200;
        ctx.strokeStyle = '#1e293b';
        ctx.lineWidth = 2;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        if (data && !loaded) {
            const img = new Image();
            img.onload = function () {
                ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
                loaded = true;
            };
            img.src = data;
            hidden.value = data;
        }
    }

    syncSize();

    function pointerPos(e) {
        const rect = canvas.getBoundingClientRect();
        const t = e.touches && e.touches.length ? e.touches[0] : e;
        return {
            x: (t.clientX - rect.left) * (canvas.width / rect.width),
            y: (t.clientY - rect.top) * (canvas.height / rect.height)
        };
    }

    function startDraw(e) {
        e.preventDefault();
        drawing = true;
        const p = pointerPos(e);
        ctx.beginPath();
        ctx.moveTo(p.x, p.y);
    }

    function draw(e) {
        if (!drawing) return;
        e.preventDefault();
        const p = pointerPos(e);
        ctx.lineTo(p.x, p.y);
        ctx.stroke();
    }

    function endDraw() {
        if (!drawing) return;
        drawing = false;
        hidden.value = canvas.toDataURL('image/png');
        loaded = true;
    }

    canvas.addEventListener('mousedown', startDraw);
    canvas.addEventListener('mousemove', draw);
    canvas.addEventListener('mouseup', endDraw);
    canvas.addEventListener('mouseleave', endDraw);
    canvas.addEventListener('touchstart', startDraw, { passive: false });
    canvas.addEventListener('touchmove', draw, { passive: false });
    canvas.addEventListener('touchend', endDraw);

    return {
        clear() {
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            hidden.value = '';
            loaded = false;
        }
    };
};

document.addEventListener('alpine:init', function () {
    Alpine.store('search', {
        openModal: false,
        query: '',
        groups: [],
        quickLinks: [],
        loading: false,
        activeIndex: 0,
        flatItems: [],

        normalize(data) {
            return {
                groups: data.groups || data.Groups || [],
                items: data.items || data.Items || [],
                unreadCount: data.unreadCount ?? data.UnreadCount ?? 0
            };
        },

        async loadQuickLinks() {
            try {
                const res = await fetch('/Navbar/QuickLinks');
                if (!res.ok) return;
                const data = await res.json();
                this.quickLinks = Array.isArray(data) ? data : (data.items || data.Items || []);
            } catch { /* ignore */ }
        },

        open() {
            this.openModal = true;
            this.query = '';
            this.groups = [];
            this.flatItems = [];
            this.activeIndex = 0;
            document.body.style.overflow = 'hidden';
            if (!this.quickLinks.length) this.loadQuickLinks();
            setTimeout(() => {
                const input = document.querySelector('[x-ref="globalSearchInput"]');
                if (input) input.focus();
            }, 50);
        },

        close() {
            this.openModal = false;
            document.body.style.overflow = '';
        },

        handleShortcut(e) {
            if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
                e.preventDefault();
                this.openModal ? this.close() : this.open();
                return;
            }
            if (!this.openModal) return;
            if (e.key === 'Escape') {
                e.preventDefault();
                this.close();
            } else if (e.key === 'ArrowDown') {
                e.preventDefault();
                if (this.flatItems.length) this.activeIndex = (this.activeIndex + 1) % this.flatItems.length;
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (this.flatItems.length) this.activeIndex = (this.activeIndex - 1 + this.flatItems.length) % this.flatItems.length;
            } else if (e.key === 'Enter' && this.flatItems[this.activeIndex]) {
                e.preventDefault();
                window.location.href = this.flatItems[this.activeIndex].href;
                this.close();
            }
        },

        itemIndex(groupIdx, itemIdx) {
            let offset = 0;
            for (let i = 0; i < groupIdx; i++) offset += (this.groups[i]?.items?.length || 0);
            return offset + itemIdx;
        },

        rebuildFlat() {
            this.flatItems = [];
            for (const g of this.groups) {
                const items = g.items || g.Items || [];
                for (const item of items) this.flatItems.push(item);
            }
            this.activeIndex = 0;
        },

        async runSearch() {
            const q = this.query.trim();
            if (q.length < 2) {
                this.groups = [];
                this.flatItems = [];
                return;
            }
            this.loading = true;
            try {
                const res = await fetch('/Navbar/Search?q=' + encodeURIComponent(q));
                if (!res.ok) throw new Error('Search failed');
                const data = this.normalize(await res.json());
                this.groups = data.groups;
                this.rebuildFlat();
            } catch {
                this.groups = [];
                this.flatItems = [];
            } finally {
                this.loading = false;
            }
        }
    });
});

window.erpNotifications = function () {
    return {
        open: false,
        loading: false,
        unreadCount: 0,
        items: [],

        toggle() {
            this.open = !this.open;
            if (this.open) this.load();
        },

        async load() {
            this.loading = true;
            try {
                const res = await fetch('/Navbar/Notifications');
                if (!res.ok) throw new Error('Failed');
                const data = await res.json();
                this.items = data.items || data.Items || [];
                this.unreadCount = data.unreadCount ?? data.UnreadCount ?? 0;
            } catch {
                this.items = [];
                this.unreadCount = 0;
            } finally {
                this.loading = false;
            }
        }
    };
};

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('[data-datatable]').forEach(function (el) {
        if (!el.id) return;
        let opts = {};
        const raw = el.getAttribute('data-dt-options');
        if (raw) {
            try { opts = JSON.parse(raw); } catch { /* ignore invalid json */ }
        }
        erpInitDataTable('#' + el.id, opts);
    });
    erpInitDatePickers();
});
