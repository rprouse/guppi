using System;

namespace Guppi.Domain.Entities
{
    public class CalendarEvent
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Summary { get; set; }
    }
}
