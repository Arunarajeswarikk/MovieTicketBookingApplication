using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IBusiness
{
    public interface IMovieBusiness
    {
        public Task<List<MovieDto>> GetMovies();
        public Task<List<MovieDto>> SearchMovie(string movieName);
        Task<MovieResponse> AddMovieAsync(MovieDto movie);
        Task<MovieResponse> DeleteMovieAsync(string moviename);
        Task<MovieStatus> UpdateMovieStatus(string moviename, string status);

    }
}
