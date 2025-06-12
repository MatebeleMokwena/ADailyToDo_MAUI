namespace MyDailyToDo;

public partial class Your_Proggress_Record : ContentPage
{
	public Your_Proggress_Record()
	{
		InitializeComponent();
		if (App.LDIR_VM == null)
			App.LDIR_VM = new TheToDo.Lets_Do_It_Right();
		this.BindingContext = App.LDIR_VM;
	}
}