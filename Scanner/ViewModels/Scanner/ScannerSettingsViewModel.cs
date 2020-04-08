using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.Views.Scanner;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Scanner
{
    /// <summary>
    /// Класс, взаимодействующий с настройками сканера 
    /// </summary>
    public class ScannerSettingsViewModel : BaseViewModel
    {
        public ScannerSettingsViewModel(ScannerSettings settings)
        {
            Settings = settings.Clone();
            tempSettings = settings.Clone();

            OnDisappearingCommand = new AsyncCommand(onDisappearing);
            DefaultSettingsCommand = new AsyncCommand(makeDefaultSettings);
            ApplySettingsCommand = new AsyncCommand(applySettings);
        }

        private ScannerSettings tempSettings;
        public ScannerSettings Settings { get; set; }

        #region Properties(OnPropertyChanged)
        public bool IsSoundShutterRelease
        {
            get => tempSettings.IsSoundShutterRelease;
            set
            {
                if (tempSettings.IsSoundShutterRelease != value)
                {
                    tempSettings.IsSoundShutterRelease = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool? UseFrontCamera
        {
            get => tempSettings.Options.UseFrontCameraIfAvailable;
            set
            {
                if (tempSettings.Options.UseFrontCameraIfAvailable != value)
                {
                    tempSettings.Options.UseFrontCameraIfAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool? TryHarder
        {
            get => tempSettings.Options.TryHarder;
            set
            {
                if (tempSettings.Options.TryHarder != value)
                {
                    tempSettings.Options.TryHarder = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool? TryInverted
        {
            get => tempSettings.Options.TryInverted;
            set
            {
                if (tempSettings.Options.TryInverted != value)
                {
                    tempSettings.Options.TryInverted = value;
                    OnPropertyChanged();
                }
            }
        }

        public double InitialDelayBeforeAnalyzingFrames
        {
            get => tempSettings.Options.InitialDelayBeforeAnalyzingFrames / 1000d; //переводит из милисекунд(int) в секунды(double)
            set
            {
                var tempValue = (int)(value * 1000); //переводит из секунд(double) в милисекунды(int)
                if (tempSettings.Options.InitialDelayBeforeAnalyzingFrames != tempValue)
                {
                    tempSettings.Options.InitialDelayBeforeAnalyzingFrames = tempValue;
                    OnPropertyChanged();
                }
            }
        }

        public double DelayBetweenAnalyzingFrames
        {
            get => tempSettings.Options.DelayBetweenAnalyzingFrames / 1000d;
            set
            {
                var tempValue = (int)(value * 1000);
                if (tempSettings.Options.DelayBetweenAnalyzingFrames != tempValue)
                {
                    tempSettings.Options.DelayBetweenAnalyzingFrames = tempValue;
                    OnPropertyChanged();
                }
            }
        }

        public double DelayBetweenContinuousScans
        {
            get => tempSettings.Options.DelayBetweenContinuousScans / 1000d;
            set
            {
                var tempValue = (int)(value * 1000);
                if (tempSettings.Options.DelayBetweenContinuousScans != tempValue)
                {
                    tempSettings.Options.DelayBetweenContinuousScans = tempValue;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Commands
        public IAsyncCommand OnDisappearingCommand { get; set; }
        public IAsyncCommand DefaultSettingsCommand { get; set; }
        public IAsyncCommand ApplySettingsCommand { get; set; }
        #endregion

        private async Task onDisappearing()
        {
            tempSettings = await Settings.CloneAsync().ConfigureAwait(false);
        }

        private Task makeDefaultSettings()
        {
            Settings = new ScannerSettings();
            tempSettings = new ScannerSettings();
            return goToMainPageAsync();
        }

        private async Task applySettings()
        {
            Settings = await tempSettings.CloneAsync();
            await goToMainPageAsync().ConfigureAwait(false);
        }

        private async Task goToMainPageAsync()
        {
            //Нужно обязательно перевязывать, иначе настройки сканера не изменяться
            App.Container.Get<ScannerPage>().Dispose();
            App.Container.Rebind<ScannerPage>().ToSelf().InSingletonScope();
            await Navigation.PopToRootAsync().ConfigureAwait(false);
            await syncSettingsWithDatabase();
        }

        private Task syncSettingsWithDatabase()
        {
            return AsyncDatabase.AddItemAsync(Settings);
        }
    }
}
