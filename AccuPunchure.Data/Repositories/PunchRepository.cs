using AccuPunchure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuPunchure.Data.Repositories;

public class PunchRepository
{
    private readonly AppDbContext _db;

    public PunchRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Punch> CreatePunch(Punch punch)
    {
        _db.Punches.Add(punch);
        await _db.SaveChangesAsync();
        return punch;
    }

    /// <summary>
    /// Admin corrections only — updates StartTime and/or EndTime on an existing punch.
    /// </summary>
    public async Task<Punch?> UpdatePunch(int punchId, DateTime? startTime, DateTime? endTime)
    {
        var punch = await _db.Punches.FindAsync(punchId);
        if (punch is null) return null;

        if (startTime.HasValue) punch.StartTime = startTime.Value;
        if (endTime.HasValue)   punch.EndTime   = endTime.Value;

        await _db.SaveChangesAsync();
        return punch;
    }

    public async Task<List<Punch>> GetPunchesByEmployeeAndDateRange(int employeeId, DateTime from, DateTime to)
    {
        return await _db.Punches
            .Where(p => p.EmployeeId == employeeId
                     && p.StartTime >= from
                     && p.StartTime <= to)
            .OrderBy(p => p.StartTime)
            .ToListAsync();
    }

    public async Task<List<Punch>> GetPunchesByOrganizationAndDateRange(int organizationId, DateTime from, DateTime to)
    {
        return await _db.Punches
            .Include(p => p.Employee)
            .Where(p => p.Employee.OrganizationId == organizationId
                        && p.StartTime >= from
                        && p.StartTime <= to)
            .OrderBy(p => p.EmployeeId)
            .ThenBy(p => p.StartTime)
            .ToListAsync();
    }
}
