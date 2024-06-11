using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Business
{
    public class TicketBusiness:ITicketBusiness
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<TicketBusiness> _logger;

        private Dictionary<string, int> seat = new Dictionary<string, int>();

        public TicketBusiness(ITicketRepository ticketRepository, ILogger<TicketBusiness> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }



        public async Task<TicketBookingResponse> BookMovieAsync(string moviename, TicketBookRequest ticket)
        {
            _logger.LogInformation("Book movie : ticket business");

            var movie = await _ticketRepository.CheckIfMovieExists(moviename);
            if (movie)
            {
                var theatre = await _ticketRepository.GetMovieAgainstTheatreAsync(ticket.TheatreName, moviename);
                if (theatre != null)
                {
                    if (ticket.NumberOfTickets == ticket.SeatNumber.Length)
                    {
                        if (CheckIfSeatExists(ticket))
                        {
                            var ticketInsert = new Ticket
                            {
                                UserName = ticket.UserName,
                                MovieName = moviename,
                                TheatreName = ticket.TheatreName,
                                NumberOfTickets = ticket.NumberOfTickets,
                                SeatNumber = ticket.SeatNumber
                            };
                            await _ticketRepository.BookMovieRepoAsync(ticketInsert);
                            await TicketCountAsync(moviename, ticket);
                            return new TicketBookingResponse
                            {
                                Message = "Ticket booked successfully",
                                Success = true,
                                UserName = ticket.UserName
                            };
                        }
                        else
                        {
                            return new TicketBookingResponse
                            {
                                Message = "Seat number is taken. Try another seat number",
                                Success = false
                            };
                        }
                    }
                    else
                    {
                        return new TicketBookingResponse
                        {
                            Message = "Number of seats is not same as number of tickets",
                            Success = false
                        };
                    }
                }
                else
                {
                    return new TicketBookingResponse
                    {
                        Message = "Theatre not found",
                        Success = false
                    };
                }
            }
            else
            {
                return new TicketBookingResponse
                {
                    Message = "Movie not found",
                    Success = false
                };
            }
        }

       public async Task TicketCountAsync(string moviename, TicketBookRequest ticket)
        {
          
            var movie = await _ticketRepository.GetMovieAgainstTheatreAsync(ticket.TheatreName, moviename);
            var ticketCount = ticket.NumberOfTickets;
            movie.NumberOfTickets -= ticketCount;
            if (movie.NumberOfTickets > 0)
            {
               var result= movie.TicketStatus = "BOOK ASAP";
                await _ticketRepository.UpdateTicketCount(movie);
               

            }
            else
            {
                var result=movie.TicketStatus = "SOLD OUT";
                await _ticketRepository.UpdateTicketCount(movie);
               

            }
            
        }

         public bool CheckIfSeatExists(TicketBookRequest ticket)
         {
             var theatre = ticket.TheatreName;

             foreach (var seatNo in ticket.SeatNumber)
             {
                 if (seat.ContainsKey(theatre) && seat.ContainsValue(seatNo))
                 {
                     return false;
                 }
                 else
                 {
                     seat.Add(theatre, seatNo);
                     return true;
                 }
             }
             return true;
         }


        public async Task<List<TicketDto>> GetTickets()
        {
            _logger.LogInformation("Get all tickets : movie Business");

            List<TicketDto> ticketsView;
            try
            {

                var ticketList = new List<TicketDto>();
                var tickets = await _ticketRepository.GetTickets();
                if (tickets.Count > 0)
                {
                    foreach (var m in tickets)
                    {
                        var ticketResponse =
                            new TicketDto
                            {
                                Name = m.MovieName,
                                TheatreName = m.TheatreName,
                                NumberOfTickets = m.NumberOfTickets,
                                SeatNumber = m.SeatNumber

                            };
                        ticketList.Add(ticketResponse);
                    }
                    return ticketList;
                   }
                return null;


            }
            catch (Exception)
            {
                ticketsView = new();
            }

            return ticketsView;
        }
    }
}