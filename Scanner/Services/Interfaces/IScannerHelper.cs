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
        Task<Result> GetResult(string path, CancellationToken token);
        Task WriteAsync();
    }
}
