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

        private MultiFormatReader reader;
        private IZxingImageHelper imageHelper;
        //private BarcodeWriter<> writer;

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

        public async Task<Result> GetResult(string path, CancellationToken token)
        {
            var binaryBitmapTask = await imageHelper.GetBinaryBitmap(path, token);

            if (binaryBitmapTask == null)
                return null;

            var result = reader.decode(binaryBitmapTask);
            return result;
            //var binaryBitmap = await imageHelper.GetBinaryBitmapAsync(path);
            //var result = reader.decode(binaryBitmap);
            //return result;

            //Если в пакетах добавить ссылку на System.Drawing.Common и использовать это, 
            //то будет ошибка, что платформа не поддерживает вызов Image.FromFile(path)
            //var bitmap = (Bitmap)Image.FromFile(path);
            //var multiReader = new MultiFormatReader();
            //var rgb = getRgbBytes(bitmap);
            //var rgbLuminanceSource = new RGBLuminanceSource(rgb, bitmap.Width, bitmap.Height);
            //var binarizer = new HybridBinarizer(rgbLuminanceSource);
            //var binaryBitmap = new BinaryBitmap(binarizer);
            //var result = multiReader.decode(binaryBitmap);
            //return result;

            //Это тоже не работает, мб не поддерживается
            //var reader = new BarcodeReader();
            //var bytes = File.ReadAllBytes(path);
            //return reader.Decode(bytes);

            //byte[] fileData = null;
            //using (FileStream fs = File.OpenRead(path))
            //    using (BinaryReader binaryReader = new BinaryReader(fs))
            //        fileData = binaryReader.ReadBytes((int)fs.Length);

            //byte[] buffer = null;
            //using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            //{
            //    buffer = new byte[fs.Length];
            //    fs.Read(buffer, 0, (int)fs.Length);
            //}

            //var imageStream = photo.GetStream();
            //var br = new BinaryReader(imageStream);
            //var imageByte = br.ReadBytes((int)imageStream.Length);
        }
    }
}
