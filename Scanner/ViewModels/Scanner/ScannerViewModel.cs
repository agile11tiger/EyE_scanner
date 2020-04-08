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

namespace Scanner.ViewModels.Scanner
{
    /// <summary>
    /// Класс, взаимодействующий со сканером
    /// </summary>
    public class ScannerViewModel : BaseViewModel, IDisposable
    {
        public ScannerViewModel(CashQRCodeViewModel cashQRCodeVM, ScannerSettingsViewModel scannerSettingsVM, ScannerHelper scannerHelper, IPlayer player) : base()
        {
            CashQRCodeVM = cashQRCodeVM;
            ScannerSettingsVM = scannerSettingsVM;
            this.scannerHelper = scannerHelper;
            this.player = player;
            cancellationTS = new CancellationTokenSource();
            setSubscribe();

            RunAnimationCommand = new AsyncCommand(runAnimation);
            BackCommand = new AsyncCommand(goToBack);
            SwitchTorchCommand = new Command(switchTorch);
            TurnTorchCommand = new Command<bool>(turnTorch);
            InitialOutlineQRCodeCommand = new Command(setInitialOutlineCode);
            InfoCommand = new AsyncCommand(showInfo);
            ScanCommand = new Command<string>(scan);
            ProcessScanResultCommand = new AsyncCommand<Result>(processScanResultFromScanner);
            ScannerSwitchCommand = new Command<bool>(scannerSwitch);
            CancelScanningPhotoCommand = new Command(cancelScanningPhoto);
        }

        private void setSubscribe()
        {
            MessagingCenter.Subscribe<IZxingImageHelper>(
                this, // кто подписывается на сообщения
                "ProgressIndicatorChanged",   // название сообщения
                (sender) =>
                {
                    ProgressIndicator = sender.ProgressIndicator;
                });    // вызываемое действие
        }

        private IScannerHelper scannerHelper;
        private IPlayer player;
        private CancellationTokenSource cancellationTS;
        private bool isZxingScanning;
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
        public CashQRCodeViewModel CashQRCodeVM { get; set; }
        public ScannerSettingsViewModel ScannerSettingsVM { get; set; }

        #region Properties(OnPropertyChanged)
        public bool IsZxingScanning
        {
            get => isZxingScanning;
            set
            {
                if (isZxingScanning != value)
                {
                    isZxingScanning = value;
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
        public IAsyncCommand RunAnimationCommand { get; set; }
        public IAsyncCommand BackCommand { get; set; }
        /// <summary>
        /// Переключает фонарик
        /// </summary>
        public ICommand SwitchTorchCommand { get; set; }
        /// <summary>
        /// Включает или отключает фонарик
        /// </summary>
        public ICommand TurnTorchCommand { get; set; }
        public ICommand InitialOutlineQRCodeCommand { get; set; }
        public IAsyncCommand InfoCommand { get; set; }
        public ICommand ScanCommand { get; set; }
        public ICommand CancelScanningPhotoCommand { get; set; }
        public IAsyncCommand<Result> ProcessScanResultCommand { get; set; }
        public ICommand ScannerSwitchCommand { get; set; }
        #endregion

        /// <summary>
        /// https://docs.microsoft.com/ru-ru/xamarin/xamarin-forms/user-interface/animation/custom
        /// </summary>
        private Task runAnimation()
        {
            return Device.InvokeOnMainThreadAsync(() =>
            {
                var animation = new Animation(v => LineTranslationY = v, 0, 240);
                animation.Commit(CurrentPage, "SimpleAnimation", 16, 2500, Easing.Linear, (v, c) => LineTranslationY = 240, () => true);
            });
        }

        private Task goToBack()
        {
            return Navigation.PopAsync();
        }

        private void switchTorch()
        {
            IsTorchOn = !IsTorchOn;
            setImageForTorch();
        }

        private void turnTorch(bool isTurnOn)
        {
            if (IsTorchOn != isTurnOn)
            {
                IsTorchOn = isTurnOn;
                setImageForTorch();
            }
        }

        private void setImageForTorch()
        {
            if (IsTorchOn)
                TorchImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.torch_on.png");
            else
                TorchImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.torch_off.png");
        }

        private void setInitialOutlineCode()
        {
            TopLineCode = new Rectangle(100, 2, 50, 3);
            BottomLineCode = new Rectangle(100, 245, 50, 3);
            LeftLineCode = new Rectangle(2, 100, 3, 50);
            RightLineCode = new Rectangle(245, 100, 3, 50);
        }

        private Task showInfo()
        {
            return CurrentPage.DisplayAlert(
                    "Как использовать правильно сканирование из галлереи?",
                    "1.Выбрать фото в галлереи с кодом для сканера;\r\n" +
                    "2.Обрезать его используя встроенный редактор, оставив только код для сканера;\r\n" +
                    "3.Важно! Чем больше ширина и длина изображения, тем дольше будет оно обрабатываться.",
                    "Ок");
        }

        private Task processScanResultFromScanner(Result result)
        {
            return scan("fromScanner", result);
        }

        private async void scan(string str)
        {
            try
            {
                await scan(str, null);
            }
            catch { }
        }

        private async Task scan(string str, Result result = null)
        {
            //Чтобы пользователь не смог запустить команду до завершения другой команды
            if (isScanning)
                return;

            isScanning = true;
            scannerSwitch(false);
            CurrentPage.AbortAnimation("SimpleAnimation");

            var isOk = await tryProcess(str, result);

            if (!isOk)
            {
                //если не получилось обработать код, то продолжаем сканировать следующий
                await runAnimation();
                scannerSwitch(true);
            }

            isScanning = false;
        }

        private Task<bool> tryProcess(string str, Result result = null)
        {
            switch (str)
            {
                case "fromScanner":
                    return tryProcessScanResult(result, true);
                case "scanFromGallery":
                    return tryProcessScanResultWithIndicatorFromGallery(true);
                case "takePhotoAndScan":
                    return tryProcessScanResultWithIndicatorFromGallery(false);
                case "scanManually":
                    return scanManually();
                default:
                    return Task.FromResult(false);
            };
        }

        private async Task<bool> scanManually()
        {
            var manualScanPage = App.Container.Get<ManualScanPage>();
            await Navigation.PushAsync(manualScanPage);
            return true;
        }

        private async Task<bool> tryProcessScanResultWithIndicatorFromGallery(bool fromGallery)
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

            var isOk = await tryProcessScanResult(result, false);
            IsRunningIndicator = false;
            return isOk;
        }

        private void cancelScanningPhoto()
        {
            if (!cancellationTS.IsCancellationRequested)
                cancellationTS.Cancel();
        }

        private async Task<bool> tryProcessScanResult(Result result, bool fromScanner)
        {
            if (!validateScanResult(result))
                return false;

            if (fromScanner)
            {
                highlightOutlineScanResult(result);

                if (ScannerSettingsVM.Settings.IsSoundShutterRelease)
                {
                    player.Play("camera_shutter_release.mp3");
                    //спим 2 сек, если будет выделения QR-кода
                }

                var isOk = await tryProcessCode(result);
                setInitialOutlineCode();
                return isOk;
            }
            else
            {
                return await tryProcessCode(result);
            }
        }

        private bool validateScanResult(Result result)
        {
            if (result == null)
            {
                showMessageUnKnownCode(result);
                return false;
            }

            return true;
        }

        private async Task<bool> tryProcessCode(Result result)
        {
            if (result.Text.StartsWith("t=") && CashQRCodeVM.Code.TryParseCode(result.Text))
            {
                return await tryProcessCashQRCode(result);
            }

            showMessageUnKnownCode(result);
            return false;
        }

        private async Task<bool> tryProcessCashQRCode(Result result)
        {
            var isContinue = await CurrentPage.DisplayAlert(
                    "В этом QRCode закодирован чек",
                    result.Text,
                    "Получить чек",
                    "Отмена");

            if (!isContinue)
                return false;

            CashQRCodeVM.ProcessCodeCommand.Execute(null);
            return true;
        }

        private Task showMessageUnKnownCode(Result result)
        {
            return CurrentPage.DisplayAlert(
                    $"Попробуйте снова или другой код",
                    $"Не понятно, что делать с этим кодом 😔:\r\n{result?.Text ?? ""}",
                    "Ок");
        }

        private void scannerSwitch(bool position)
        {
            IsAnalyzing = position;
            //Отключать сканнер нельзя, так как потом он не включиться
            //В будущем zxing, надеюсь, это исправит https://github.com/Redth/ZXing.Net.Mobile/issues/710
            //https://stackoverflow.com/questions/41382512/can-zxing-be-stopped-or-dispose-so-i-can-use-it-again
            //IsScanning = position;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // освободить управляемое состояние (управляемые объекты).
                }
                // освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // задать большим полям значение NULL.

                MessagingCenter.Unsubscribe<IZxingImageHelper>(
                    this,
                    "ProgressIndicatorChanged");

                disposedValue = true;
            }
        }

        // переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~ScannerViewModel()
        // {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion

        //public Image Corner { get; set; }
        //public ZXingScannerView Scanner { get; set; }

        //нужно останавливать фотокамеру  
        //остановить можно с помощью Zxing.IsScanning = false, потом уже нельзя включить сканер
        //https://docs.microsoft.com/ru-ru/xamarin/xamarin-forms/app-fundamentals/custom-renderer/view
        //https://vike.io/ru/339026/
        //получать разрешение камеры например 480х640
        //https://github.com/Redth/ZXing.Net.Mobile/blob/ebcb4e4cdd716570d2c7e8c1112e4165b9550343/Source/ZXing.Net.Mobile.Android/CameraAccess/CameraController.cs#L272

        //https://switch-case.ru/53232983
        //http://www.bolshoyvopros.ru/questions/2515982-diagonal-telefona-55-djujmov-eto-skolko-v-santimetrah.html
        //https://stackoverflow.com/questions/43034961/how-to-get-the-coordinates-of-qr-code-using-zxing
        private void highlightOutlineScanResult(Result result)
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

        public class MyFinderPattern : IComparable<MyFinderPattern>
        {
            public MyFinderPattern(double x, double y, double estimatedModuleSize)
            {
                X = x;
                Y = y;
                EstimatedModuleSize = estimatedModuleSize;
            }

            public MyFinderPattern(Point point, double estimatedModuleSize)
            {
                X = point.X;
                Y = point.Y;
                EstimatedModuleSize = estimatedModuleSize;
            }

            public double X { get; set; }
            public double Y { get; set; }
            public double EstimatedModuleSize { get; set; }

            public int CompareTo(MyFinderPattern other)
            {
                var arg1 = X + Y;
                var arg2 = other.X + other.Y;

                if (arg1 > arg2)
                    return 1;
                else if (arg1 < arg2)
                    return -1;
                else
                    return 0;
            }
        }

    }
}
