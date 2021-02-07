using System;

namespace Guppi.Application.Queries.Calendar
{
    public class EventDto
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Summary { get; set; }
    }
}
