using System;

namespace TimeTrackerLibrary.Models
{
    public class TimeSheet
    {
        public TimeSheet(string name)
        {
            if (name != null)
            {
                WeekEnding = name;
                var weekEndingString = name.Replace("WeekEnding_", "");
                var year =int.Parse(weekEndingString.Substring(0, 4));
                var month = int.Parse(weekEndingString.Substring(4, 2));
                var day = int.Parse(weekEndingString.Substring(6, 2));
                var dateEnding = new DateTime(year, month, day);
                Sunday = new WorkDay(DayOfWeek.Sunday, dateEnding);
                Saturday = new WorkDay(DayOfWeek.Saturday, dateEnding.AddDays(-1));
                Friday = new WorkDay(DayOfWeek.Friday, dateEnding.AddDays(-2));
                Thursday = new WorkDay(DayOfWeek.Thursday, dateEnding.AddDays(-3));
                Wednesday = new WorkDay(DayOfWeek.Wednesday, dateEnding.AddDays(-4));
                Tuesday = new WorkDay(DayOfWeek.Tuesday, dateEnding.AddDays(-5));
                Monday = new WorkDay(DayOfWeek.Monday, dateEnding.AddDays(-6));
                //Days = new List<WorkDay>();
            }
        }

        public string WeekEnding { get; set; }

        public WorkDay Monday { get; set; }
        public WorkDay Tuesday { get; set; }
        public WorkDay Wednesday { get; set; }
        public WorkDay Thursday { get; set; }
        public WorkDay Friday { get; set; }
        public WorkDay Saturday { get; set; }
        public WorkDay Sunday { get; set; }


        //public List<WorkDay> Days { get; set; }

    }
}
