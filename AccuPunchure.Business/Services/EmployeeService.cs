using AccuPunchure.Business.Enums;
using AccuPunchure.Business.Models;
using AccuPunchure.Data.Repositories;

namespace AccuPunchure.Business.Services;

public class EmployeeService
{
    private readonly EmployeeRepository _employeeRepo;
    private readonly PunchRepository    _punchRepo;

    public EmployeeService(EmployeeRepository employeeRepo, PunchRepository punchRepo)
    {
        _employeeRepo = employeeRepo;
        _punchRepo    = punchRepo;
    }

    public async Task<EmployeeModel?> GetEmployeeWithPunches(int employeeId, TimeSpanFilter filter)
    {
        var employee = await _employeeRepo.GetByEmployeeId(employeeId);
        if (employee is null) return null;

        var from   = GetStartDate(filter);
        var punches = await _punchRepo.GetPunchesByEmployeeAndDateRange(employeeId, from, DateTime.UtcNow);

        var model = MapToModel(employee);
        model.Punches = punches.Select(MapPunchToModel).ToList();
        return model;
    }

    public async Task<List<EmployeeModel>> GetAllEmployeesWithPunches(int organizationId, TimeSpanFilter filter)
    {
        var from      = GetStartDate(filter);
        var employees = await _employeeRepo.GetAllActiveByOrganizationId(organizationId);
        var allPunches = await _punchRepo.GetPunchesByOrganizationAndDateRange(organizationId, from, DateTime.UtcNow);

        var punchMap = allPunches
            .GroupBy(p => p.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Select(MapPunchToModel).ToList());

        return employees.Select(e =>
        {
            var model = MapToModel(e);
            model.Punches = punchMap.GetValueOrDefault(e.EmployeeId) ?? new();
            return model;
        }).ToList();
    }

    public async Task<EmployeeModel> CreateEmployee(EmployeeModel model)
    {
        var entity = new Data.Models.Employee
        {
            FirstName      = model.FirstName,
            LastName       = model.LastName,
            Email          = model.Email,
            PhoneNumber    = model.PhoneNumber,
            IsActive       = model.IsActive,
            OrganizationId = model.OrganizationId
        };
        var created = await _employeeRepo.CreateEmployee(entity);
        return MapToModel(created);
    }

    public async Task<EmployeeModel> UpdateEmployee(EmployeeModel model)
    {
        var entity = new Data.Models.Employee
        {
            EmployeeId     = model.EmployeeId,
            FirstName      = model.FirstName,
            LastName       = model.LastName,
            Email          = model.Email,
            PhoneNumber    = model.PhoneNumber,
            IsActive       = model.IsActive,
            OrganizationId = model.OrganizationId
        };
        var updated = await _employeeRepo.UpdateEmployee(entity);
        return MapToModel(updated);
    }

    public async Task DeactivateEmployee(int employeeId)
    {
        await _employeeRepo.DeactivateEmployee(employeeId);
    }

    public async Task<EmployeeModel?> GetEmployeeByUserId(int userId)
    {
        var employee = await _employeeRepo.GetByUserId(userId);
        return employee is null ? null : MapToModel(employee);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static EmployeeModel MapToModel(Data.Models.Employee e) => new()
    {
        EmployeeId     = e.EmployeeId,
        FirstName      = e.FirstName,
        LastName       = e.LastName,
        Email          = e.Email,
        PhoneNumber    = e.PhoneNumber,
        IsActive       = e.IsActive,
        OrganizationId = e.OrganizationId
    };

    private static PunchModel MapPunchToModel(Data.Models.Punch p) => new()
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
