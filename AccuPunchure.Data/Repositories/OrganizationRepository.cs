using AccuPunchure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuPunchure.Data.Repositories;

public class OrganizationRepository
{
    private readonly AppDbContext _db;

    public OrganizationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Organization?> GetBySlug(string slug)
    {
        return await _db.Organizations
            .FirstOrDefaultAsync(o => o.Slug == slug);
    }
}
