using System.Collections.ObjectModel;
using KFC_Menu.Models;
using KFC_Menu.Services;

namespace KFC_Menu;

public partial class FavoritesPage : ContentPage
{
	private User loggedInUser;
	public ObservableCollection<MenuItem> Favorites {  get; set; }
	public FavoritesPage()
	{
		InitializeComponent();
		LoadFavorites();
	}

	private async void BackButtonClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

	private async void FavoriteItemClicked(object sender, EventArgs e)
	{
		var menuItem = (sender as ImageButton).BindingContext as MenuItem;
		if (menuItem != null)
		{
			await Navigation.PushAsync(new ItemDetailsPage(menuItem));
		}
	}

    private async void LoadFavorites()
    {
        loggedInUser = await DatabaseServices.Instance.getLoggedInUser();

        if (loggedInUser == null)
        {
            await DisplayAlert("Error", "You need to be logged in to see your favorites", "OK");
        }

        var favorites = await DatabaseServices.Instance.getFavoritesForUser(loggedInUser.Id);
		
		Favorites = new ObservableCollection<MenuItem>(favorites);
        BindingContext = this;
        FavoritesCollectionView.ItemsSource = Favorites;
		
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
		LoadFavorites();
    }
}