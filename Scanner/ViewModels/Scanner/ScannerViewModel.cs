using Plugin.Media.Abstractions;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Services;
using Scanner.Services.Interfaces;
using Scanner.ViewModels.Scanner.QRCodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;

namespace Scanner.ViewModels.Scanner
{
    /// <summary>
    /// Класс, взаимодействующий со сканером
    /// </summary>
    public class ScannerViewModel : BaseViewModel
    {
        public ScannerViewModel(
            CashQRCodeViewModel cashQRCodeVM,
            ScannerSettingsViewModel scannerSettingsVM,
            ScannerHelper scannerHelper,
            IPlayer player)
            : base()
        {
            CashQRCodeVM = cashQRCodeVM;
            ScannerSettingsVM = scannerSettingsVM;
            this.scannerHelper = scannerHelper;
            this.player = player;
            cancellationTS = new CancellationTokenSource();
            SetSubscribe();

            RunAnimationCommand = new AsyncCommand(RunAnimation);
            BackCommand = new AsyncCommand(GoToBack);
            SwitchTorchCommand = new Command(SwitchTorch);
            TurnTorchCommand = new Command<bool>(TurnTorch);
            SetOutlineCodeCommand = new Command(SetOutlineCode);
            InfoCommand = new AsyncCommand(ShowInfo);
            ScanCommand = new AsyncCommand<string>(Scan);
            ProcessScanResultCommand = new AsyncCommand<Result>(ProcessScanResult);
            ScannerSwitchCommand = new Command<bool>(ScannerSwitch);
            CancelScanningPhotoCommand = new Command(CancelScanningPhoto);
        }

        private void SetSubscribe()
        {
            MessagingCenter.Subscribe<IZxingImageHelper>(
                this, // кто подписывается на сообщения
                "ProgressIndicatorChanged",   // название сообщения
                (sender) =>
                {
                    ProgressIndicator = sender.ProgressIndicator;
                });    // вызываемое действие
        }

        private readonly IScannerHelper scannerHelper;
        private readonly IPlayer player;
        private CancellationTokenSource cancellationTS;
        private bool isScanning;
        private bool isAnalyzing;
        private bool isTorchOn;
        private ImageSource torchImage;
        private Rectangle topLineCode;
        private Rectangle bottomLineCode;
        private Rectangle leftLineCode;
        private Rectangle rightLineCode;
        private double lineTranslationY;
        private bool isRunningIndicator;
        private int progressIndicator;
        public CashQRCodeViewModel CashQRCodeVM { get; }
        public ScannerSettingsViewModel ScannerSettingsVM { get; }

        #region Properties(OnPropertyChanged)
        public bool IsScanning
        {
            get => isScanning;
            set
            {
                if (isScanning != value)
                {
                    isScanning = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAnalyzing
        {
            get => isAnalyzing;
            set
            {
                if (isAnalyzing != value)
                {
                    isAnalyzing = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsTorchOn
        {
            get => isTorchOn;
            set
            {
                if (isTorchOn != value)
                {
                    isTorchOn = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource TorchImage
        {
            get => torchImage;
            set
            {
                if (torchImage != value)
                {
                    torchImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public Rectangle TopLineCode
        {
            get => topLineCode;
            set
            {
                if (topLineCode != value)
                {
                    topLineCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Rectangle BottomLineCode
        {
            get => bottomLineCode;
            set
            {
                if (bottomLineCode != value)
                {
                    bottomLineCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Rectangle LeftLineCode
        {
            get => leftLineCode;
            set
            {
                if (leftLineCode != value)
                {
                    leftLineCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Rectangle RightLineCode
        {
            get => rightLineCode;
            set
            {
                if (rightLineCode != value)
                {
                    rightLineCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public double LineTranslationY
        {
            get => lineTranslationY;
            set
            {
                if (lineTranslationY != value)
                {
                    lineTranslationY = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRunningIndicator
        {
            get => isRunningIndicator;
            set
            {
                if (isRunningIndicator != value)
                {
                    isRunningIndicator = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProgressIndicator
        {
            get => progressIndicator;
            set
            {
                if (progressIndicator != value)
                {
                    progressIndicator = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Commands
        public IAsyncCommand RunAnimationCommand { get; }
        public IAsyncCommand BackCommand { get; }
        public ICommand SwitchTorchCommand { get; }
        public ICommand TurnTorchCommand { get; }
        public ICommand SetOutlineCodeCommand { get; }
        public IAsyncCommand InfoCommand { get; }
        public AsyncCommand<string> ScanCommand { get; }
        public ICommand CancelScanningPhotoCommand { get; }
        public IAsyncCommand<Result> ProcessScanResultCommand { get; }
        public ICommand ScannerSwitchCommand { get; }
        #endregion

        /// <summary>
        /// https://docs.microsoft.com/ru-ru/xamarin/xamarin-forms/user-interface/animation/custom
        /// </summary>
        private Task RunAnimation()
        {
            return Device.InvokeOnMainThreadAsync(() =>
            {
                var animation = new Animation(v => LineTranslationY = v, 0, 240);
                animation.Commit(CurrentPage, "SimpleAnimation", 16, 2500, Easing.Linear, (v, c) => LineTranslationY = 240, () => true);
            });
        }

        private Task GoToBack()
        {
            return Navigation.PopAsync();
        }

        private void SwitchTorch()
        {
            IsTorchOn = !IsTorchOn;
            SetImageForTorch();
        }

        private void TurnTorch(bool isTurnOn)
        {
            if (IsTorchOn != isTurnOn)
            {
                IsTorchOn = isTurnOn;
                SetImageForTorch();
            }
        }

        private void SetImageForTorch()
        {
            if (IsTorchOn)
                TorchImage = ImageSource.FromResource(ImagePaths.TorchOn);
            else
                TorchImage = ImageSource.FromResource(ImagePaths.TorchOff);
        }

        private void SetOutlineCode()
        {
            TopLineCode = new Rectangle(100, 2, 50, 3);
            BottomLineCode = new Rectangle(100, 245, 50, 3);
            LeftLineCode = new Rectangle(2, 100, 3, 50);
            RightLineCode = new Rectangle(245, 100, 3, 50);
        }

        private Task ShowInfo()
        {
            return CurrentPage.DisplayAlert(
                    "Как использовать правильно сканирование из галлереи?",
                    "1.Выбрать фото в галлереи с кодом для сканера;\r\n" +
                    "2.Обрезать его используя встроенный редактор, оставив только код для сканера;\r\n" +
                    "3.Важно! Чем больше ширина и длина изображения, тем дольше будет оно обрабатываться.",
                    "Ок");
        }

        private Task ProcessScanResult(Result result)
        {
            return WrapUp("scanResult", result);
        }

        private Task Scan(string name)
        {
            return WrapUp(name, null);
        }

        private async Task WrapUp(string name, Result result = null)
        {
            await Device.InvokeOnMainThreadAsync(async() =>
            {
                //Чтобы пользователь не смог запустить команду до завершения другой команды
                if (!isScanning)
                    return;

                ScannerSwitch(false);
                CurrentPage.AbortAnimation("SimpleAnimation");

                var isOk = await TryProcess(name, result);

                if (!isOk)
                {
                    //если не получилось обработать код, то продолжаем сканировать следующий
                    await RunAnimation();
                    ScannerSwitch(true);
                }
            });
        }

        private async Task<bool> TryProcess(string name, Result result = null)
        {
            switch (name)
            {
                case "scanResult":
                    return await TryProcessScanResult(result, true);
                case "scanFromGallery":
                    return await TryScanWithIndicator(await scannerHelper.GetFromGallery());
                case "takePhotoAndScan":
                    return await TryScanWithIndicator(await scannerHelper.TakePhoto());
                case "scanManually":
                    return await ScanManually();
                default:
                    return await Task.FromResult(false);
            };
        }

        private async Task<bool> ScanManually()
        {
            await Navigation.PushAsync(Pages.GetManualScanPage());
            return true;
        }

        private async Task<bool> TryScanWithIndicator(MediaFile file)
        {
            if (file == default)
                return false;

            IsRunningIndicator = true;
            var result = await scannerHelper.Scan(file.Path, cancellationTS.Token);

            if (cancellationTS.IsCancellationRequested)
            {
                cancellationTS.Dispose();
                cancellationTS = new CancellationTokenSource();
                IsRunningIndicator = false;
                return false;
            }

            var isOk = await TryProcessScanResult(result, false);
            IsRunningIndicator = false;
            return isOk;
        }

        private void CancelScanningPhoto()
        {
            if (!cancellationTS.IsCancellationRequested)
                cancellationTS.Cancel();
        }

        private async Task<bool> TryProcessScanResult(Result result, bool fromScanner)
        {
            if (!ValidateScanResult(result))
                return false;

            if (fromScanner)
            {
                if (ScannerSettingsVM.Settings.IsSoundShutterRelease)
                    player.Play(Audio.CameraShutterRelease);

                return await TryProcessCode(result);
            }
            else
            {
                return await TryProcessCode(result);
            }
        }

        private bool ValidateScanResult(Result result)
        {
            if (result == null)
            {
                ShowMessageUnKnownCode(result);
                return false;
            }

            return true;
        }

        private async Task<bool> TryProcessCode(Result result)
        {
            if (result.Text.StartsWith("t=") && CashQRCodeVM.Code.TryParseCode(result.Text))
            {
                return await TryProcessCashQRCode(result);
            }

            ShowMessageUnKnownCode(result);
            return false;
        }

        private async Task<bool> TryProcessCashQRCode(Result result)
        {
            var isContinue = await CurrentPage.DisplayAlert(
                "В этом QRCode закодирован чек",
                result.Text,
                "Получить чек",
                "Отмена");

            if (!isContinue)
                return false;

            await CashQRCodeVM.ProcessCodeCommand.ExecuteAsync();
            return true;
        }

        private void ShowMessageUnKnownCode(Result result)
        {
            CurrentPage.DisplayAlert(
                $"Попробуйте снова или другой код",
                $"Не понятно, что делать с этим кодом 😔:\r\n{result?.Text ?? ""}",
                "Ок");
        }

        private void ScannerSwitch(bool position)
        {
            IsAnalyzing = position;
            IsScanning = position;
        }
    }
}
