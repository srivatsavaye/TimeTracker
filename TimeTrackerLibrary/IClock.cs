using System;

namespace TimeTrackerLibrary
{
    public interface IClock
    {
        DateTime Now();
        DateTime UtcNow();
    }
}