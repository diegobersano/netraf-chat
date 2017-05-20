using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SignalRChat.Views
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async Task Button_OnClicked(object sender, EventArgs e)
        {
            App.UserName = this.UserName.Text;

            await App.ChatServices.Connect(App.UserName);

            await Navigation.PushAsync(new ChatPage());
        }
    }
}