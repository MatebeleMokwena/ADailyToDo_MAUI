using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace MyDailyToDo.TheToDo
{
   public class Lets_Do_It_Right : ObservableObject
    {
        public ObservableCollection<ToDoItems> Items { get; set; } = new();
        private System.Timers.Timer endofday;
       

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

        public Command Add   { get; set; }
        public Command DATE  { get; set; }

        public Lets_Do_It_Right()
        {
            Add = new Command(AddExecute);
            DATE = new Command(DisplayDate);
            TheDeadLine();
        }

        public void AddExecute() 
        {
            /*
            The purpose it has is to take the item in the entry and input it into the list
             */

            if (!string.IsNullOrWhiteSpace(activity)) 
            {
                var items = new ToDoItems { label = activity , DeadLine = date  };
                Items.Add(items);
                activity = "";

            }
        }
        public void DisplayDate() 
        {
            /*
            Will allow you to choose a date where you want a certain task to be complete
             */
        }

        public void TheDeadLine()
        {
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 0, 1, 0);
            scheduledTime.AddDays(1);
            double msUntilDeadline = 10000; //(scheduledTime - now).TotalMilliseconds;

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

        private void OnDeadLine(object? sender, System.Timers.ElapsedEventArgs e)
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

            TheDeadLine();
        }
    }

    public class ToDoItems 
    {
        
         public string? label { get; set; }
         public DateTime DeadLine { get; set; }
         public Command TickCommand { get; set; }
         //public Command CrossCommand { get; set; }
         public Command DeleteCommand { get; set; }
        public bool isFailed { get; set; } = false;
        public bool isComplete { get; set; } = false;




        public ToDoItems()
        {
            DeleteCommand = new Command(DeleteCommandExecute);
            TickCommand = new Command(TickCommandExecute);
            
            // Add method to check when last was the application opened 
            //SO that it increments to fail if nothing was do during a certain day/s
            
        }

        public void TickCommandExecute()
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

        //public void CrossCommandExecute()
        //{
        //    //  then it will increment by 1 on the Yourproggress report where the button is 
        //    App.LDIR_VM!.Failed++;
        //    // Will the function of scratching the text/item in the list and placing it at the bottom


        //}
      
        public void DeleteCommandExecute() 
        {
            App.LDIR_VM!.Deleted++;
            App.LDIR_VM!.Items.Remove(this);

        }

    }
}
