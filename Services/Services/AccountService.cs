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
        public String Register(Account account)
        {
            Account isRegistered = _repo.FindAccount(account.Username);

            if (isRegistered is not null)
            {
                return("Username already exists!");
            }
            else
            {
                _repo.CreateAccount(account);
                return("Account created successfully!");
            }
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
