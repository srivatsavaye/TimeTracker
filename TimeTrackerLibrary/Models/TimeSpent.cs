using System;

namespace TimeTrackerLibrary.Models
{
    public class TimeSpent
    {
        private readonly IClock _clock;
        public TimeSpent(IClock clock)
        {
            _clock = clock ?? new Clock();
            IsActive = true;
            StartTime = DateTime.Now;
        }
        public bool IsActive { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long Duration => (long) ((IsActive || !EndTime.HasValue || !StartTime.HasValue) ? 0 : (EndTime.Value - StartTime.Value).TotalSeconds);
        public int DurationInMinutes => (int) Math.Ceiling(((double)Duration / 60));
        public long ActiveDuration => (long)(StartTime.HasValue ? ((EndTime ?? _clock.Now()) - StartTime.Value).TotalSeconds : 0);
        public int ActiveDurationInMinutes => (int) Math.Ceiling(((double)ActiveDuration / 60));
    }
}
