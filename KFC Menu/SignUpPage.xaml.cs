using KFC_Menu.Models;
using KFC_Menu.Services;

namespace KFC_Menu;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private void SignInTapped(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SignInPage());
    }

    private void SignUpClicked(object sender, EventArgs e)
    {
        var user = new User
        {
            Name = nameEntry.Text,
            Email = emailEntry.Text,
            Password = passwordEntry.Text,
        };

        DatabaseServices.addUser(user);
        DisplayAlert("Success","Account created successfully", "OK");
        Navigation.PushAsync(new SignInPage());

    }
    
}