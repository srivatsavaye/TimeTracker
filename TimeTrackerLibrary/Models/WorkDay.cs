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
        }
        public DayOfWeek DayNumber { get; set; }

        public DateTime Date { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public List<WorkItem> WorkItems { get; set; }
    }
}
