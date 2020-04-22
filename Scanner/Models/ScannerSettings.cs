using Newtonsoft.Json;
using Ninject;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerificationCheck.Core.Interfaces;
using ZXing;
using ZXing.Mobile;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию об настройках сканера
    /// </summary>
    [Table("ScannerSettings")]
    public class ScannerSettings : ISerializable, IDatabaseItem
    {
        public ScannerSettings()
        {
            Options = new MobileBarcodeScanningOptions
            {
                DelayBetweenContinuousScans = 2000,
                PossibleFormats = App.Container.Get<List<BarcodeFormat>>()
            };
        }

        [PrimaryKey, Unique]
        public int Id { get; set; }
        public string OptionsJson { get; set; }
        public bool IsSoundShutterRelease { get; set; }
        /// <summary>
        /// http://developer.intersoftsolutions.com/display/crosslightapi/MobileBarcodeScanningOptions+Class
        /// https://forums.xamarin.com/discussion/161328/how-to-reference-object-in-sqlite
        /// </summary>
        [Ignore]
        public MobileBarcodeScanningOptions Options { get; private set; }

        public void Serialize()
        {
            OptionsJson = JsonConvert.SerializeObject(Options, ISerializable.JsonSettings);
        }

        public void Deserialize()
        {
            Options = JsonConvert.DeserializeObject<MobileBarcodeScanningOptions>(OptionsJson, ISerializable.JsonSettings);
        }

        public async Task<ScannerSettings> CloneAsync()
        {
            return await Task.Run(() => Clone());
        }

        public ScannerSettings Clone()
        {
            Serialize();
            var settings = (ScannerSettings)MemberwiseClone();
            settings.Deserialize();

            return settings;
        }
    }
}
