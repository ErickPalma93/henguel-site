using System.Security.Cryptography;
using System.Text;
using HenguelSistemas.Domain.Entities;
using HenguelSistemas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HenguelSistemas.Application.Services;

public class VisitTrackingService
{
    private readonly AppDbContext _db;

    public VisitTrackingService(AppDbContext db) => _db = db;

    public async Task TrackAsync(string page, string? ip, string? userAgent)
    {
        string? hashedIp = ip is not null ? HashIp(ip) : null;

        _db.PageVisits.Add(new PageVisit
        {
            Page = page,
            HashedIp = hashedIp,
            UserAgent = userAgent?[..Math.Min(userAgent.Length, 500)],
            VisitedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    private static string HashIp(string ip)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(ip + "henguel_salt_2024"));
        return Convert.ToHexString(bytes)[..16];
    }

    public async Task<int> GetTotalVisitsAsync() =>
        await _db.PageVisits.CountAsync();
}
