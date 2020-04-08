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
    public class ScannerSettings : ISerializable
    {
        public ScannerSettings()
        {
            Options = new MobileBarcodeScanningOptions();
            Options.DelayBetweenContinuousScans = 2000;
            Options.PossibleFormats = App.Container.Get<List<BarcodeFormat>>();
        }

        [Unique]
        public string OptionsJson { get; set; }
        [Unique]
        public bool IsSoundShutterRelease { get; set; }
        /// <summary>
        /// http://developer.intersoftsolutions.com/display/crosslightapi/MobileBarcodeScanningOptions+Class
        /// https://forums.xamarin.com/discussion/161328/how-to-reference-object-in-sqlite
        /// </summary>
        [Ignore]
        public MobileBarcodeScanningOptions Options { get; set; }

        public void Serialize()
        {
            OptionsJson = JsonConvert.SerializeObject(Options, ISerializable.JsonSettings);
        }

        public void Deserialize()
        {
            Options = JsonConvert.DeserializeObject<MobileBarcodeScanningOptions>(OptionsJson, ISerializable.JsonSettings);
        }

        /// <summary>
        /// Глубокое клонирование, кроме свойства jsonSettings
        /// </summary>
        public async Task<ScannerSettings> CloneAsync()
        {
            return await Task.Run(() => Clone());
        }

        /// <summary>
        /// Глубокое клонирование, кроме свойства jsonSettings
        /// </summary>
        public ScannerSettings Clone()
        {
            Serialize();
            var settings = (ScannerSettings)MemberwiseClone();
            settings.Deserialize();

            return settings;
        }
    }
}
