using System;
using System.Collections.Generic;

namespace TimeTrackerLibrary.Models
{
    public class TimeSheet
    {
        public TimeSheet(string name)
        {
            if (name != null)
            {
                WeekEnding = name;
                var weekEndingString = name.Replace(Constants.TimeSheetPrefix, "");
                var year =int.Parse(weekEndingString.Substring(0, 4));
                var month = int.Parse(weekEndingString.Substring(4, 2));
                var day = int.Parse(weekEndingString.Substring(6, 2));
                var dateEnding = new DateTime(year, month, day);
                WorkDays = new Dictionary<int, WorkDay>
                {
                    [0] = new WorkDay(DayOfWeek.Sunday, dateEnding),
                    [1] = new WorkDay(DayOfWeek.Monday, dateEnding.AddDays(-6)),
                    [2] = new WorkDay(DayOfWeek.Tuesday, dateEnding.AddDays(-5)),
                    [3] = new WorkDay(DayOfWeek.Wednesday, dateEnding.AddDays(-4)),
                    [4] = new WorkDay(DayOfWeek.Thursday, dateEnding.AddDays(-3)),
                    [5] = new WorkDay(DayOfWeek.Friday, dateEnding.AddDays(-2)),
                    [6] = new WorkDay(DayOfWeek.Saturday, dateEnding.AddDays(-1))
                };
            }
        }
        public string WeekEnding { get; set; }
        public Dictionary<int,WorkDay> WorkDays { get; set; }
    }
}
