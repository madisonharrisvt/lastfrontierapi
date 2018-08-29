using System;

namespace LastFrontierApi.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public bool IsActiveEvent { get; set; }
    }
}
