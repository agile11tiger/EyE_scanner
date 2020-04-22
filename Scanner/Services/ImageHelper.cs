using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;

namespace Scanner.Services
{
    public class ImageHelper
    {
        public async Task<string> GetImagePathFromGalleryAsync(IMedia media)
        {
            if (media.IsPickPhotoSupported)
            {
                var photo = await media.PickPhotoAsync();

                if (photo != null)
                    return photo.Path;
            }

            return null;
        }
    }
}
