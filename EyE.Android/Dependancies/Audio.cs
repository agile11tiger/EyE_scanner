using Android.App;
using Android.Media;

namespace EyE.Droid.Dependancies
{
    /// <summary>
    /// Класс, позволяющий проигрывать мультимедию с помощью MediaPlayer
    /// </summary>
    public class Audio
    {
        public Audio(MediaPlayer player)
        {
            this.player = player;
        }

        private readonly MediaPlayer player;

        public void Play(string fileName)
        {
            try
            {
                var afd = Application.Context.Assets.OpenFd(fileName);
                player.Reset();
                player.SetDataSource(afd.FileDescriptor);
                player.Prepare();
                player.Start();
            }
            catch
            {

            }

            //try
            //{
            //    var player1 = MediaPlayer.Create(Application.Context, Resource.Drawable.camera_shutter_release);
            //    player1.Start();
            //}
            //catch { }

            //try
            //{
            //    var file = new File(pathName); //pathName хз какой путь указывать
            //    var fileStream = new FileInputStream(file);
            //    player.Reset();
            //    player.SetDataSource(fileStream.FD);
            //    player.Prepare();
            //    player.Start();
            //}
            //catch { }
        }
    }
}