using System;
using System.Collections.Generic;

namespace TimeTrackerLibrary.Models
{
    public class WorkDay
    {
        public WorkDay(DayOfWeek dayNumber, DateTime date)
        {
            DayNumber = dayNumber;
            Date = date;
            WorkItems = new List<WorkItem>();
            ScreenOns = new List<TimeSpent>();
        }
        public DayOfWeek DayNumber { get; set; }

        public DateTime Date { get; set; }

        public List<WorkItem> WorkItems { get; set; }

        public List<TimeSpent> ScreenOns { get; set; }
    }
}
