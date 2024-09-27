
namespace KFC_Menu
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void GetStarted_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignInPage());
        }
    }

}
