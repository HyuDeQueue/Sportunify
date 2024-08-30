using Repositories.Models;

namespace Repositories.Repositories
{
    public class AccountRepository
    {
        ListentogetherContext _context;

        public Account GetAccount(Account account)
        {
            _context = new();

            var result = _context.Accounts
                .Where(x => x.Username == account.Username)
                .FirstOrDefault();
            if (result != null && BCrypt.Net.BCrypt.Verify(account.Password, result.Password))
            {
                return result;
            }

            return null;

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
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
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
