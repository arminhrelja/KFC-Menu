using System.Collections.ObjectModel;
using KFC_Menu.Models;
using KFC_Menu.Services;

namespace KFC_Menu;

public partial class ProfilePage : ContentPage
{
	private User loggedInUser;
	public ObservableCollection<MenuItem> Favorites { get; set; }
	public ProfilePage()
	{
		InitializeComponent();
		LoadUserProfile();
	}

	private async void LoadUserProfile()
	{
		loggedInUser = await DatabaseServices.Instance.getLoggedInUser();
		if (loggedInUser != null)
		{
			nameLabel.Text = loggedInUser.Name;
			emailLabel.Text = loggedInUser.Email;
			BindingContext = loggedInUser;
		}
		else
		{
			await Navigation.PushAsync(new SignInPage());
		}
	}

	
	private async void MyFavoritesTapped(object sender, EventArgs e)
	{
        Navigation.PushAsync(new FavoritesPage());
    }

	private async void SignOutClicked(object sender, EventArgs e)
	{
		await DatabaseServices.Instance.signOutUser();
        Application.Current.MainPage = new NavigationPage(new SignInPage());
    }
}