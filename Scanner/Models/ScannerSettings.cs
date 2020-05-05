using Newtonsoft.Json;
using Ninject;
using Scanner.Models.Interfaces;
using Scanner.Models.Iterfaces;
using SQLite;
using System.Collections.Generic;
using ZXing;
using ZXing.Mobile;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию об настройках сканера
    /// </summary>
    [Table("ScannerSettings")]
    public class ScannerSettings : ISerializableDatabaseItem, IDatabaseItem, IClone<ScannerSettings>
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
        public bool IsSoundShutterRelease { get; set; }
        /// <summary>
        /// http://developer.intersoftsolutions.com/display/crosslightapi/MobileBarcodeScanningOptions+Class
        /// https://forums.xamarin.com/discussion/161328/how-to-reference-object-in-sqlite
        /// </summary>
        [Ignore]
        public MobileBarcodeScanningOptions Options { get; private set; }
        public string OptionsJson { get; private set; }

        public void Serialize()
        {
            OptionsJson = JsonConvert.SerializeObject(Options, ISerializableDatabaseItem.JsonSettings);
        }

        public void Deserialize()
        {
            Options = JsonConvert.DeserializeObject<MobileBarcodeScanningOptions>(OptionsJson, ISerializableDatabaseItem.JsonSettings);
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
