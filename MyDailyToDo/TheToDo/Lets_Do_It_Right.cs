using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace MyDailyToDo.TheToDo
{
    public class Lets_Do_It_Right : ObservableObject
    {
        public ObservableCollection<ToDoItems> Items { get; set; } = new();
        private System.Timers.Timer endofday;
        readonly string savePath = Path.Combine(FileSystem.AppDataDirectory, "progress report.json");
        readonly string lastOpenedPath = Path.Combine(FileSystem.AppDataDirectory, "last_opened.txt");

        string? activity;
        public string? Activity
        {
            get => activity;
            set
            {
                SetProperty(ref activity, value);
                OnPropertyChanged(nameof(Activity));
            }
        }

        DateTime date = DateTime.Now;
        public DateTime Date
        {
            get => date;
            set
            {
                SetProperty(ref date, value);
                OnPropertyChanged(nameof(Date));
            }
        }

        //Increment on complete
        int? complete = 0;
        public int? Complete
        {
            get => complete;
            set
            {
                SetProperty(ref complete, value);
                OnPropertyChanged(nameof(Complete));
            }
        }

        //Increment on Delete
        int? deleted = 0;
        public int? Deleted
        {
            get => deleted;
            set
            {
                SetProperty(ref deleted, value);
                OnPropertyChanged(nameof(Deleted));
            }
        }

        //Increment on Failed
        int? failed = 0;
        public int? Failed
        {
            get => failed;
            set
            {
                SetProperty(ref failed, value);
                OnPropertyChanged(nameof(Failed));
            }
        }

        public Command Add { get; set; }
        //public Command DATE  { get; set; }

        public Lets_Do_It_Right()//////////////////////////
        {
            Add = new Command(AddExecute);
            //DATE = new Command(DisplayDate);
            LoadProgress();
            TheDeadLine();

            //LastOpened();
            StartTaskMonitoring();
        }

        public void AddExecute() /////////////////////////////////////////
        {
            /*
            The purpose it has is to take the item in the entry and input it into the list
             */

            if (!string.IsNullOrWhiteSpace(activity))
            {
                var items = new ToDoItems { label = activity, DeadLine = date };
                Items.Add(items);
                activity = "";

            }
        }

        public void TheDeadLine()///////////////////////////////////////
        {
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 0, 1, 0);
            scheduledTime.AddDays(1);
            double msUntilDeadline = ( now - scheduledTime).TotalMilliseconds;

            if (endofday != null)
            {
                endofday.Stop();
                endofday.Close();
            }

            endofday = new System.Timers.Timer(msUntilDeadline);
            //endofday.AutoReset = true; 
            endofday.Elapsed += OnDeadLine;
            endofday.Start();
        }

        public void SaveProgress()//////////////////////////////////////
        {
            var data = new ProgressData
            {
                Complete = Complete,
                Failed = Failed,
                Deleted = Deleted
            };

            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(savePath, json);
        }

        private void OnDeadLine(object? sender, System.Timers.ElapsedEventArgs e)////////////////////////////////////////
        {
            List<int> indexes = [];
            for (int i = 0; i < Items.Count; i++)
            {

                if (!Items[i].isComplete)
                {
                    Items[i].isFailed = true;
                }
                else
                {
                    Items[i].isFailed = false;
                }
                indexes.Add(i);


            }

            App.Current!.Dispatcher.Dispatch(() =>
            {

                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    int idx = indexes[i];
                    if (Items[idx].isFailed)
                    {
                        App.LDIR_VM!.Failed++;
                    }

                    Items.RemoveAt(idx);
                }
            });

            SaveProgress();
            TheDeadLine();
        }

        //public void LastOpened() ////////////////////////////////////////////
        //{
        //    //Will check when the app was last opened then compile the outstanding activities and do necessary incrementaion
        //    DateTime today = DateTime.Today;
        //    DateTime lastOpened = today;

        //    if (File.Exists(lastOpenedPath))
        //    {
        //        string content = File.ReadAllText(lastOpenedPath);
        //        if (DateTime.TryParse(content, out DateTime parsedDate))
        //            lastOpened = parsedDate;
        //    }

        //    int missedDays = (today - lastOpened).Days;

        //    if (missedDays > 0)
        //    {
        //        for (int d = 0; d < missedDays; d++)
        //        {
        //            if (Items.Count > 0)
        //            {
        //                foreach (var item in Items)
        //                {
        //                    if (!item.isComplete)
        //                    {
        //                        item.isFailed = true;
        //                        Failed++;
        //                    }
        //                }
        //                Items.Clear();
        //            }
        //            else
        //            {
        //                Failed++;
        //            }

        //        }
        //    }
        //    //File.WriteAllText(lastOpenedPath,today.ToString("yyyy-mm-dd"));
        //    SaveProgress();

        //}

        public void LoadProgress()/////////////////////////////////
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                var data = JsonConvert.DeserializeObject<ProgressData>(json);

                Complete = data?.Complete ?? 0;
                Failed = data?.Failed ?? 0;
                Deleted = data?.Deleted ?? 0;
            }
        }

        public void StartTaskMonitoring()
        {
            DateTime today = DateTime.Today;
            DateTime lastOpened = today;

            // Try loading last opened date
            if (File.Exists(lastOpenedPath))
            {
                string content = File.ReadAllText(lastOpenedPath);
                if (DateTime.TryParse(content, out DateTime parsedDate))
                    lastOpened = parsedDate;
            }

            int missedDays = (today - lastOpened).Days;

            // Process missed days
            if (missedDays > 0)
            {
                for (int d = 0; d < missedDays; d++)
                {
                    if (Items.Count > 0)
                    {
                        foreach (var item in Items)
                        {
                            if (!item.isComplete)
                            {
                                item.isFailed = true;
                                Failed++;
                            }
                        }
                        Items.Clear();
                    }
                    else
                    {
                        Failed++;
                    }
                }
            }

            // Update last opened timestamp
            File.WriteAllText(lastOpenedPath, today.ToString("yyyy-MM-dd"));
            SaveProgress();

            // Start repeating 10-second timer
            endofday = new System.Timers.Timer(30000); // 30 seconds/////////////
            endofday.Elapsed += OnIntervalElapsed;
            endofday.AutoReset = true;
            endofday.Start();
        }

        private void OnIntervalElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            App.Current!.Dispatcher.Dispatch(() =>
            {
                if (Items.Count > 0)
                {
                    foreach (var item in Items)
                    {
                        if (!item.isComplete)
                        {
                            item.isFailed = true;
                            Failed++;
                        }
                    }
                    Items.Clear();
                }
                else
                {
                    Failed++;
                }

                SaveProgress();
            });
        }
    } 

        public class ToDoItems ////////////////////////
    {

        public string? label { get; set; }
        public DateTime DeadLine { get; set; }
        public Command TickCommand { get; set; }
        //public Command CrossCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public bool isFailed { get; set; } = true;
        public bool isComplete { get; set; } = false;




        public ToDoItems()////////////////////////////
        {
            DeleteCommand = new Command(DeleteCommandExecute);
            TickCommand = new Command(TickCommandExecute);

            // Add method to check when last was the application opened 
            //SO that it increments to fail if nothing was do during a certain day/s

        }

        public void TickCommandExecute()/////////////////////
        {
            isComplete = true;
            isFailed = false;
            if (!isFailed)
            {
                App.LDIR_VM!.Complete++;
            }
            //  then it will increment by 1 on the Yourproggress report where the button is 

            // Will the function of scratching the text/item in the list and placing it at the bottom


        }

        public void DeleteCommandExecute() /////////////////////
        {
            App.LDIR_VM!.Deleted++;
            App.LDIR_VM!.Items.Remove(this);

        }

    }

    public class ProgressData///////////////////
    {
        public int? Complete { get; set; }
        public int? Failed { get; set; }
        public int? Deleted { get; set; }
    }
}
