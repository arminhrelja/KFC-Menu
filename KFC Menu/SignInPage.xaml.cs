using KFC_Menu.Models;
using KFC_Menu.Services;
namespace KFC_Menu;

public partial class SignInPage : ContentPage
{
	public SignInPage()
	{
		InitializeComponent();
        DatabaseServices.initializeDatabase();
	}

    private async void NextClicked(object sender, EventArgs e)
    {         
            var user = await DatabaseServices.getUser(emailEntry.Text, passwordEntry.Text);

            if (user != null)
            {
                await DatabaseServices.Instance.setLoggedInUser(user);
                DisplayAlert("Success", "Signed in successfully", "OK");
                Application.Current.MainPage = new NavigationPage(new MenuPage());
                ((App)Application.Current).loginSuccessful();
            }
            else
            {

                DisplayAlert("Error", "Invalid email or password", "OK");
            } 
    }

	private void SignUpTapped(object sender, EventArgs e)
	{
		Navigation.PushAsync(new SignUpPage());
	}
}