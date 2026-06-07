using AccuPunchure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AccuPunchure.Data.Repositories;

public class UserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }
}
