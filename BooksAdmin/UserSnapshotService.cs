using BooksAdmin.Models;
using Microsoft.EntityFrameworkCore;

//namespace BooksAdmin
//{
    public interface IUserSnapshotService
    {
        Task AddUserAsync(string userId, string email, string name);
    }

    public class UserSnapshotService : IUserSnapshotService
    {
        private readonly BooksAdminContext _context;

        public UserSnapshotService(BooksAdminContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(string userId, string email, string name)
        {
            var userSnapshot = await EntityFrameworkQueryableExtensions
                .FirstOrDefaultAsync(
                    _context.UserSnapshot,
                    u => u.UserId == userId);

            // If the user doesn't exist, add them to the database. Otherwise, update their email and name.
            if (userSnapshot == null)
            {
                _context.UserSnapshot.Add(new UserSnapshot
                {
                    UserId = userId,
                    Email = email,
                    Name = name
                });
            }
            else
            {
                userSnapshot.Email = email;
                userSnapshot.Name = name;
                _context.UserSnapshot.Update(userSnapshot);
            }

            await _context.SaveChangesAsync();
        }
    }
//}
