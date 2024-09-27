using System.Collections.ObjectModel;
using KFC_Menu.Models;
using KFC_Menu.Services;
using SQLite;

namespace KFC_Menu;

public partial class MenuPage : ContentPage
{
	public ObservableCollection<MenuItem> Burgers { get; set; }
    public ObservableCollection<MenuItem> Wraps { get; set; }
    public ObservableCollection<MenuItem> Boxes { get; set; }
    public ObservableCollection<MenuItem> Drinks { get; set; }
    public ProfilePage profilePage;
    public MenuPage()
	{
		InitializeComponent();

        profilePage = new ProfilePage();

        LoadData();

        
    }

    private async void LoadData()
    {
        Burgers = new ObservableCollection<MenuItem>
        {
            new MenuItem {Name = "Classic Burger", Price = "$5.50", Image = "classic_burger.png", Description = "Uživajte u klasičnom burgeru s ukusnim mesom, svježom salatom i kremastim umakom.", Category = "Burgers"},
            new MenuItem {Name = "Double Cheeseburger", Price = "$6.50", Image = "double_cheeseburger.png", Description = "Dvostruki užitak s dvostrukim sirom i sočnim mesom u svakom zalogaju.", Category = "Burgers"},
            new MenuItem {Name = "Grander Burger", Price = "$7", Image = "grander_burger.png", Description = "Veličanstveni burger s većim komadom mesa za prave gurmane.", Category = "Burgers"},
            new MenuItem {Name = "Mini Cheeseburger", Price = "$3.50", Image = "mini_cheeseburger.png", Description = "Mali, ali moćan, s topljenim sirom i sočnim mesom.", Category = "Burgers"},
            new MenuItem {Name = "Mini Chickenburger", Price = "$3.75", Image = "mini_chickenburger.png", Description = "Ukusan mini burger s hrskavom piletinom i svježim povrćem.", Category = "Burgers"},
            new MenuItem {Name = "Zinger Burger", Price = "$6.00", Image = "zinger_burger.png", Description = "Pikantni Zinger Burger za ljubitelje začinjene hrane.", Category = "Burgers"},
        };

        Wraps = new ObservableCollection<MenuItem>
        {
            new MenuItem {Name = "i-Twister", Price = "$5.00", Image = "i_twister.png", Description = "Svježi wrap s piletinom, povrćem i ukusnim umakom.", Category = "Wraps"},
            new MenuItem {Name = "Twister", Price = "$5.50", Image = "twister.png", Description = "Klasični Twister wrap s piletinom, salatom i rajčicom.", Category = "Wraps"},
            new MenuItem {Name = "Qurrito", Price = "$6.0", Image = "qurrito.png", Description = "Savršena kombinacija burrita i quesadille s piletinom i sirom.", Category = "Wraps"},
            new MenuItem {Name = "Boxmaster", Price = "$6.50", Image = "boxmaster.png", Description = "Izdašan wrap s hrskavom piletinom, sirom i hrskavim krompirom.", Category = "Wraps"},
        };

        Boxes = new ObservableCollection<MenuItem>
        {
            new MenuItem {Name = "Classic Burger Box", Price = "$8.50", Image = "classic_burger_box.png", Description = "Klasična kutija s burgerom, krompirom i pićem.", Category = "Boxes"},
            new MenuItem {Name = "Double Cheeseburger Box", Price = "$9.50", Image = "double_cheeseburger_box.png", Description = "Kutija s dvostrukim cheeseburgerom, krompirom i pićem.", Category = "Boxes"},
            new MenuItem {Name = "Grander Box", Price = "$10.00", Image = "grander_box.png", Description = "Kutija s Grander Burgerom, krompirom i salatom.", Category = "Boxes"},
            new MenuItem {Name = "Tower Box", Price = "$10.50", Image = "tower_box.png", Description = "Impresivna kutija s velikim burgerom, krompirom i pićem.", Category = "Boxes"},
            new MenuItem {Name = "Twister Box", Price = "$9.00", Image = "twister_box.png", Description = "Kutija s Twister wrapom, krompirom i pićem.", Category = "Boxes"},
            new MenuItem {Name = "Zinger Box", Price = "$9.50", Image = "zinger_burger_box.png", Description = "Pikantna kutija s Zinger Burgerom, krompirom i pićem.", Category = "Boxes"},
        };

        Drinks = new ObservableCollection<MenuItem>
        {
            new MenuItem {Name = "Coca Cola", Price = "$1.50", Image = "coca_cola.png", Description = "Osvježavajuće gazirano piće s okusom Cole.", Category = "Drinks"},
            new MenuItem {Name = "Fanta", Price = "$1.50", Image = "fanta.png", Description = "Slatko gazirano piće s okusom naranče.", Category = "Drinks"},
            new MenuItem {Name = "Sprite", Price = "$1.50", Image = "sprite.png", Description = "Osvježavajuće gazirano piće s okusom limuna i limete.", Category = "Drinks"},
            new MenuItem {Name = "Water", Price = "$1.00", Image = "voda.png", Description = "Čista, osvježavajuća voda za hidrataciju.", Category = "Drinks"},
            new MenuItem {Name = "Cappuccino", Price = "$2.50", Image = "cappuccino.png", Description = "Bogat i kremast cappuccino za ljubitelje kave.", Category = "Drinks"},
        };

        BindingContext = this;
    }

    public void CategoryClicked(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var label = frame.Content as StackLayout;
        var category = (label.Children[1] as Label).Text;

        var yOffset = category switch
        {
            "Burgers" => 0,
            "Wraps" => BurgersSection.Height,
            "Boxes" => BurgersSection.Height + WrapsSection.Height,
            "Drinks" => BurgersSection.Height + WrapsSection.Height + BoxesSection.Height,
            _ => 0
        };

        ContentScrollView.ScrollToAsync(0, yOffset, true);
    }

    private async void ItemTapped(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        var item = frame.BindingContext as MenuItem;

        if (item != null)
        {
            await Navigation.PushAsync(new ItemDetailsPage(item));
        }
    }

    private async void addToFavoritesClicked(object  sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button.BindingContext as MenuItem;
        var loggedInUser = DatabaseServices.Instance.getLoggedInUser();

        if (loggedInUser != null && item != null)
        {
            await DatabaseServices.Instance.addFavorite(loggedInUser.Id, item);
            await DisplayAlert("Favorites", $"{item.Name} has been added to your favorites", "OK");
        }
    }
}

public class MenuItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }
    public string Image {  get; set; }
    public string Description {  get; set; }
    public string Category {  get; set; }
}