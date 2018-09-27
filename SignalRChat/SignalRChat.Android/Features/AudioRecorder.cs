using Android.Content;
using Android.Media;
using Java.IO;
using SignalRChat.Contracts;
using SignalRChat.Droid.Features;
using SignalRChat.Features;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Encoding = Android.Media.Encoding;

[assembly: Dependency(typeof(AudioRecorder))]
namespace SignalRChat.Droid.Features
{
    public class AudioRecorder : IAudioRecorder
    {
        private const int RecorderBpp = 16;
        private int _recorderSamplerate;
        private const ChannelIn RecorderChannels = ChannelIn.Stereo;
        private const Encoding RecorderAudioEncoding = Encoding.Pcm16bit;

        private AudioRecord _recorder;
        private int _bufferSize;
        private bool _isRecording;
        private CancellationTokenSource _token;

        public void StartRecording()
        {
            var context = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;
            var audioManager = (AudioManager)context.GetSystemService(Context.AudioService);
            _recorderSamplerate = int.Parse(audioManager.GetProperty(AudioManager.PropertyOutputSampleRate));

            _recorder?.Release();

            _bufferSize = AudioRecord.GetMinBufferSize(_recorderSamplerate, ChannelIn.Stereo, Encoding.Pcm16bit) * 3;
            _recorder = new AudioRecord(AudioSource.Mic, _recorderSamplerate, RecorderChannels, RecorderAudioEncoding, _bufferSize);
            _recorder.StartRecording();
            _isRecording = true;

            _token = new CancellationTokenSource();
            Task.Run(() => WriteAudioDataToFile(), _token.Token);
        }

        public void StopRecording()
        {
            if (_recorder != null)
            {
                _recorder.Stop();
                _isRecording = false;
                _token.Cancel();

                _recorder.Release();
                _recorder = null;
            }
            CopyWaveFile(GetTempFilename(), GetFilename());
        }

        private void WriteAudioDataToFile()
        {
            var data = new byte[_bufferSize];
            var filename = GetTempFilename();
            FileOutputStream outputStream = null;

            try
            {
                outputStream = new FileOutputStream(filename);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (outputStream != null)
            {
                while (_isRecording)
                {
                    _recorder.Read(data, 0, _bufferSize);
                    try
                    {
                        outputStream.Write(data);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                try
                {
                    outputStream.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void CopyWaveFile(string tempFile, string permanentFile)
        {
            long sampleRate = _recorderSamplerate;
            const int channels = 2;
            long byteRate = RecorderBpp * _recorderSamplerate * channels / 8;

            var data = new byte[_bufferSize];

            try
            {
                var inputStream = new FileInputStream(tempFile);
                var outputStream = new FileOutputStream(permanentFile);
                var totalAudioLength = inputStream.Channel.Size();
                var totalDataLength = totalAudioLength + 36;

                Debug.WriteLine("File size: " + totalDataLength);
                WriteWaveFileHeader(outputStream, totalAudioLength, totalDataLength, sampleRate, channels, byteRate);

                while (inputStream.Read(data) != -1)
                {
                    outputStream.Write(data);
                }
                inputStream.Close();
                outputStream.Close();
                DeleteTempFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void WriteWaveFileHeader(OutputStream outputStream, long audioLength, long dataLength, long sampleRate, int channels, long byteRate)
        {
            var header = new byte[44];

            header[0] = Convert.ToByte('R'); // RIFF/WAVE header
            header[1] = Convert.ToByte('I'); // (byte)'I';
            header[2] = Convert.ToByte('F');
            header[3] = Convert.ToByte('F');
            header[4] = (byte)(dataLength & 0xff);
            header[5] = (byte)((dataLength >> 8) & 0xff);
            header[6] = (byte)((dataLength >> 16) & 0xff);
            header[7] = (byte)((dataLength >> 24) & 0xff);
            header[8] = Convert.ToByte('W');
            header[9] = Convert.ToByte('A');
            header[10] = Convert.ToByte('V');
            header[11] = Convert.ToByte('E');
            header[12] = Convert.ToByte('f');// 'fmt ' chunk
            header[13] = Convert.ToByte('m');
            header[14] = Convert.ToByte('t');
            header[15] = (byte)' ';
            header[16] = 16; // 4 bytes: size of 'fmt ' chunk
            header[17] = 0;
            header[18] = 0;
            header[19] = 0;
            header[20] = 1; // format = 1
            header[21] = 0;
            header[22] = Convert.ToByte(channels);
            header[23] = 0;
            header[24] = (byte)(sampleRate & 0xff);
            header[25] = (byte)((sampleRate >> 8) & 0xff);
            header[26] = (byte)((sampleRate >> 16) & 0xff);
            header[27] = (byte)((sampleRate >> 24) & 0xff);
            header[28] = (byte)(byteRate & 0xff);
            header[29] = (byte)((byteRate >> 8) & 0xff);
            header[30] = (byte)((byteRate >> 16) & 0xff);
            header[31] = (byte)((byteRate >> 24) & 0xff);
            header[32] = 2 * 16 / 8; // block align
            header[33] = 0;
            header[34] = Convert.ToByte(RecorderBpp); // bits per sample
            header[35] = 0;
            header[36] = Convert.ToByte('d');
            header[37] = Convert.ToByte('a');
            header[38] = Convert.ToByte('t');
            header[39] = Convert.ToByte('a');
            header[40] = (byte)(audioLength & 0xff);
            header[41] = (byte)((audioLength >> 8) & 0xff);
            header[42] = (byte)((audioLength >> 16) & 0xff);
            header[43] = (byte)((audioLength >> 24) & 0xff);

            outputStream.Write(header, 0, 44);
        }

        private static string GetTempFilename()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(path, "temp.wav");
        }

        private static string GetFilename()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(path, Constants.AudioFilename);
        }

        private static void DeleteTempFile()
        {
            var file = new Java.IO.File(GetTempFilename());
            file.Delete();
        }
    }
}