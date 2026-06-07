using AccuPunchure.Business.Enums;
using AccuPunchure.Business.Models;
using AccuPunchure.Data.Repositories;

namespace AccuPunchure.Business.Services;

public class PunchServices
{
    private readonly PunchRepository _punchRepo;

    public PunchServices(PunchRepository punchRepo)
    {
        _punchRepo = punchRepo;
    }

    public async Task<PunchModel> CreatePunch(PunchModel model)
    {
        var entity = new Data.Models.Punch
        {
            EmployeeId = model.EmployeeId,
            StartTime  = model.StartTime,
            EndTime    = model.EndTime
        };
        var created = await _punchRepo.CreatePunch(entity);
        return MapToModel(created);
    }

    /// <summary>Admin corrections only — flags the returned model as edited.</summary>
    public async Task<PunchModel?> UpdatePunch(PunchModel model)
    {
        var updated = await _punchRepo.UpdatePunch(model.PunchId, model.StartTime, model.EndTime);
        if (updated is null) return null;

        var result = MapToModel(updated);
        result.IsEdited = true;
        return result;
    }

    public async Task<List<PunchModel>> GetPunchesByEmployee(int employeeId, TimeSpanFilter filter)
    {
        var from   = GetStartDate(filter);
        var punches = await _punchRepo.GetPunchesByEmployeeAndDateRange(employeeId, from, DateTime.UtcNow);
        return punches.Select(MapToModel).ToList();
    }

    public async Task<List<PunchModel>> GetPunchesByOrganization(int organizationId, TimeSpanFilter filter)
    {
        var from   = GetStartDate(filter);
        var punches = await _punchRepo.GetPunchesByOrganizationAndDateRange(organizationId, from, DateTime.UtcNow);
        return punches.Select(MapToModel).ToList();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static PunchModel MapToModel(Data.Models.Punch p) => new()
    {
        PunchId    = p.PunchId,
        EmployeeId = p.EmployeeId,
        StartTime  = p.StartTime,
        EndTime    = p.EndTime,
        IsEdited   = false
    };

    private static DateTime GetStartDate(TimeSpanFilter filter) => filter switch
    {
        TimeSpanFilter.OneDay   => DateTime.UtcNow.AddDays(-1),
        TimeSpanFilter.OneWeek  => DateTime.UtcNow.AddDays(-7),
        TimeSpanFilter.TwoWeeks => DateTime.UtcNow.AddDays(-14),
        TimeSpanFilter.OneMonth => DateTime.UtcNow.AddMonths(-1),
        TimeSpanFilter.OneYear  => DateTime.UtcNow.AddYears(-1),
        _                       => DateTime.UtcNow.AddDays(-7)
    };
}
