using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Interfaces.IRepository
{
    public interface IMovieRepository
    {
        public Task<List<MovieStatus>> GetMovies();
        public Task<List<MovieStatus>> SearchMovie(string movieName);
        public Task AddMovieRepoAsync(MovieStatus movie);
        public Task DeleteMovieRepoAsync(string id);
        public Task<List<MovieStatus>> GetMovieRepoAsync(string movieName);
        public Task<MovieStatus> UpdateMovieStatus(string movieName, string movieStatus);

    }
}
