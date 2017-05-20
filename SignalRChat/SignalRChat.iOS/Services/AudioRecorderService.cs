using AVFoundation;
using Foundation;
using SignalRChat.iOS.Services;
using SignalRChat.Services;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioRecorderService))]
namespace SignalRChat.iOS.Services
{
    public class AudioRecorderService : IAudioRecorderService
    {
        private AVAudioRecorder _recorder;
        private NSError _error;
        private NSUrl _url;
        private NSDictionary _settings;

        public void StartRecording()
        {
            var localStorage = PCLStorage.FileSystem.Current.LocalStorage.Path;
            var audioFilePath = localStorage + "/" + Constants.AudioFilename;
            Console.WriteLine("Audio file path: " + audioFilePath);

            try
            {
                File.Delete(audioFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (_recorder == null)
            {
                InitializeRecorder();
            }

            _recorder.Record();
        }

        public void StopRecording()
        {
            if (_recorder == null)
            {
                throw new Exception("Start recording first.");
            }

            _recorder.Stop();
        }

        private void InitializeRecorder()
        {
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
                return;
            }

            err = audioSession.SetActive(true);
            if (_error != null)
            {
                Console.WriteLine("audioSession: {0}", err);
                return;
            }

            var localStorage = PCLStorage.FileSystem.Current.LocalStorage.Path;
            var audioFilePath = localStorage + "/" + Constants.AudioFilename;
            Console.WriteLine("Audio file path: " + audioFilePath);

            //try
            //{
            //    File.Delete(audioFilePath);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            _url = NSUrl.FromFilename(audioFilePath);

            var values = new NSObject[]
            {
                NSNumber.FromFloat (8000.0f), // Sample Rate
                NSNumber.FromInt32 ((int)AudioToolbox.AudioFormatType.LinearPCM), // AVFormat
                NSNumber.FromInt32 (1), // Channels
                NSNumber.FromInt32 (16), // PCMBitDepth
                NSNumber.FromBoolean (false), // IsBigEndianKey
                NSNumber.FromBoolean (false) // IsFloatKey
            };

            var keys = new NSObject[]
            {
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVNumberOfChannelsKey,
                AVAudioSettings.AVLinearPCMBitDepthKey,
                AVAudioSettings.AVLinearPCMIsBigEndianKey,
                AVAudioSettings.AVLinearPCMIsFloatKey
            };

            _settings = NSDictionary.FromObjectsAndKeys(values, keys);
            _recorder = AVAudioRecorder.Create(_url, new AudioSettings(_settings), out _error);
            _recorder.PrepareToRecord();
        }
    }
}