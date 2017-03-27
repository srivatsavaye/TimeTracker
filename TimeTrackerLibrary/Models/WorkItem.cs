using System.Collections.Generic;
using System.Linq;

namespace TimeTrackerLibrary.Models
{
    public class WorkItem
    {
        public WorkItem(string name)
        {
            Name = name;
            TimeSpentList = new List<TimeSpent>();
        }
        public string Name { get; set; }

        public List<TimeSpent> TimeSpentList{ get; set; }

        public int TotalTime {
            get
            {
                return TimeSpentList
                    .Where(timeSpent => timeSpent.StartTime != null && timeSpent.EndTime != null)
                    .Aggregate(0, (total, timeSpent) => total + (timeSpent.EndTime - timeSpent.StartTime).Value.Seconds);
            }
        }
    }
}
