
namespace MonkeyMVVM;

[QueryProperty(nameof(Monkey), "monkey")]
public partial class DetailsPage : ContentPage
{
    string title;

    public string _Title { get => title; }
    Monkey monkey;
    public Monkey Monkey
    {
        get => monkey;
        set
        {
            monkey = value;
            OnPropertyChanged();
        }
    }
    public DetailsPage()
	{
		InitializeComponent();
        title = "blah blah";
        BindingContext = this;
    }

}