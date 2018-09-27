using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SignalRChat.Contracts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SignalRChat.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly IChatService _chatService = DependencyService.Get<IChatService>();

        public LoginPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(RequestPermissions);
        }

        private async Task RequestPermissions()
        {
            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Microphone);
            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            App.UserName = UserName.Text;

            await _chatService.Connect(App.UserName);

            await Navigation.PushAsync(new ChatPage());
        }
    }
}