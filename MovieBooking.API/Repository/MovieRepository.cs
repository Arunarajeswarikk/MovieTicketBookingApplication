using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.Appsettings;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IOptions<MongoDbConfig> _mongoDbConfig;
        private readonly IMongoCollection<MovieStatus> _movieCollection;
        private readonly ILogger<MovieRepository> _logger;

        public MovieRepository(IOptions<MongoDbConfig> mongoDbConfig, IMongoClient mongoClient, ILogger<MovieRepository> logger)
        {
            _mongoDbConfig = mongoDbConfig;

            var database = mongoClient.GetDatabase(_mongoDbConfig.Value.DatabaseName);
            _movieCollection = database.GetCollection<MovieStatus>(_mongoDbConfig.Value.MovieStatusCollectionName);
            _logger = logger;
        }

        

        public async Task<List<MovieStatus>> GetMovies()
        {
            try
            {
                _logger.LogInformation("Get movie list from movie collection : movie repository");
                return await _movieCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting movies");
                throw; 
            }
        }

        public async Task<List<MovieStatus>> SearchMovie(string movieName)
        {
            try
            {
                _logger.LogInformation("Get movie list by name from movie collection : movie repository");
                return await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for movies by name");
                throw; 
            }
        }

        public async Task AddMovieRepoAsync(MovieStatus movie)
        {
            try
            {
                _logger.LogInformation("Add movie to movie collection : admin repository");
                await _movieCollection.InsertOneAsync(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding movie");
                throw;
            }
        }

        public async Task DeleteMovieRepoAsync(string id)
        {
            try
            {
                _logger.LogInformation("Delete movie from movie collection : movie repository");
                await _movieCollection.DeleteOneAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting movie");
                throw; 
            }
        }

        public async Task<List<MovieStatus>> GetMovieRepoAsync(string movieName)
        {
            try
            {
                _logger.LogInformation("Get movie list from movie collection : movie repository");
                return await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting movies");
                throw; 
            }
        }

        public async Task<MovieStatus> UpdateMovieStatus(string movieName, string Status)
        {
            try
            {
                var result = await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
                result.TicketStatus = Status;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating movie status");
                throw; 
            }
        }
    }
}
