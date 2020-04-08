using Plugin.Media;
using System.Threading.Tasks;

namespace Scanner.Services
{
    public class ImageHelper
    {
        public async Task<string> GetImagePathFromGalleryAsync()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                var photo = await CrossMedia.Current.PickPhotoAsync();

                if (photo != null)
                    return photo.Path;
            }

            return null;
        }
    }
}
