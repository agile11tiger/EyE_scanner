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
        public MainPageViewModel() : base()
        {
            GoToChecksCommand = new AsyncCommand(toChecks);
            GoToScannerCommand = new AsyncCommand(toScanner);
            GoToWaitingChecksCommand = new AsyncCommand(toChecksWaiting);
        }

        public IAsyncCommand GoToChecksCommand { get; set; }
        public IAsyncCommand GoToScannerCommand { get; set; }
        public IAsyncCommand GoToWaitingChecksCommand { get; set; }

        private Task toChecks()
        {
            var checksPage = App.Container.Get<ChecksTabbedPage>();
            return Navigation.PushAsync(checksPage);
        }

        private Task toScanner()
        {
            #region баг "Чёрный экран"(Оказалось проблема в включенной анимации при переходе на другую страницу, но без неё не красиво...)
            //https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/NavigationPage.cs
            //Когда запускаю "await Navigation.PushAsync(scannerPage)", появляется ЧЁРНЫЙ ЭКРАН, 
            //затем выхожу из "await Navigation.PushAsync(scannerPage)", появляется текущая страница(MainPage), 
            //затем выхожу из метода "toScanner()" и появляется новая страница(ScannerPage).
            //Стоит отменить, что при возврате с ЧЕРНОГО ЭКРАНА, OnAppearing  текущей страницы(MainPage) не запускается.
            //Это при первой загрузке, при последующих все нормально!??
            #endregion
            var scannerPage = App.Container.Get<ScannerPage>();
            return Navigation.PushAsync(scannerPage);
        }

        private Task toChecksWaiting()
        {
            var checksWaitingPage = App.Container.Get<WaitingChecksPage>();
            return Navigation.PushAsync(checksWaitingPage);
        }
    }
}
