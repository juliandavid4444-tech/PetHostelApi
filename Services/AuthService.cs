using System;
using PetHostelApi.Entities;

namespace PetHostelApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public User Authenticate(string userName, string password)
        {
            var user = _context.User.FirstOrDefault(u => u.user_user == userName && u.user_password == password);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return user;
        }
    }
}

