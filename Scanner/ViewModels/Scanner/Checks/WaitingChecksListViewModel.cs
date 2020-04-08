using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.ViewModels.Scanner.QRCodes;
using Scanner.Views.Scanner.Checks;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий со списком ожидающих чеков 
    /// </summary>
    public class WaitingChecksListViewModel : WaitingListViewModel<CashQRCodeViewModel>
    {
        public WaitingChecksListViewModel(UserAccountFNSViewModel userAccountFNS) : base()
        {
            UserAccountFNS = userAccountFNS;
            InfoCommand = new AsyncCommand(showInfo);
            qrCodeImage = ImageSource.FromResource(AppConstants.QrCodePng);
        }

        private ImageSource qrCodeImage;
        public ImageSource QRCodeImage { get => qrCodeImage; }
        public UserAccountFNSViewModel UserAccountFNS { get; set; }
        public IAsyncCommand InfoCommand { get; set; }

        protected override Task Refresh(CashQRCodeViewModel cashQRCodeVM)
        {
            if (UserAccountFNS.Sign.IsAuthorization)
                return cashQRCodeVM.ProcessCodeCommand.ExecuteAsync();
            else
                return UserAccountFNS.AuthorizationCommand.ExecuteAsync(true);
        }

        protected async override Task RefreshAll()
        {
            var tasks = List.Select(i => i.TryProcessCode());
            var results = await Task.WhenAll(tasks);
            var amountReceivedСhecks = results.Where(r => r == true).Count();
            var isContinue = await showResultCommandRefreshAll(amountReceivedСhecks).ConfigureAwait(false);

            if (isContinue)
            {
                var checksPage = App.Container.Get<ChecksTabbedPage>();
                await Navigation.PushAsync(checksPage).ConfigureAwait(false);
            }
        }

        protected override Task DisplayData(CashQRCodeViewModel cashQRCodeVM)
        {
            return CurrentPage.DisplayAlert(
                    "Данные чека:",
                    $"ФН — {cashQRCodeVM.FiscalNumber},\n" +
                    $"ФД — {cashQRCodeVM.FiscalDocument},\n" +
                    $"ФП — {cashQRCodeVM.FiscalSignDocument},\n" +
                    $"Дата и время — {cashQRCodeVM.DateTime},\n" +
                    $"Сумма — {cashQRCodeVM.CheckAmount}",
                    "Ок");
        }

        private Task showInfo()
        {
            return CurrentPage.DisplayAlert(
                    "Что это?",
                    "Это список ожидания чеков. Когда вы сканируете кассовый QR-код, " +
                    "ваш чек сначала попадает сюда, потому что некоторым магазинам требуется " +
                    "время для передачи данных о покупках. " +
                    "Это может занять от нескольких часов до нескольких дней.",
                    "Ок");
        }

        private Task<bool> showResultCommandRefreshAll(int amount)
        {
            return CurrentPage.DisplayAlert(
                    "Результат обновления:",
                    $"Получено новых чеков {amount}",
                    "Перейти к чекам",
                    "Остаться");
        }
    }
}
