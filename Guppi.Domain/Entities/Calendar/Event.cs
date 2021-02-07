using System;

namespace Guppi.Domain.Entities.Calendar
{
    public class Event
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Summary { get; set; }
    }
}
