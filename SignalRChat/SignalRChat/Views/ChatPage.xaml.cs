using Plugin.Toasts;
using SignalRChat.Contracts;
using SignalRChat.Features;
using SignalRChat.ViewModels;
using System;
using Xamarin.Forms;

namespace SignalRChat.Views
{
    public partial class ChatPage : ContentPage
    {
        private readonly IAudioRecorder _audioRecorder = DependencyService.Get<IAudioRecorder>();
        private readonly IChatService _chatService = DependencyService.Get<IChatService>();
        private readonly ISpeechService _speechService = DependencyService.Get<ISpeechService>();
        private readonly IToastNotificator _notificator = DependencyService.Get<IToastNotificator>();
        private readonly ChatViewModel _chatViewModel = new ChatViewModel();

        private bool isRecording;

        public ChatPage()
        {
            InitializeComponent();

            _chatService.OnAudioRecognized += ChatService_OnAudioRecognized;
            _chatService.OnMessageReceived += ChatService_OnMessageReceived;
            _chatService.OnNewUser += ChatService_OnNewUser;

            MessagesList.RowHeight = 100;
            MessagesList.ItemsSource = _chatViewModel.Messages;
        }

        private async void ChatService_OnNewUser(object sender, string name)
        {
            var options = new NotificationOptions
            {
                Title = $"{name} está conectado al chat",
                Description = $"{name} está conectado al chat"
            };

            await _notificator.Notify(options);
        }

        private void ChatService_OnMessageReceived(object sender, Message message)
        {
            _chatViewModel.Messages.Add(message);

            MessagesList.RowHeight = 100;
            MessagesList.IsEnabled = true;
        }

        private void ChatService_OnAudioRecognized(object sender, string text)
        {
            _chatService.Send(App.UserName, text);
        }

        private void SendButton_OnClicked(object sender, EventArgs e)
        {
            _chatService.Send(App.UserName, MessageInput.Text);

            MessageInput.Text = string.Empty;
        }

        private async void RecordButton_OnClicked(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                isRecording = true;
                RecordButton.Text = "Grabando...";
                _audioRecorder.StartRecording();
            }
            else
            {
                isRecording = false;
                RecordButton.Text = "Grabar";
                _audioRecorder.StopRecording();

                await _speechService.RecognizeSpeechAsync(Constants.AudioFilename);
            }
        }
    }
}