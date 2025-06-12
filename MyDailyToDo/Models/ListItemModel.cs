using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDailyToDo.Models
{
    public class ListItemModel
    {
        public string label { get; set; } = "";
        public Command<string> TickCommand { get; set; }
        public Command<string> CrossCommand { get; set; }

        public ListItemModel()
        {
            TickCommand = new Command<string>(TickCommandExecute);
            CrossCommand = new Command<string>(CrossCommandExecute);
        }

        public void TickCommandExecute(string item)
        {

        }

        public void CrossCommandExecute(string item)
        {

        }
    }
}
