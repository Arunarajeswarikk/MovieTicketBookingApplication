﻿namespace MovieBooking.API.Models.DTO
{
    public class MovieDto
    {
        public string Name { get; set; } = string.Empty;
        public string TheatreName { get; set; } = string.Empty;
        public int NumberOfTickets { get; set; }
        public string TicketStatus { get; set; }
    }
}
