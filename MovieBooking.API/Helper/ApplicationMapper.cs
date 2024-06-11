using AutoMapper;
using MovieBooking.API.Models;
using MovieBooking.API.Models.DTO;
using MovieBooking.API.Models.Entities;

namespace MovieBooking.API.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<UserDto, User>().ReverseMap();

            CreateMap<MovieStatus, MovieDto>()
                .ForMember(dto => dto.Name, map => map.MapFrom((entity => entity.MovieName)))
                .ForMember(dto => dto.TheatreName, map => map.MapFrom((entity => entity.TheatreName)));
                //.ForMember(dto => dto.IsAvailable, map => map.MapFrom((entity => entity.TicketStatus)));
        }


    }
}
