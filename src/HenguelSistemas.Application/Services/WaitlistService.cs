using HenguelSistemas.Application.Interfaces;
using HenguelSistemas.Domain.Entities;
using HenguelSistemas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HenguelSistemas.Application.Services;

public class WaitlistService : IWaitlistService
{
    private readonly AppDbContext _db;

    public WaitlistService(AppDbContext db) => _db = db;

    public async Task<(bool Success, string Message)> RegisterAsync(
        string name, string email, bool consent)
    {
        if (!consent)
            return (false, "É necessário aceitar os termos para se cadastrar.");

        var exists = await _db.WaitlistEntries
            .AnyAsync(e => e.Email.ToLower() == email.ToLower());

        if (exists)
            return (false, "Este e-mail já está na lista de espera.");

        _db.WaitlistEntries.Add(new WaitlistEntry
        {
            Name = name.Trim(),
            Email = email.Trim().ToLower(),
            ConsentGiven = consent,
            ConsentDate = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return (true, "Cadastro realizado! Entraremos em contato em breve. 🎉");
    }

    public async Task<int> GetCountAsync() =>
        await _db.WaitlistEntries.CountAsync();
}