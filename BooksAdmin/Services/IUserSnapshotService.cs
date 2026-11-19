namespace BooksAdmin.Services
{
    public interface IUserSnapshotService
    {
        Task AddUserAsync(string userId, string email, string name);
    }
}