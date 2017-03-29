namespace TimeTrackerLibrary
{
    public interface ITimeSheetRepository
    {
        void Write(string name, string text);
        string Read(string name);
    }
}