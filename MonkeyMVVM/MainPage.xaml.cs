namespace MonkeyMVVM;

public partial class MainPage : ContentPage
{
	BaseViewModel monkeymodel;
	public MainPage()
	{
		InitializeComponent();
		
		string whichTab = Shell.Current.CurrentItem.CurrentItem.Title;
        monkeymodel = new BaseViewModel(whichTab);
		BindingContext = monkeymodel;

    }
}

