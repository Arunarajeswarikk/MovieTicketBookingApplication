using MongoDB.Bson.Serialization.Attributes;

namespace MovieBooking.API.Models.DTO
{
    public class TicketDto
    {
        [BsonElement("Movie")]
        public string Name { get; set; }

        [BsonElement("Theatre")]
        public string TheatreName { get; set; }

        [BsonElement("Tickets")]
        public int NumberOfTickets { get; set; }

        [BsonElement("SeatNumber")]
        public int[] SeatNumber { get; set; }
    }
}
