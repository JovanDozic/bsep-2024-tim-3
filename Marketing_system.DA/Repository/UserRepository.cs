﻿using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;
using Microsoft.EntityFrameworkCore;

namespace Marketing_system.DA.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }

        public UserRepository(DataContext context) : base(context) 
        {
            
        }
        public new IEnumerable<User> GetAll()
        {
            return _dbContext.Set<User>().ToList();
        }
        /*public bool CheckPasswordAsync(User? user, string password)
        {
            throw new NotImplementedException();
        }*/

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Email == email && x.AccountStatus == AccountStatus.Active);
        }

        public string? GetSaltByEmail(string email)
        {
            var user = _dbContext.Set<User>().SingleOrDefault(x => x.Email == email);
            if (user == null) { return null; }
            return user.Salt;
        }

        public string? GetPasswordByEmail(string email)
        {
            var user = _dbContext.Set<User>().SingleOrDefault(x => x.Email == email);
            if (user == null) { return null; }
            return user.Password;
        }
    }
}
