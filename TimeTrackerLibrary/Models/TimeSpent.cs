using System;

namespace TimeTrackerLibrary.Models
{
    public class TimeSpent
    {
        public bool IsActive { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
