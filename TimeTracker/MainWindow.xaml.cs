using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using TimeTrackerLibrary;
using TimeTrackerLibrary.Models;

namespace TimeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private TimeSheet _timeSheet;
        private DayOfWeek _today;
        private readonly ITimeSheetRepository _timeSheetRepository;
        private readonly IClock _clock;

        public MainWindow()
        {
            _timeSheetRepository = new TimeSheetRepository();
            _clock = new Clock();
            InitializeComponent();
            LoadTimeSheet();
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            LoadGrid();
            PopulateLogInTimes(GetWorkDay());
        }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            var workDay = GetWorkDay();
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                {
                    var screenOn = workDay.ScreenOns.FirstOrDefault(s => s.IsActive);
                    if (screenOn != null)
                    {
                        screenOn.EndTime = DateTime.Now;
                        screenOn.IsActive = false;
                    }
                }
                    break;
                case SessionSwitchReason.SessionUnlock:
                {
                    workDay.ScreenOns.Add(new TimeSpent(_clock));
                }
                    break;
            }
            PopulateLogInTimes(workDay);
            Save();
        }

        private void LoadGrid()
        {
            ListView.ItemsSource = GetGridItems();
        }

        private IEnumerable<GridItem> GetGridItems()
        {
            var workItems = new List<string>();
            workItems.AddRange(_timeSheet.WorkDays.SelectMany(w => w.Value.WorkItems.Select(r => r.Name)));
            var distinctWorkItems = workItems.Distinct().ToList();

            var gridItems = distinctWorkItems.Select(PopulateGridItem).ToList();
            gridItems.Add(PopulateTotalGridItem());

            return gridItems;
        }

        private GridItem PopulateGridItem(string distinctWorkItem)
        {
            var gridItem = new GridItem();
            var total = 0.0;

            for (var i = 0; i < 7; i++)
            {
                var item = _timeSheet.WorkDays[i].WorkItems.FirstOrDefault(v => v.Name == distinctWorkItem);
                gridItem.WorkItem = distinctWorkItem;
                gridItem.WorkDays[i] = item?.TotalTime ?? 0;
                total += item?.TotalTime ?? 0;
            }
            gridItem.Total = total;
            return gridItem;
        }

        private GridItem PopulateTotalGridItem()
        {
            var totalGridItem = new GridItem
            {
                WorkItem = "Total"
            };
            for (var i = 0; i < 7; i++)
            {
                totalGridItem.WorkDays[i] = _timeSheet.WorkDays[i].WorkItems.Select(w => w.TotalTime).Sum();
            }

            return totalGridItem;
        }

        private void LoadTimeSheet()
        {
            var now = DateTime.Now;
            var numberOfdaysToAdd = 0;
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    numberOfdaysToAdd = 0;
                    break;

                case DayOfWeek.Saturday:
                    numberOfdaysToAdd = 1;
                    break;

                case DayOfWeek.Friday:
                    numberOfdaysToAdd = 2;
                    break;

                case DayOfWeek.Thursday:
                    numberOfdaysToAdd = 3;
                    break;

                case DayOfWeek.Wednesday:
                    numberOfdaysToAdd = 4;
                    break;

                case DayOfWeek.Tuesday:
                    numberOfdaysToAdd = 5;
                    break;

                case DayOfWeek.Monday:
                    numberOfdaysToAdd = 6;
                    break;
            }

            var endDay = now.AddDays(numberOfdaysToAdd);

            var timeSheetName =
                $"{Constants.TimeSheetPrefix}{endDay.Year}{(endDay.Month < 10 ? $"0{endDay.Month}" : endDay.Month.ToString())}{(endDay.Day.ToString().Length == 1 ? $"0{endDay.Day}" : endDay.Day.ToString())}";

            lblTimesheetName.Content = timeSheetName;
            _today = now.DayOfWeek;
            _timeSheet = ReadFile(timeSheetName);
            if (_timeSheet == null)
            {
                _timeSheet = new TimeSheet(timeSheetName);
            }
            var workDay = GetWorkDay();
            var inProgessWorkItem = workDay.WorkItems.FirstOrDefault(w => w.TimeSpentList.Any(t => !t.EndTime.HasValue));
            if (inProgessWorkItem != null)
            {
                lblCurrentItem.Content = inProgessWorkItem.Name;
                txtWorkItem.Text = inProgessWorkItem.Name;
            }
            
            if (workDay.ScreenOns.Count == 0 || workDay.ScreenOns.Count(s => s.EndTime.HasValue == false) == 0)
            {
                workDay.ScreenOns.Add(new TimeSpent(_clock));
            }
        }

        private void Save()
        {
            var text = JsonConvert.SerializeObject(_timeSheet);
            _timeSheetRepository.Write(_timeSheet.WeekEnding, text);
            LoadGrid();
        }

        private TimeSheet ReadFile(string name)
        {
            var text = _timeSheetRepository.Read(name);
            return string.IsNullOrEmpty(text) ? null : JsonConvert.DeserializeObject<TimeSheet>(text);
        }

        #region "Event Handlers"
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var workItemName = txtWorkItem.Text;
            StartLog(workItemName);
        }


        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            var currentItemName = lblCurrentItem.Content.ToString();
            if (!string.IsNullOrEmpty(currentItemName))
            {
                var workItem = GetWorkDay().WorkItems.FirstOrDefault(w => w.Name.Equals(currentItemName));
                if (workItem != null)
                {
                    var activeDuration = workItem.TimeSpentList.FirstOrDefault(t => t.IsActive);
                    if (activeDuration != null)
                    {
                        activeDuration.EndTime = DateTime.Now;
                        activeDuration.IsActive = false;
                        lblCurrentItem.Content = string.Empty;
                        Save();
                    }
                    else
                    {
                        MessageBox.Show(
                            $"Something went wrong, there is no active duration for workitem {currentItemName} in timesheet: {_timeSheet.WeekEnding} ");
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"Something went wrong, there is no workitem {currentItemName} in timesheet: {_timeSheet.WeekEnding} ");
                }
            }
            else
            {
                MessageBox.Show("There is no current work item");
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedWorkItem = ((GridItem)((System.Windows.Controls.Primitives.Selector)e.Source).SelectedItem).WorkItem;
            if (!string.Equals(selectedWorkItem, "total", StringComparison.InvariantCultureIgnoreCase))
            {
                txtWorkItem.Text = selectedWorkItem;
                StartLog(selectedWorkItem);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            PopulateLogInTimes(GetWorkDay());
            LoadGrid();
            Save();
        }

        //private void btnLogIn_Click(object sender, RoutedEventArgs e)
        //{
        //    var workDay = GetWorkDay();
        //    if (workDay != null)
        //    {
        //        workDay.CheckIn = DateTime.Now;
        //        btnLogIn.IsEnabled = false;
        //        btnLogOut.IsEnabled = true;
        //    }
        //    Save();
        //}

        //private void btnLogOut_Click(object sender, RoutedEventArgs e)
        //{
        //    var workDay = GetWorkDay();
        //    if (workDay != null)
        //    {
        //        workDay.CheckOut = DateTime.Now;
        //        btnLogIn.IsEnabled = true;
        //        btnLogOut.IsEnabled = false;
        //    }
        //    Save();
        //}

        #endregion "Event Handlers"


        private void PopulateLogInTimes(WorkDay workDay)
        {
            if(workDay.ScreenOns.Count == 0) return;
            lblTotalDuration.Content = $"Total Time Logged In Today: {workDay.ScreenOns.Select(r => r.ActiveDurationInMinutes).Sum()}";
            if(workDay.ScreenOns.Count(s => s.StartTime.HasValue) == 0) return;
            lblFirstLogin.Content = $"First LogIn Today: {workDay.ScreenOns.Where(s => s.StartTime.HasValue).Select(s => s.StartTime.Value).Min()}";
            if(workDay.ScreenOns.Count(s => s.EndTime.HasValue) == 0) return;
            lblLastLogin.Content = $"Last LogIn Today: {workDay.ScreenOns.Where(s => s.EndTime.HasValue).Select(s => s.EndTime.Value).Min()}";
        }

        private void StartLog(string workItemName)
        {
            if (!string.IsNullOrEmpty(workItemName))
            {
                var workDay = GetWorkDay();

                foreach (var item in workDay.WorkItems.Where(i => i.TimeSpentList.Any(j => j.IsActive)))
                {
                    foreach (var i in item.TimeSpentList.Where(l => l.IsActive))
                    {
                        i.IsActive = false;
                        i.EndTime = DateTime.Now;
                    }
                }
                var workItem = workDay.WorkItems.FirstOrDefault(t => t.Name.Equals(workItemName));
                if (workItem == null)
                {
                    workItem = new WorkItem(workItemName);
                    workDay.WorkItems.Add(workItem);
                }
                foreach (var timeSpent in workItem.TimeSpentList.Where(l => l.IsActive))
                {
                    timeSpent.EndTime = DateTime.Now;
                    timeSpent.IsActive = false;
                }
                workItem.TimeSpentList.Add(new TimeSpent(_clock) { StartTime = DateTime.Now, IsActive = true });

                lblCurrentItem.Content = workItemName;
                Save();
            }
            else
            {
                MessageBox.Show("Please enter a workItem");
            }
        }

        private WorkDay GetWorkDay()
        {
            var today = (int)Enum.Parse(typeof(DayOfWeek), Enum.GetName(typeof(DayOfWeek), _today));
            return _timeSheet.WorkDays[today];
        }
    }

    public class GridItem
    {
        public GridItem()
        {
            WorkDays = new Dictionary<int, double>();
        }
        public string WorkItem { get; set; }
        public Dictionary<int, double> WorkDays { get; set; }
        //public double Monday { get; set; }
        //public double Tuesday { get; set; }
        //public double Wednesday { get; set; }
        //public double Thursday { get; set; }
        //public double Friday { get; set; }
        //public double Saturday { get; set; }
        //public double Sunday { get; set; }
        public double Total { get; set; }
    }
}
