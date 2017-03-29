using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TimeTrackerLibrary.Models;

namespace TimeTrackerLibrary.Tests.Models
{
    [TestClass]
    public class TimeSpentTests
    {

        private Mock<IClock> _clock;

        [TestInitialize]
        public void Initialize()
        {
            _clock = new Mock<IClock>();
        }

        [TestMethod]
        public void Duration_should_be_0_for_active_TimeSpent()
        {
            var subject = new TimeSpent(_clock.Object);
            Assert.AreEqual(0, subject.Duration);
        }

        [TestMethod]
        public void Duration_should_return_0_if_start_or_end_is_null()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                IsActive = false
            };
            Assert.AreEqual(0, subject.Duration);
        }

        [TestMethod]
        public void Duration_should_return_seconds_for_inactive_TimeSpent()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                IsActive = false,
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = new DateTime(2017, 01, 01, 10, 01, 00)
            };
            Assert.AreEqual(60, subject.Duration);
        }

        [TestMethod]
        public void DurationInMinutes_should_return_minutes()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                IsActive = false,
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = new DateTime(2017, 01, 01, 10, 01, 00)
            };
            Assert.AreEqual(1, subject.DurationInMinutes);
        }

        [TestMethod]
        public void DurationInMinutes_should_return_minutes_rounded_up_to_next_minute()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                IsActive = false,
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = new DateTime(2017, 01, 01, 10, 01, 01)
            };
            Assert.AreEqual(2, subject.DurationInMinutes);
        }


        [TestMethod]
        public void ActiveDuration_should_be_0_if_start_is_null()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                StartTime = null
            };
            Assert.AreEqual(0, subject.ActiveDuration);
        }

        [TestMethod]
        public void ActiveDuration_should_return_seconds_when_start_and_end_dates_are_not_null()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = new DateTime(2017, 01, 01, 10, 01, 00)
            };
            Assert.AreEqual(60, subject.ActiveDuration);
        }


        [TestMethod]
        public void ActiveDuration_should_return_seconds_uses_current_time_when_enddate_is_null()
        {
            _clock.Setup(x => x.Now()).Returns(new DateTime(2017, 1, 1, 10, 00, 35));
            var subject = new TimeSpent(_clock.Object)
            {
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = null
            };
            Assert.AreEqual(35, subject.ActiveDuration);
        }

        [TestMethod]
        public void ActiveDurationInMinutes_should_return_minutes()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                IsActive = false,
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = new DateTime(2017, 01, 01, 10, 01, 00)
            };
            Assert.AreEqual(1, subject.ActiveDurationInMinutes);
        }

        [TestMethod]
        public void ActiveDurationInMinutes_should_return_minutes_rounded_up_to_next_minute()
        {
            var subject = new TimeSpent(_clock.Object)
            {
                IsActive = false,
                StartTime = new DateTime(2017, 01, 01, 10, 00, 00),
                EndTime = new DateTime(2017, 01, 01, 10, 01, 01)
            };
            Assert.AreEqual(2, subject.ActiveDurationInMinutes);
        }


    }
}
