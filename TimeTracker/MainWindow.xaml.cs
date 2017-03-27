using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using TimeTrackerLibrary.Models;

namespace TimeTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private TimeSheet _timeSheet;
        private string path = "Timesheets";
        private DayOfWeek _today;
        public MainWindow()
        {
            InitializeComponent();
            LoadTimeSheet();
            LoadGrid();
            //_timeSheet.Days.Select(d => d.DayNumber)
        }

        private void LoadGrid()
        {
            ListView.ItemsSource = GetGridItems();
        }

        private IEnumerable<GridItem> GetGridItems()
        {
            var workItems = new List<string>();
            workItems.AddRange(_timeSheet.Sunday.WorkItems.Select(w => w.Name));
            workItems.AddRange(_timeSheet.Saturday.WorkItems.Select(w => w.Name));
            workItems.AddRange(_timeSheet.Friday.WorkItems.Select(w => w.Name));
            workItems.AddRange(_timeSheet.Thursday.WorkItems.Select(w => w.Name));
            workItems.AddRange(_timeSheet.Wednesday.WorkItems.Select(w => w.Name));
            workItems.AddRange(_timeSheet.Tuesday.WorkItems.Select(w => w.Name));
            workItems.AddRange(_timeSheet.Monday.WorkItems.Select(w => w.Name));
            var distinctWorkItems = workItems.Distinct().ToList();
            //var gridData = new TimeSheetGridView();

            var gridItems = new List<GridItem>();

            foreach (var distinctWorkItem in distinctWorkItems)
            {
                var gridItem = new GridItem();
                var total = 0.0;
                var item = _timeSheet.Sunday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Sunday = item.TotalTime;
                    total += item.TotalTime;
                }

                item = _timeSheet.Saturday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Saturday = item.TotalTime;
                    total += item.TotalTime;
                }

                item = _timeSheet.Friday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Friday = item.TotalTime;
                    total += item.TotalTime;
                }

                item = _timeSheet.Thursday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Thursday = item.TotalTime;
                    total += item.TotalTime;
                }

                item = _timeSheet.Wednesday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Wednesday = item.TotalTime;
                    total += item.TotalTime;
                }

                item = _timeSheet.Tuesday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Tuesday = item.TotalTime;
                    total += item.TotalTime;
                }

                item = _timeSheet.Monday.WorkItems.FirstOrDefault(i => i.Name == distinctWorkItem);
                if (item != null)
                {
                    gridItem.WorkItem = distinctWorkItem;
                    gridItem.Monday = item.TotalTime;
                    total += item.TotalTime;
                }

                gridItem.Total = total;
                gridItems.Add(gridItem);
            }

            var totalGridItem = new GridItem
            {
                WorkItem = "Total",
                Sunday = _timeSheet.Sunday.WorkItems.Select(w => w.TotalTime).Sum(),
                Saturday = _timeSheet.Saturday.WorkItems.Select(w => w.TotalTime).Sum(),
                Friday = _timeSheet.Friday.WorkItems.Select(w => w.TotalTime).Sum(),
                Thursday = _timeSheet.Thursday.WorkItems.Select(w => w.TotalTime).Sum(),
                Wednesday = _timeSheet.Wednesday.WorkItems.Select(w => w.TotalTime).Sum(),
                Tuesday = _timeSheet.Tuesday.WorkItems.Select(w => w.TotalTime).Sum(),
                Monday = _timeSheet.Monday.WorkItems.Select(w => w.TotalTime).Sum()
            };

            gridItems.Add(totalGridItem);

            return gridItems;
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
                $"WeekEnding_{endDay.Year}{(endDay.Month < 10 ? $"0{endDay.Month}" : endDay.Month.ToString())}{(endDay.Day.ToString().Length == 1 ? $"0{endDay.Day}" : endDay.Day.ToString())}";

            lblTimesheetName.Content = timeSheetName;
            _today = now.DayOfWeek;
            _timeSheet = ReadFile(timeSheetName);
            if (_timeSheet == null)
            {
                _timeSheet = new TimeSheet(timeSheetName);
                //_timeSheet.Days.Add(new WorkDay(today));
            }
        }

        private void Save()
        {
            var text = JsonConvert.SerializeObject(_timeSheet);
            File.WriteAllText(path + @"\" +_timeSheet.WeekEnding, text);
            LoadGrid();
        }

        private TimeSheet ReadFile(string name)
        {
            if (File.Exists(path + @"\" + name))
            {
                var text = File.ReadAllText(path + @"\" + name);
                var timeSheet = JsonConvert.DeserializeObject<TimeSheet>(text);
                return timeSheet;
            }
            return null;
        }

        #region "Event Handlers"
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var workItemName = txtWorkItem.Text;
            if (!string.IsNullOrEmpty(workItemName))
            {
                //var workDay = _timeSheet.Days.FirstOrDefault(d => d.DayNumber == today);
                //if (workDay == null)
                //{
                //    workDay = new WorkDay(today);
                //    _timeSheet.Days.Add(workDay);
                //}

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
                workItem.TimeSpentList.Add(new TimeSpent { StartTime = DateTime.Now, IsActive = true });

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
            var workDay = _timeSheet.Sunday;
            switch (_today)
            {
                case DayOfWeek.Sunday:
                    workDay = _timeSheet.Sunday;
                    break;
                case DayOfWeek.Saturday:
                    workDay = _timeSheet.Saturday;
                    break;
                case DayOfWeek.Friday:
                    workDay = _timeSheet.Friday;
                    break;
                case DayOfWeek.Thursday:
                    workDay = _timeSheet.Thursday;
                    break;
                case DayOfWeek.Wednesday:
                    workDay = _timeSheet.Wednesday;
                    break;
                case DayOfWeek.Tuesday:
                    workDay = _timeSheet.Tuesday;
                    break;
                case DayOfWeek.Monday:
                    workDay = _timeSheet.Monday;
                    break;
            }
            return workDay;
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

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            var workDay = GetWorkDay();
            if (workDay != null)
            {
                workDay.CheckIn = DateTime.Now;
                btnLogIn.IsEnabled = false;
                btnLogOut.IsEnabled = true;
            }
            Save();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            var workDay = GetWorkDay();
            if (workDay != null)
            {
                workDay.CheckOut = DateTime.Now;
                btnLogIn.IsEnabled = true;
                btnLogOut.IsEnabled = false;
            }
            Save();
        }

        #endregion "Event Handlers"

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedWorkItem = ((GridItem) ((System.Windows.Controls.Primitives.Selector) e.Source).SelectedItem).WorkItem;
            if (!string.Equals(selectedWorkItem, "total", StringComparison.InvariantCultureIgnoreCase))
            {
                txtWorkItem.Text = selectedWorkItem;
            }
        }
    }

    public class GridItem
    {
        public string WorkItem { get; set; }
        public double Monday { get; set; }
        public double Tuesday { get; set; }
        public double Wednesday { get; set; }
        public double Thursday { get; set; }
        public double Friday { get; set; }
        public double Saturday { get; set; }
        public double Sunday { get; set; }
        public double Total { get; set; }
    }
}
