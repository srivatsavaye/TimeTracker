using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeTrackerLibrary.Models;

namespace TimeTrackerLibrary.Tests.Models
{
    [TestClass]
    public class WorkItemTests
    {
        [TestMethod]
        public void TotalTime_is_zero_when_no_items_TimeSpentList()
        {
            var subject = new WorkItem("test");

            Assert.AreEqual(0, subject.TotalTime);
        }

        [TestMethod]
        public void TotalTime_is_zero_when_Startime_or_endtime_are_null()
        {
            var subject = new WorkItem("test");
            subject.TimeSpentList.Add(new TimeSpent());

            Assert.AreEqual(0, subject.TotalTime);
        }

        [TestMethod]
        public void TotalTime_returns_value_for_non_empty_values()
        {
            var subject = new WorkItem("test");
            subject.TimeSpentList.Add(new TimeSpent{EndTime = new DateTime(2017,1,1,1,1,10), StartTime = new DateTime(2017, 1, 1, 1, 1, 1) });

            Assert.AreEqual(9, subject.TotalTime);
        }
    }
}
