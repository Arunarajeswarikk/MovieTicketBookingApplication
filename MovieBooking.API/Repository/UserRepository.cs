using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models;
using MovieBooking.API.Models.Appsettings;

namespace MovieBooking.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IOptions<MongoDbConfig> _mongoDbConfig;
        private readonly IMongoCollection<User> _users;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IOptions<MongoDbConfig> mongoDbConfig, IMongoClient mongoClient, ILogger<UserRepository> logger)
        {
            _mongoDbConfig = mongoDbConfig;
            _logger = logger;

            var database = mongoClient.GetDatabase(_mongoDbConfig.Value.DatabaseName);
            _users = database.GetCollection<User>(_mongoDbConfig.Value.UserCollectionName);
        }

        public async Task<bool> AddUser(User user)
        {
            try
            {
                _logger.LogInformation("Add user to role : user repository");
                await _users.InsertOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user");
                return false;
            }
        }

        public async Task DeleteUser(string id)
        {
            try
            {
                await _users.DeleteOneAsync(user => user.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user");
                throw; 
            }
        }

        public async Task<User> GetUser(string id)
        {
            try
            {
                _logger.LogInformation("Find user by id: user repository");
                var users = await _users.FindAsync(user => user.Id == id);
                return await users.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user by id");
                throw;
            }
        }

        public async Task<User> GetUser(string loginId, string email)
        {
            try
            {
                _logger.LogInformation("Get user by loginId and email : user repository");
                var users = await _users.FindAsync(user => user.LoginId == loginId || user.Email == email);
                return await users.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user by loginId and email");
                throw; 
            }
        }

        public async Task<User> GetUserByLoginIdPassword(string loginId, string password)
        {
            try
            {
                _logger.LogInformation("GetUserByLoginIdPassword: user repository");
                var users = await _users.FindAsync(user => user.LoginId == loginId && user.Password == password);
                return await users.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user by loginId and password");
                throw; 
            }
        }

        public async Task<User> GetUserByLoginId(string loginId)
        {
            try
            {
                _logger.LogInformation("GetUserByLoginId: user repository");
                var users = await _users.FindAsync(user => user.LoginId.Equals(loginId));
                return await users.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user by loginId");
                throw; 
            }
        }

        public async Task<List<User>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Get all users: user repository");
                return await _users.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all users");
                throw; 
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                _logger.LogInformation("Update user: user repository");
                var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user");
                return false;
            }
        }
    }
}
