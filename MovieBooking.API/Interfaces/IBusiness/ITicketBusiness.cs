using MovieBooking.API.Models.DTO;

namespace MovieBooking.API.Interfaces.IBusiness
{
    public interface ITicketBusiness
    {
        Task<TicketBookingResponse> BookMovieAsync(string moviename, TicketBookRequest ticket);

        Task TicketCountAsync(string moviename, TicketBookRequest ticket);

        Task<List<TicketDto>> GetTickets();
    }
}
