using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class AccountRepository
    {
        ListentogetherContext _context;

        public Account GetAccount(Account account)
        {
            _context = new();

            var result = _context.Accounts
                .Where(x => x.Username == account.Username && x.Password == account.Password)
                .FirstOrDefault();
            return result;
            
        }

        public List<Account> GetAllAccounts()
        {
            _context = new();
            var result = _context.Accounts
                .ToList();
            return result;
        }
        
        public void CreateAccount(Account account)
        {
            _context = new();
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public Account FindAccount(string username)
        {
            _context = new();
            var result = _context.Accounts
                .Where(x => x.Username == username)
                .FirstOrDefault();
            return result;
        }
    }
}
