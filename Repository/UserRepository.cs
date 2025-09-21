using Microsoft.Data.SqlClient;
using simple_note_app_api.Models;
using simple_note_app_api.Services;

namespace simple_note_app_api.Repository
{
    public interface IUserRepository
    {
        Task<User> CreateUser(string username, string hasedPassword);
        Task<User?> GetUserByUsername(string username);
        Task<User?> GetUserById(int id);
    }

    public class UserRepository: IUserRepository
    {
        private readonly IDBService _dbService;

        public UserRepository(IDBService dbService)
        {
            _dbService = dbService;
        }

        public async Task<User> CreateUser(string username, string hasedPassword)
        {
            var query = @"
                INSERT INTO Users (Username, Password)
                OUTPUT INSERTED.Id, INSERTED.Username, INSERTED.Password, INSERTED.CreatedAt, INSERTED.UpdatedAt
                VALUES (@Username, @Password)
                ";
           
            var p = new { Username = username, Password = hasedPassword };

            var user = await _dbService.CreateOrSave<User>(query, p);

            if (user == null)
            {
                throw new Exception("Failed to create user");
            }

            return user;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            var query = @"
                SELECT TOP 1 * FROM Users WHERE Username = @Username
                ";
            var p = new { Username = username };
            var user = await _dbService.GetOne<User>(query, p);
            return user;
        }

        public async Task<User?> GetUserById(int id)
        {
            var query = @"
                SELECT TOP 1 * FROM Users WHERE Id = @Id
                ";
            var p = new { Id = id };
            var user = await _dbService.GetOne<User>(query, p);
            return user;
        }
    }
}
