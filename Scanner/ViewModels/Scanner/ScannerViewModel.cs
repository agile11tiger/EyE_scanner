using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Services;
using Scanner.Services.Interfaces;
using Scanner.ViewModels.Scanner.QRCodes;
using Scanner.Views.Scanner.QRCodes;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

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
            IPlayer player,
            Func<ManualScanPage> GetManualScanPage) 
            : base()
        {
            CashQRCodeVM = cashQRCodeVM;
            ScannerSettingsVM = scannerSettingsVM;
            this.scannerHelper = scannerHelper;
            this.player = player;
            this.GetManualScanPage = GetManualScanPage;
            cancellationTS = new CancellationTokenSource();
            SetSubscribe();

            RunAnimationCommand = new AsyncCommand(RunAnimation);
            BackCommand = new AsyncCommand(GoToBack);
            SwitchTorchCommand = new Command(SwitchTorch);
            TurnTorchCommand = new Command<bool>(TurnTorch);
            InitialOutlineQRCodeCommand = new Command(SetInitialOutlineCode);
            InfoCommand = new AsyncCommand(ShowInfo);
            ScanCommand = new AsyncCommand<string>(Scan);
            ProcessScanResultCommand = new AsyncCommand<Result>(ProcessScanResultFromScanner);
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
        private readonly Func<ManualScanPage> GetManualScanPage;
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
        public ICommand InitialOutlineQRCodeCommand { get; }
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
                TorchImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.torch_on.png");
            else
                TorchImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.torch_off.png");
        }

        private void SetInitialOutlineCode()
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

        private Task ProcessScanResultFromScanner(Result result)
        {
            return Scan("fromScanner", result);
        }

        private Task Scan(string str)
        {
            return Scan(str, null);
        }

        private async Task Scan(string str, Result result = null)
        {
            //Чтобы пользователь не смог запустить команду до завершения другой команды
            if (!isScanning)
                return;

            ScannerSwitch(false);
            CurrentPage.AbortAnimation("SimpleAnimation");

            var isOk = await TryProcess(str, result);

            if (!isOk)
            {
                //если не получилось обработать код, то продолжаем сканировать следующий
                await RunAnimation();
                ScannerSwitch(true);
            }
        }

        private Task<bool> TryProcess(string str, Result result = null)
        {
            switch (str)
            {
                case "fromScanner":
                    return TryProcessScanResult(result, true);
                case "scanFromGallery":
                    return TryProcessScanResultWithIndicatorFromGallery(true);
                case "takePhotoAndScan":
                    return TryProcessScanResultWithIndicatorFromGallery(false);
                case "scanManually":
                    return ScanManually();
                default:
                    return Task.FromResult(false);
            };
        }

        private async Task<bool> ScanManually()
        {
            await Navigation.PushAsync(GetManualScanPage());
            return true;
        }

        private async Task<bool> TryProcessScanResultWithIndicatorFromGallery(bool fromGallery)
        {
            var file = fromGallery
                    ? await scannerHelper.GetFromGallery()
                    : await scannerHelper.TakePhoto();

            if (file == null)
                return false;

            IsRunningIndicator = true;
            var result = await scannerHelper.GetResult(file.Path, cancellationTS.Token);

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
                HighlightOutlineScanResult(result);

                if (ScannerSettingsVM.Settings.IsSoundShutterRelease)
                {
                    player.Play("camera_shutter_release.mp3");
                    //спим 2 сек, если будет выделения QR-кода
                }

                var isOk = await TryProcessCode(result);
                SetInitialOutlineCode();
                return isOk;
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
            var isContinue = await Device.InvokeOnMainThreadAsync(
                () => CurrentPage.DisplayAlert(
                    "В этом QRCode закодирован чек",
                    result.Text,
                    "Получить чек",
                    "Отмена"));

            if (!isContinue)
                return false;

            CashQRCodeVM.ProcessCodeCommand.Execute(null);
            return true;
        }

        private void ShowMessageUnKnownCode(Result result)
        {
            Device.InvokeOnMainThreadAsync(() => CurrentPage.DisplayAlert(
                    $"Попробуйте снова или другой код",
                    $"Не понятно, что делать с этим кодом 😔:\r\n{result?.Text ?? ""}",
                    "Ок"));
        }

        private void ScannerSwitch(bool position)
        {
            IsAnalyzing = position;
            IsScanning = position;
        }

        //public Image Corner { get; set; }
        //public ZXingScannerView Scanner { get; set; }

        //получать разрешение камеры например 480х640
        //https://github.com/Redth/ZXing.Net.Mobile/blob/ebcb4e4cdd716570d2c7e8c1112e4165b9550343/Source/ZXing.Net.Mobile.Android/CameraAccess/CameraController.cs#L272
        //https://switch-case.ru/53232983
        //http://www.bolshoyvopros.ru/questions/2515982-diagonal-telefona-55-djujmov-eto-skolko-v-santimetrah.html
        //https://stackoverflow.com/questions/43034961/how-to-get-the-coordinates-of-qr-code-using-zxing
        private void HighlightOutlineScanResult(Result result)
        {
            //var cofX = Scanner.Width / 480; //коэффициент, для преобразования пикселей в см
            //var cofY = Scanner.Height / 640;

            //if (result.ResultPoints.Length == 3)
            //{
            //    var point = new Point(Scanner.Width, Scanner.Height); //крайняя точка у сканера
            //    var points = result.ResultPoints
            //        .Select(p =>
            //        {
            //            var x = (p.X * cofX) + Corner.X; //пиксели в см, плюс текущая позиция в см(настолько надо смещать)
            //            var y = (p.Y * cofY) + Corner.Y;
            //            var moduleSise = (p as FinderPattern).EstimatedModuleSize;
            //            return new MyFinderPattern(point.Offset(-x, -y), moduleSise);
            //        })
            //        .OrderBy(p => p)
            //        .ToList();

            //    //var firstPoint = new Point(points[0].X - points[0].EstimatedModuleSize, points[0].Y - points[0].EstimatedModuleSize);
            //    //var secondPoint = new Point(points[1].X - points[1].EstimatedModuleSize, points[1].Y - points[1].EstimatedModuleSize);
            //    //var thirdPoint = new Point(points[2].X - points[2].EstimatedModuleSize, points[2].Y - points[2].EstimatedModuleSize);

            //    var firstPoint = new Point(points[0].X - 5, points[0].Y - 5);
            //    var secondPoint = new Point(points[1].X - 5, points[1].Y - 5);
            //    var thirdPoint = new Point(points[2].X - 5, points[2].Y - 5);

            //    var width = thirdPoint.X - firstPoint.X;
            //    var height =points.OrderBy(p => p.Y).Last().Y - points.OrderBy(p => p.Y).First().Y;

            //    TopLineQRCode = new Rectangle(firstPoint.X, firstPoint.Y, width, 3);
            //    BottomLineQRCode = new Rectangle(firstPoint.X, secondPoint.Y, width, 3);
            //    LeftLineQRCode = new Rectangle(firstPoint.X, firstPoint.Y, 3, height);
            //    RightLineQRCode = new Rectangle(firstPoint.X + width, firstPoint.Y, 3, height+3);

            //    Thread.Sleep(2000);
            //}
        }

        //public class MyFinderPattern : IComparable<MyFinderPattern>
        //{
        //    public MyFinderPattern(double x, double y, double estimatedModuleSize)
        //    {
        //        X = x;
        //        Y = y;
        //        EstimatedModuleSize = estimatedModuleSize;
        //    }

        //    public MyFinderPattern(Point point, double estimatedModuleSize)
        //    {
        //        X = point.X;
        //        Y = point.Y;
        //        EstimatedModuleSize = estimatedModuleSize;
        //    }

        //    public double X { get; set; }
        //    public double Y { get; set; }
        //    public double EstimatedModuleSize { get; set; }

        //    public int CompareTo(MyFinderPattern other)
        //    {
        //        var arg1 = X + Y;
        //        var arg2 = other.X + other.Y;

        //        if (arg1 > arg2)
        //            return 1;
        //        else if (arg1 < arg2)
        //            return -1;
        //        else
        //            return 0;
        //    }
        //}

    }
}
