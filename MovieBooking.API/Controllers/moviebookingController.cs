using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBooking.API.Interfaces.IBusiness;
using MovieBooking.API.Models.DTO;

namespace MovieBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class moviebookingController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IMovieBusiness _movieBusiness;
        private readonly ITicketBusiness _ticketBusiness;
        private readonly ILogger<moviebookingController> _logger;

        public moviebookingController(IUserBusiness userBusiness, IMovieBusiness movieBusiness, ITicketBusiness ticketBusiness, ILogger<moviebookingController> logger)
        {
            _userBusiness = userBusiness;
            _movieBusiness = movieBusiness;
            _ticketBusiness = ticketBusiness;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto user)
        {
            try
            {
                _logger.LogInformation("Register user : MovieBooking Controller");
                _logger.LogDebug($"RegisterRequest : {user}");

                var userId = await _userBusiness.AddUser(user);

                if (!string.IsNullOrEmpty(userId))
                {
                    return Ok(new
                    {
                        StatusCode = 201,
                        Message = "Registered successfully"
                    });
                }
                else
                {
                    return BadRequest("User already exists");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering user");
            }
        }

        [HttpGet("login")]
        public async Task<ActionResult<string>> Login(string loginId, string password)
        {
            try
            {
                _logger.LogInformation("Login user : MovieBooking Controller");
                _logger.LogDebug($"LoginId : {loginId}, Password: {password}");

                var token1 = await _userBusiness.GetUserToken(loginId, password);

                if (!string.IsNullOrEmpty(token1))
                {
                    return Ok(new
                    {
                        token = token1
                    });
                }
                else
                {
                    return BadRequest("Incorrect LoginId or Password");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while logging in user");
            }
        }

        [HttpGet("{username}/forgot")]
        public async Task<ActionResult<string>> Forgot(string loginId, string newPassword)
        {
            try
            {
                _logger.LogInformation("Reset password : MovieBooking Controller");
                _logger.LogDebug($"LoginId : {loginId}, Reset password : {newPassword}");

                var passwordChangedStatus = await _userBusiness.ChangePassword(loginId, newPassword);

                if (!string.IsNullOrEmpty(passwordChangedStatus))
                {
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Password changed Successfully"
                    });
                }
                return BadRequest(passwordChangedStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting password");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while resetting password");
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> ViewAllMovies()
        {
            try
            {
                _logger.LogInformation("Get all movies from MovieBooking Controller");

                var movies = await _movieBusiness.GetMovies();

                if (movies is not null && movies.Count > 0)
                {
                    return Ok(movies);
                }
                return NotFound("No Movies found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving movies");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving movies");
            }
        }

        [HttpGet("movies/search/moviename")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> SearchMovie(string movieName)
        {
            try
            {
                _logger.LogInformation("Get movie by name from MovieBooking Controller");
                _logger.LogDebug($"moviename: {movieName}");

                var movies = await _movieBusiness.SearchMovie(movieName);

                if (movies is not null && movies.Count > 0)
                {
                    return Ok(movies);
                }
                return NotFound("Movie doesn't exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching for movie");
            }
        }

        [HttpPost("tickets")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> BookTicket(string moviename, TicketBookRequest ticket)
        {
            try
            {
                _logger.LogInformation("Book ticket from MovieBooking Controller");
                _logger.LogDebug($"moviename: {moviename}, ticket: {ticket}");

                var result = await _ticketBusiness.BookMovieAsync(moviename, ticket);

                if (result.Success)
                {
                    return Ok(ticket);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while booking ticket");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while booking ticket");
            }
        }

        [HttpPost]
        [Route("{moviename}/add")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddMovie(MovieDto movie)
        {
            try
            {
                _logger.LogInformation("Add movie : admin controller");
                _logger.LogDebug($"movie: {movie}");

                var result = await _movieBusiness.AddMovieAsync(movie);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding movie");
            }
        }

        [HttpDelete]
        [Route("{moviename}/delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMovie(string moviename)
        {
            try
            {
                _logger.LogInformation("Delete movie from MovieBooking Controller");
                _logger.LogDebug($"moviename: {moviename}");

                var result = await _movieBusiness.DeleteMovieAsync(moviename);
                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting movie");
            }
        }

        [HttpPut]
        [Route("{moviename}/update/{ticket}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTicketStatus(string moviename, string status)
        {
            try
            {
                _logger.LogInformation("Update ticket status : MovieBooking controller");
                _logger.LogDebug($"moviename: {moviename},MoviestatusUpdate: {status}");

                var result = await _movieBusiness.UpdateMovieStatus(moviename, status);
                if (result is not null)
                {
                    return Ok(result);
                }

                return NotFound("Entered movie is not valid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating ticket status");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating ticket status");
            }
        }

        [HttpGet("tickets")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ViewAllTickets()
        {
            try
            {
                _logger.LogInformation("Get all tickets from MovieBooking Controller");

                var tickets = await _ticketBusiness.GetTickets();

                if (tickets is not null && tickets.Count > 0)
                {
                    return Ok(tickets);
                }
                return NotFound("No Tickets found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tickets");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tickets");
            }
        }
    }
}
