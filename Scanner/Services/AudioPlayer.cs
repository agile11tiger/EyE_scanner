using Plugin.SimpleAudioPlayer;
using Scanner.Services.Interfaces;
using System.IO;
using System.Reflection;

namespace Scanner.Services
{
    /// <summary>
    /// Класс, позволяющий проигрывать мультимедию с помощью CrossSimpleAudioPlayer
    /// https://github.com/adrianstevens/Xamarin-Plugins/tree/master/SimpleAudioPlayer
    /// </summary>
    public class AudioPlayer : IPlayer
    {
        private ISimpleAudioPlayer Player { get => CrossSimpleAudioPlayer.Current; }

        public void Play(string fileName)
        {
            var stream = GetStreamFromFile(fileName);
            Player.Load(stream);
            Player.Play();
        }

        private Stream GetStreamFromFile(string fileName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"{Audio.PathToAudio}" + fileName);
            return stream;
        }
    }
}
