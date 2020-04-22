using Android.Graphics;
using Scanner.Services.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace EyE.Droid.Dependancies
{
    /// <summary>
    /// Класс, позволяющий получить BinaryBitmap из проекта андроид
    /// </summary>
    public class ZxingImageHelper : IZxingImageHelper
    {
        public int ProgressIndicator { get; set; }

        public async Task<BinaryBitmap> GetBinaryBitmap(string path, CancellationToken token)
        {
            var bitmap = BitmapFactory.DecodeFile(path);
            var rgbBytes = await Task.Run(() => GetRgbBytesFaster(bitmap, token));
            ResetToReuse();

            if (rgbBytes == null)
                return null;

            var rgbLuminanceSource = new RGBLuminanceSource(rgbBytes, bitmap.Width, bitmap.Height);
            var binarizer = new HybridBinarizer(rgbLuminanceSource);
            var binaryBitmap = new BinaryBitmap(binarizer);
            return binaryBitmap;
        }

        #region slow getRgbBytes
        /// <summary>
        /// https://github.com/micjahn/ZXing.Net/blob/master/Source/lib/BitmapLuminanceSource.cs
        /// https://github.com/Redth/ZXing.Net.Mobile/issues/495
        /// https://stackoverflow.com/questions/42464321/read-barcode-from-gallery-image-in-xamarin-c-sharp
        /// </summary>
        private byte[] GetRgbBytes(Bitmap image, CancellationToken token)
        {
            var rgbBytes = new List<byte>();
            var percent10 = image.Height / 10;

            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    if (token.IsCancellationRequested)
                        return null;

                    if (y % percent10 == 0 && x == 0)
                    {
                        if (y == image.Height)
                            ProgressIndicator = 100;
                        else
                            ProgressIndicator = y / percent10 * 10;

                        Xamarin.Forms.MessagingCenter.Send<IZxingImageHelper>(this, "ProgressIndicatorChanged");
                    }

                    var color = new Color(image.GetPixel(x, y));
                    rgbBytes.AddRange(new[] { color.R, color.G, color.B });
                }
            }

            return rgbBytes.ToArray();
        }
        #endregion

        //https://itblogdsi.blog.fc2.com/blog-entry-262.html
        private byte[] GetRgbBytesFaster(Bitmap image, CancellationToken token)
        {
            var rgbBytes = new List<byte>();
            var square = image.Width * image.Height;
            var pixels = new int[square];
            var numberOfColorsInRGB = 3;
            var percent10 = square / 10 * numberOfColorsInRGB;
            image.GetPixels(pixels, 0, image.Width, 0, 0, image.Width, image.Height);

            foreach (var argb in pixels)
            {
                if (token.IsCancellationRequested)
                    return null;

                if (rgbBytes.Count % percent10 == 0)
                {
                    if (rgbBytes.Count == square)
                        ProgressIndicator = 100;
                    else
                        ProgressIndicator = rgbBytes.Count / percent10 * 10;

                    Xamarin.Forms.MessagingCenter.Send<IZxingImageHelper>(this, "ProgressIndicatorChanged");
                }

                var color = new Color(argb);
                rgbBytes.AddRange(new[] { color.R, color.G, color.B });
            }

            return rgbBytes.ToArray();
        }

        private void ResetToReuse()
        {
            ProgressIndicator = 0;
            Xamarin.Forms.MessagingCenter.Send<IZxingImageHelper>(this, "ProgressIndicatorChanged");
        }
    }
}