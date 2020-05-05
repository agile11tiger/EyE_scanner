using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
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

            OnDisappearingCommand = new AsyncCommand(OnDisappearing);
            DefaultSettingsCommand = new AsyncCommand(MakeDefaultSettings);
            ApplySettingsCommand = new AsyncCommand(ApplySettings);
        }

        private ScannerSettings tempSettings;
        private ScannerSettings settings;

        #region Properties(OnPropertyChanged)
        public ScannerSettings Settings
        {
            get => settings;
            set
            {
                if (settings != value)
                {
                    settings = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Settings.Options));
                }
            }
        }

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
            //переводит из милисекунд(int) в секунды(double)
            get => tempSettings.Options.InitialDelayBeforeAnalyzingFrames / 1000d;
            set
            {
                //переводит из секунд(double) в милисекунды(int)
                var tempValue = (int)(value * 1000);
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
        public IAsyncCommand OnDisappearingCommand { get; }
        public IAsyncCommand DefaultSettingsCommand { get; }
        public IAsyncCommand ApplySettingsCommand { get; }
        #endregion

        private Task OnDisappearing()
        {
            tempSettings = Settings.Clone();
            return Task.FromResult(true);
        }

        private Task MakeDefaultSettings()
        {
            Settings = new ScannerSettings();
            tempSettings = new ScannerSettings();
            return GoToMainPageAsync();
        }

        private async Task ApplySettings()
        {
            Settings = tempSettings.Clone();
            await GoToMainPageAsync().ConfigureAwait(false);
        }

        private async Task GoToMainPageAsync()
        {
            await Navigation.PopToRootAsync().ConfigureAwait(false);
            await AsyncDatabase.AddOrReplaceItemAsync(Settings);
        }
    }
}
