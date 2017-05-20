using SignalRChat.Models;
using SignalRChat.Services;
using SignalRChat.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SignalRChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        private readonly IAudioRecorderService _audioRecorderService = DependencyService.Get<IAudioRecorderService>();
        private bool _isRecording;
        private readonly ChatViewModel chatViewModel = new ChatViewModel();

        public ChatPage()
        {
            InitializeComponent();

            App.ChatServices.OnAudioRecognized += ChatServicesOnOnAudioRecognized;
            App.ChatServices.OnMessageReceived += ChatServicesOnOnMessageReceived;

            this.MessagesList.RowHeight = 100;
            this.MessagesList.ItemsSource = chatViewModel.Messages;
        }

        private void ChatServicesOnOnMessageReceived(object sender, Message message)
        {
            chatViewModel.Messages.Add(message);

            this.MessagesList.RowHeight = 100;
            this.MessagesList.IsEnabled = true;
        }

        private void ChatServicesOnOnAudioRecognized(object sender, string text)
        {
            App.ChatServices.Send(App.UserName, text);
        }

        private void SendButton_OnClicked(object sender, EventArgs e)
        {
            App.ChatServices.Send(App.UserName, this.MessageInput.Text);

            this.MessageInput.Text = string.Empty;
        }

        private async void RecordButton_OnClicked(object sender, EventArgs e)
        {
            if (!_isRecording)
            {
                this._isRecording = true;
                this.RecordButton.Text = "Grabando...";
                this._audioRecorderService.StartRecording();
            }
            else
            {
                this._isRecording = false;
                this.RecordButton.Text = "Grabar";
                this._audioRecorderService.StopRecording();

                var service = new BingSpeechService();

                await service.RecognizeSpeechAsync(Constants.AudioFilename);
            }
        }
    }
}