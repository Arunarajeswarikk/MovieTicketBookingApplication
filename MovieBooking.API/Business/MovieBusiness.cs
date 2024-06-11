using AutoMapper;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Interfaces.IRepository;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Business
{
    public class MovieBusiness : IMovieBusiness
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieBusiness> _logger;

        public MovieBusiness(IMovieRepository movieRepository, ILogger<MovieBusiness> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
        }
        public async Task<List<MovieDto>> GetMovies()
        {
            _logger.LogInformation("Get all movies : movie Business");

            List<MovieDto> moviesView;
            try
            {

                var movieList = new List<MovieDto>();
                var movies = await _movieRepository.GetMovies();
                if (movies.Count > 0)
                {
                    foreach (var movie in movies)
                    {
                        var movieResponse =
                            new MovieDto
                            {
                                Name = movie.MovieName,
                                TheatreName = movie.TheatreName,
                                NumberOfTickets = movie.NumberOfTickets,
                                TicketStatus = movie.TicketStatus
                            };
                        movieList.Add(movieResponse);
                    }
                    return movieList;
                }
                return null;


            }
            catch(Exception)
            {
                moviesView = new();
            }

            return moviesView;
        }

        public async Task<List<MovieDto>> SearchMovie(string movieName)
        {
            _logger.LogInformation("Get movie by name : movie Business");
         
                var moviesModel = await _movieRepository.SearchMovie(movieName);

                var movieList = new List<MovieDto>();

                if (moviesModel.Count > 0)
                {
                    foreach (var movie in moviesModel)
                    {
                        var movieResponse =
                            new MovieDto
                            {
                                Name = movie.MovieName,
                                TheatreName = movie.TheatreName,
                                NumberOfTickets = movie.NumberOfTickets,
                                TicketStatus = movie.TicketStatus
                            };
                        movieList.Add(movieResponse);
                    }
                    return movieList;
                }
            
           return null;
        }

        public async Task<MovieResponse> AddMovieAsync(MovieDto movie)
        {
            try
            {
                _logger.LogInformation("Add movie : admin service");

                var movieRequest = new MovieStatus
                {
                    MovieName = movie.Name,
                    TheatreName = movie.TheatreName,
                    NumberOfTickets = movie.NumberOfTickets,
                    TicketStatus = movie.TicketStatus
                };
                await _movieRepository.AddMovieRepoAsync(movieRequest);
                return new MovieResponse
                {
                    Message = "Movie added succeessfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Add movie failed");
                return new MovieResponse
                {
                    Message = ex.Message,
                    Success = false
                };
            }

        }

        public async Task<MovieResponse> DeleteMovieAsync(string moviename)
        {
            try
            {
                _logger.LogInformation("Delete movie : movie Business");


                var result = await _movieRepository.GetMovieRepoAsync(moviename);
                foreach (var movie in result)
                {
                    await _movieRepository.DeleteMovieRepoAsync(movie.Id);
                    return new MovieResponse
                    {
                        Message = "Deleted movie successfully",
                        Success = true
                    };
                }
                return new MovieResponse
                {
                    Message = "Movie deletion failed",
                    Success = false
                };
            }
            catch (Exception ex)
            {

                return new MovieResponse
                {
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<MovieStatus> UpdateMovieStatus(string moviename, string status)
        {
           var movieStatus= await _movieRepository.UpdateMovieStatus(moviename, status);

            if(movieStatus is null)
            {
                return null;
            }
            else
            {
                return movieStatus;
            }
        }
    }
}
