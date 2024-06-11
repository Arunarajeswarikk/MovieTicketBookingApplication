
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IRepository
{
    public interface ITicketRepository
    {
        Task BookMovieRepoAsync(Ticket ticket);
        Task<bool> CheckIfMovieExists(string movieName);
        Task<MovieStatus> GetMovieAgainstTheatreAsync(string theatreName, string movieName);
        Task UpdateTicketCount(MovieStatus movie);
        Task<List<Ticket>> GetTickets();
    }
}
