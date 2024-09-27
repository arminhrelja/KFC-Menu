using KFC_Menu.Models;
using KFC_Menu.Services;

namespace KFC_Menu
{
    public partial class App : Application
    {
        public static ProfilePage ProfilePage {  get; private set; }
        public App()
        {
            InitializeComponent();
            ProfilePage = new ProfilePage();
            MainPage = new NavigationPage(new MainPage());
        }

        protected async void onStart()
        {
            await DatabaseServices.initializeDatabase();
        }

        public void loginSuccessful()
        {
            MainPage = new AppShell();
        }
    }
}
