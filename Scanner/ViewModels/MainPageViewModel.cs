using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Views.Scanner;
using Scanner.Views.Scanner.Checks;
using System.Threading.Tasks;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, представляющий основную страницу в приложение
    /// </summary>
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel(
            ChecksTabbedPage checksTabbedPage,
            ScannerPage scannerPage,
            WaitingChecksPage waitingChecksPage) 
            : base()
        {
            this.checksTabbedPage = checksTabbedPage;
            this.scannerPage = scannerPage;
            this.waitingChecksPage = waitingChecksPage;
            GoToChecksCommand = new AsyncCommand(ToChecks);
            GoToScannerCommand = new AsyncCommand(ToScanner);
            GoToWaitingChecksCommand = new AsyncCommand(ToWaitingChecks);
        }

        private readonly ChecksTabbedPage checksTabbedPage;
        private readonly ScannerPage scannerPage;
        private readonly WaitingChecksPage waitingChecksPage;
        public IAsyncCommand GoToChecksCommand { get; }
        public IAsyncCommand GoToScannerCommand { get; }
        public IAsyncCommand GoToWaitingChecksCommand { get; }

        private Task ToChecks()
        {
            return Navigation.PushAsync(checksTabbedPage);
        }

        private Task ToScanner()
        {
            #region баг "Чёрный экран"(Оказалось проблема в включенной анимации при переходе на другую страницу, но без неё не красиво...)
            //https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/NavigationPage.cs
            //Когда запускаю "await Navigation.PushAsync(scannerPage)", появляется ЧЁРНЫЙ ЭКРАН, 
            //затем выхожу из "await Navigation.PushAsync(scannerPage)", появляется текущая страница(MainPage), 
            //затем выхожу из метода "toScanner()" и появляется новая страница(ScannerPage).
            //Стоит отменить, что при возврате с ЧЕРНОГО ЭКРАНА, OnAppearing  текущей страницы(MainPage) не запускается.
            //Это при первой загрузке, при последующих все нормально!??
            #endregion
            return Navigation.PushAsync(scannerPage);
        }

        private Task ToWaitingChecks()
        {
            return Navigation.PushAsync(waitingChecksPage);
        }
    }
}
