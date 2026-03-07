using System;

namespace Guppi.Core.Entities.Calendar
{
    public class Event
    {
        private string _summary;

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        
        public string Summary 
        { 
            get => _summary;
            set
            {
                // Strip out markdown tags from the summary that mess up my tables and links
                _summary = value.Replace("|", "").Replace("[", "").Replace("]", "").Trim();
            }
        }
        
        public string MeetingUrl { get; set; }

    }
}
