using AccuPunchure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuPunchure.Data.Repositories;

public class EmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> GetAllActiveByOrganizationId(int organizationId)
    {
        return await _db.Employees
            .Where(e => e.OrganizationId == organizationId && e.IsActive)
            .ToListAsync();
    }

    public async Task<Employee?> GetByEmployeeId(int employeeId)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<Employee?> GetByUserId(int userId)
    {
        return await _db.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.User != null && e.User.UserId == userId);
    }

    public async Task<Employee?> GetByEmail(string email)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Employee> CreateEmployee(Employee employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task DeactivateEmployee(int employeeId)
    {
        var employee = await _db.Employees.FindAsync(employeeId);
        if (employee is null) return;

        employee.IsActive = false;
        await _db.SaveChangesAsync();
    }
}
