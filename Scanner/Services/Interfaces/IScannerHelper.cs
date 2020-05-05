using Plugin.Media.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using ZXing;

namespace Scanner.Services.Interfaces
{
    /// <summary>
    /// Интерфейс, расширяющий возможности сканера
    /// </summary>
    public interface IScannerHelper
    {
        Task<MediaFile> TakePhoto();
        Task<MediaFile> GetFromGallery();
        Task<Result> Scan(string path, CancellationToken token);
        Task WriteAsync();
    }
}
