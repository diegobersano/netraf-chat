using SignalRChat.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SignalRChat
{
    public partial class App : Application
    {
        public static string ConnectionId { get; set; }
        public static string UserName { get; set; }

        public App()
        {
            InitializeComponent();

            SetMainPage();
        }

        public static void SetMainPage()
        {
            Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}