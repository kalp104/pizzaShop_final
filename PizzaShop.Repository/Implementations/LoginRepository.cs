using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class LoginRepository : ILoginRepository
{
    private readonly PizzaShop2Context _context;

    public LoginRepository(PizzaShop2Context context)
    {
        _context = context;       
    }


    public async Task<Account?> GetAccountById(int accountid)
    {
        return await _context.Accounts
                     .Where(u => u.Accountid == accountid && u.Isdeleted == false)
                     .FirstOrDefaultAsync();
    }

    public async Task<Account?> GetAccountByEmail(string email)
    {
        return await _context.Accounts
                     .Where(u => u.Email == email && u.Isdeleted == false)
                     .FirstOrDefaultAsync();
    }
    
    public async Task<Account?> GetAccountByUsername(string username)
    {
        return await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);
    }


    public async Task<string> UpdateAccount(Account account)
    {
        try{
            _context.Update(account);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddAccount(Account account)
    {
        try{
            _context.Add(account);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task UpdatePasswordResetRequest(PasswordResetRequest resetRequest)
    {
        _context.Update(resetRequest);
        await _context.SaveChangesAsync();
    }

    public async Task AddPasswordResetRequest(PasswordResetRequest resetRequest)
    {
        _context.Add(resetRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetRequest?> GetTokenData(string token)
    {
        PasswordResetRequest? result = await _context.PasswordResetRequests
                .Where(f => f.Guidtoken == token)
                .FirstOrDefaultAsync();

        return result;
    }


    public async Task AddUser2faAuth(User2faAuth user2faAuth)
    {
        _context.Add(user2faAuth);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser2faAuth(User2faAuth user2faAuth)
    {
        _context.Update(user2faAuth);
        await _context.SaveChangesAsync();
    }


    public async Task<User2faAuth> GetUser2faAuth(string email)
    {
        var result = await _context.User2faAuths
            .Where(f => f.Email.Contains(email))
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task deleteAllUser2faAuthByEmail(string Email)
    {
        List<User2faAuth> user2faAuths = await _context.User2faAuths
            .Where(f => f.Email.Contains(Email))
            .ToListAsync();
        
        _context.User2faAuths.RemoveRange(user2faAuths);
        await _context.SaveChangesAsync();
    }
}
