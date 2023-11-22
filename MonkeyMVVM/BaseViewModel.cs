

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MonkeyMVVM;
public class BaseViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Monkey> _monkeys = new ObservableCollection<Monkey>();
    public ObservableCollection<Monkey> Monkeys
    {
        get
        {
            return _monkeys;
        }
    }
    HttpClient httpClient;
    public List<Monkey> monkeyList;
    bool isBusy;
    string title;

    public string Url
    {
        get; set;
    }

    public string _Title
    {
        get
        {
            return title;
        }
        set
        {
            if (title == value)
                return;
            title = value;
            OnPropertyChanged();
        }
    }

    private string animal;
    public string Animal
    {
        get
        {
            return animal;
        }
        set
        {
            if (animal == value)
                return;
            animal = value;
            OnPropertyChanged();
        }
    }

    public Command GetMonkeysCommand { get; }
    public Command GoToDetailsCommand { get; }

    public BaseViewModel(string ani) {
        httpClient = new HttpClient();
        monkeyList = new();
        GetMonkeysCommand = new Command(async () => await MakeCollection());
        GoToDetailsCommand = new Command<Monkey>(async (monkey) => await GoToDetails(monkey));
        Animal = ani;
        if (ani == "Dog")
            Url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/dogs.json";
        else if(ani == "Cat")
            Url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/cats.json";
        else if (ani == "Bear")
            Url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/bears.json";
        else
            Url = "https://www.montemagno.com/monkeys.json";
    }
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (isBusy == value)
                return;
            isBusy = value;
            OnPropertyChanged();
            // Also raise the IsNotBusy property changed
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    async Task GoToDetails(Monkey monkey) {
        if (monkey == null)
            return;

        await Shell.Current.GoToAsync(nameof(DetailsPage), true, new Dictionary<string, object>
    {
        {"monkey", monkey }
    });
    }

    public async Task MakeCollection() {
        if (IsBusy)
            return;

        try {
            IsBusy = true;

            await GetMonkeys();
            if (monkeyList.Count > 0) {
                _monkeys.Clear();
                foreach (var monkey in monkeyList) {
                    _monkeys.Add(monkey);
                }
            }
        }
        catch (Exception ex) {
            await Shell.Current.DisplayAlert("Error in loading monkeys", ex.Message, "OK");
           // await GetMonkeysCached();
        }
        finally {
            IsBusy = false;
        }
    }

    private async Task GetMonkeys() {
        if(monkeyList.Count > 0) 
            return;
        var response = await httpClient.GetAsync(Url);

        if (response.IsSuccessStatusCode) {
            string contents = await response.Content.ReadAsStringAsync();
            monkeyList = JsonSerializer.Deserialize<List<Monkey>>(contents);
        }
        return;
    }

    public string Title
    {
        get => title;
        set
        {
            if (title == value)
                return;
            title = value;
            OnPropertyChanged();
        }
    }

    public void AddMonkey(Monkey monkey) {
        monkeyList.Add(monkey);
    }

    public bool IsNotBusy => !IsBusy;
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

