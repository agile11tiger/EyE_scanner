using System.Threading;
using System.Threading.Tasks;
using ZXing;

namespace Scanner.Services.Interfaces
{
    /// <summary>
    /// Интерфейс, помогающий работать с фото
    /// </summary>
    public interface IZxingImageHelper
    {
        int ProgressIndicator { get; set; }
        Task<BinaryBitmap> GetBinaryBitmap(string path, CancellationToken token);
    }
}
