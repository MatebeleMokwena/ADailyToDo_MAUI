using MyDailyToDo.TheToDo;

namespace MyDailyToDo
{
    public partial class App : Application
    {
        public static Lets_Do_It_Right? LDIR_VM;
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}