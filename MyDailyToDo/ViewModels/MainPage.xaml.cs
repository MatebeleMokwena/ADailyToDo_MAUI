using MyDailyToDo.ViewModels;

namespace MyDailyToDo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            if (App.LDIR_VM == null)
                App.LDIR_VM = new TheToDo.Lets_Do_It_Right();
            this.BindingContext = App.LDIR_VM; 
        }
    }

}
