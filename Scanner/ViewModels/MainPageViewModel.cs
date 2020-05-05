using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
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
            GoToChecksCommand = new AsyncCommand(ToChecks);
            GoToScannerCommand = new AsyncCommand(ToScanner);
            GoToWaitingChecksCommand = new AsyncCommand(ToWaitingChecks);
        }

        public IAsyncCommand GoToChecksCommand { get; }
        public IAsyncCommand GoToScannerCommand { get; }
        public IAsyncCommand GoToWaitingChecksCommand { get; }

        private Task ToChecks()
        {
            return Navigation.PushAsync(Pages.ChecksTabbedPage);
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
            return Navigation.PushAsync(Pages.ScannerPage);
        }

        private Task ToWaitingChecks()
        {
            return Navigation.PushAsync(Pages.WaitingChecksPage);
        }
    }
}
