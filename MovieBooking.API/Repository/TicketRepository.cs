using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.Appsettings;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _ticketCollection;
        private readonly IMongoCollection<MovieStatus> _movieCollection;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(IOptions<MongoDbConfig> movieTicketDatabaseSettings, ILogger<TicketRepository> logger)
        {
            var mongoClient = new MongoClient(movieTicketDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(movieTicketDatabaseSettings.Value.DatabaseName);
            _ticketCollection = mongoDatabase.GetCollection<Ticket>(movieTicketDatabaseSettings.Value.TicketCollectionName);
            _movieCollection = mongoDatabase.GetCollection<MovieStatus>(movieTicketDatabaseSettings.Value.MovieStatusCollectionName);
            _logger = logger;
        }

        public async Task BookMovieRepoAsync(Ticket ticket)
        {
            try
            {
                _logger.LogInformation("Add ticket to ticket collection : ticket repository");
                await _ticketCollection.InsertOneAsync(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding ticket");
                throw; 
            }
        }

        public async Task<bool> CheckIfMovieExists(string movieName)
        {
            try
            {
                _logger.LogInformation("Check if movie exists in movie collection : ticket repository");
                var result = await _movieCollection.Find(x => x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
                return (result != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if movie exists");
                throw; 
            }
        }

        public async Task<MovieStatus> GetMovieAgainstTheatreAsync(string theatreName, string movieName)
        {
            try
            {
                return await _movieCollection.Find(x => x.TheatreName.ToLower().Contains(theatreName.ToLower()) && x.MovieName.ToLower().Contains(movieName.ToLower())).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting movie against theatre");
                throw; 
            }
        }

        public async Task UpdateTicketCount(MovieStatus movie)
        {
            try
            {
                await _movieCollection.ReplaceOneAsync(x => x.Id == movie.Id, movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating ticket count");
                throw; 
            }
        }

        public async Task<List<Ticket>> GetTickets()
        {
            try
            {
                _logger.LogInformation("Get ticket list from movie collection : ticket repository");
                return await _ticketCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting tickets");
                throw; 
            }
        }
    }
}
