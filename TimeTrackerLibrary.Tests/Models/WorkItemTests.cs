using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TimeTrackerLibrary.Models;

namespace TimeTrackerLibrary.Tests.Models
{
    [TestClass]
    public class WorkItemTests
    {
        private Mock<IClock> _clock;

        [TestInitialize]
        public void Initialize()
        {
            _clock = new Mock<IClock>();
        }

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
            subject.TimeSpentList.Add(new TimeSpent(_clock.Object));

            Assert.AreEqual(0, subject.TotalTime);
        }

        [TestMethod]
        public void TotalTime_returns_value_for_non_empty_values()
        {
            var subject = new WorkItem("test");
            subject.TimeSpentList.Add(new TimeSpent(_clock.Object)
            {
                IsActive = false,
                EndTime = new DateTime(2017, 1, 1, 1, 10, 0),
                StartTime = new DateTime(2017, 1, 1, 1, 1, 0)
            });

            Assert.AreEqual(9, subject.TotalTime);
        }
    }
}
