using Repositories.Models;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService
    {
        private AccountRepository _repo = new();
        public void GetAllAccounts()
        {
            List<Account> accounts = _repo.GetAllAccounts();
        }

        public Account CheckAccountExists(Account account)
        {
            return _repo.FindAccount(account.Username);
        }
        public void Register(Account account)
        {
            _repo.CreateAccount(account);
        }

        public Account Login(Account account)
        {
            Account loggedIn = _repo.GetAccount(account);
            if (loggedIn is not null)
            { 
                return loggedIn;
            }
            else
            {
                return null;
            }
        }

        public void Logout()
        {

        }

        public void GetSongsFromAccount()
        {

        }

        public void GetPlaylistsFromAccount()
        {

        }
    }
}
