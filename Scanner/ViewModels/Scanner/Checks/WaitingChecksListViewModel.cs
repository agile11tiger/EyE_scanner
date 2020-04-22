using Ninject;
using Ninject.Parameters;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.ViewModels.Scanner.QRCodes;
using Scanner.Views.Scanner.Checks;
using System;
using System.Collections.ObjectModel;
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
        public WaitingChecksListViewModel(
            UserAccountFNSViewModel userAccountFNS,
            ChecksTabbedPage checksTabbedPage,
            Func<CashQRCode, CashQRCodeViewModel> GetCashQRCodeVM)
            : base()
        {
            this.checksTabbedPage = checksTabbedPage;
            this.GetCashQRCodeVM = GetCashQRCodeVM;
            UserAccountFNS = userAccountFNS;
            InfoCommand = new AsyncCommand(ShowInfo);
            QRCodeImage = ImageSource.FromResource(AppConstants.QrCodePng);
        }

        private readonly ChecksTabbedPage checksTabbedPage;
        private readonly Func<CashQRCode, CashQRCodeViewModel> GetCashQRCodeVM;
        public ImageSource QRCodeImage { get; }
        public UserAccountFNSViewModel UserAccountFNS { get; }
        public IAsyncCommand InfoCommand { get; }

        protected override async Task InitializeListFromDatabase()
        {
            await AsyncDatabase.CreateTableAsync<CashQRCode>();
            var cashQRCodes = await AsyncDatabase.GetItemsAsync<CashQRCode>();

            List = new ObservableCollection<CashQRCodeViewModel>(
                cashQRCodes.Select(c => GetCashQRCodeVM(c)));
        }

        protected override async Task Add(CashQRCodeViewModel item)
        {
            List.Add(item);
            await AsyncDatabase.AddItemAsync(item.CashQRCode);
        }

        protected override async Task Remove(CashQRCodeViewModel item)
        {
            List.RemoveAt(item.CashQRCode.Id);
            await AsyncDatabase.RemoveItemAsync<Friend>(item.CashQRCode.Id);
        }

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
            var isContinue = await ShowResultCommandRefreshAll(amountReceivedСhecks).ConfigureAwait(false);

            if (isContinue)
            {
                await Navigation.PushAsync(checksTabbedPage).ConfigureAwait(false);
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

        private Task ShowInfo()
        {
            return CurrentPage.DisplayAlert(
                    "Что это?",
                    "Это список ожидания чеков. Когда вы сканируете кассовый QR-код, " +
                    "ваш чек сначала попадает сюда, потому что некоторым магазинам требуется " +
                    "время для передачи данных о покупках. " +
                    "Это может занять от нескольких часов до нескольких дней.",
                    "Ок");
        }

        private Task<bool> ShowResultCommandRefreshAll(int amount)
        {
            return CurrentPage.DisplayAlert(
                    "Результат обновления:",
                    $"Получено новых чеков {amount}",
                    "Перейти к чекам",
                    "Остаться");
        }
    }
}
