using Plugin.Media;
using Plugin.Media.Abstractions;
using Scanner.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZXing;

namespace Scanner.Services
{
    /// <summary>
    /// Класс, расширяющий возможности сканера https://github.com/jamesmontemagno/MediaPlugin
    /// </summary>
    public class ScannerHelper : IScannerHelper
    {
        public ScannerHelper(List<BarcodeFormat> formats, IZxingImageHelper imageHelper)
        {
            this.imageHelper = imageHelper;
            var hints = new Dictionary<DecodeHintType, object>();
            hints.Add(DecodeHintType.POSSIBLE_FORMATS, formats);
            reader = new MultiFormatReader() { Hints = hints };
        }

        private readonly MultiFormatReader reader;
        private readonly IZxingImageHelper imageHelper;

        public Task<MediaFile> TakePhoto()
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                return CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Barcodes",
                    Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.png"
                });
            }

            return null;
        }

        public Task<MediaFile> GetFromGallery()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
                return CrossMedia.Current.PickPhotoAsync();

            return null;
        }

        public Task WriteAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Result> Scan(string path, CancellationToken token)
        {
            var binaryBitmapTask = await imageHelper.GetBinaryBitmap(path, token);

            if (binaryBitmapTask == null)
                return null;

            var result = reader.decode(binaryBitmapTask);
            return result;
        }
    }
}
