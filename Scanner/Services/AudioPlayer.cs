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
        private ISimpleAudioPlayer player { get => CrossSimpleAudioPlayer.Current; }
        private const string pathToAudio = "Scanner.Resources.Audio.";

        public void Play(string fileName)
        {
            var stream = getStreamFromFile(fileName);
            player.Load(stream);
            player.Play();
        }

        private Stream getStreamFromFile(string fileName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"{pathToAudio}" + fileName);
            return stream;
        }
    }
}
