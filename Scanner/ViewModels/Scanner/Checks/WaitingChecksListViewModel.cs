using Scanner.Models;
using Scanner.ViewModels.Scanner.QRCodes;
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
            Func<CashQRCode, CashQRCodeViewModel> GetCashQRCodeVM)
            : base()
        {
            this.GetCashQRCodeVM = GetCashQRCodeVM;
            UserAccountFNS = userAccountFNS;
            Image = ImageSource.FromResource(ImagePaths.QrCode);
        }

        private readonly Func<CashQRCode, CashQRCodeViewModel> GetCashQRCodeVM;
        public UserAccountFNSViewModel UserAccountFNS { get; }

        protected override async Task InitializeListFromDatabase()
        {
            await AsyncDatabase.CreateTableAsync<CashQRCode>();
            var cashQRCodes = await AsyncDatabase.GetItemsAsync<CashQRCode>();

            List = new ObservableCollection<CashQRCodeViewModel>(
                cashQRCodes.Select(c => GetCashQRCodeVM(c)));
        }

        protected override async Task Add(CashQRCodeViewModel item)
        {
            var cashQRCodeVM = item.PartialClone();
            List.Add(cashQRCodeVM);
            await AsyncDatabase.AddItemAsync(cashQRCodeVM.CashQRCode);
        }

        protected override async Task Remove(CashQRCodeViewModel item)
        {
            List.Remove(item);
            await AsyncDatabase.GetItemsAsync<CashQRCode>();
            await AsyncDatabase.RemoveItemAsync<CashQRCode>(item.CashQRCode.Id);
        }

        protected async override Task Refresh(CashQRCodeViewModel cashQRCodeVM)
        {
            if (!await UserAccountFNS.TryAuthorization())
                return;

            if (await cashQRCodeVM.TryProcessCode() && await ShowResultCommandRefreshAll(1))
                await Navigation.PushAsync(Pages.ChecksTabbedPage).ConfigureAwait(false);
        }

        protected async override Task RefreshAll()
        {
            if (!await UserAccountFNS.TryAuthorization())
                return;

            var tasks = List.Select(i => i.TryProcessCode());
            var results = await Task.WhenAll(tasks);
            var amountReceivedСhecks = results.Where(r => r == true).Count();

            if (await ShowResultCommandRefreshAll(amountReceivedСhecks).ConfigureAwait(false))
                await Navigation.PushAsync(Pages.ChecksTabbedPage).ConfigureAwait(false);
        }

        protected override Task DisplayData(CashQRCodeViewModel cashQRCodeVM)
        {
            return CurrentPage.DisplayAlert(
                    "Данные чека:",
                    $"ФН — {cashQRCodeVM.FiscalNumber},\n" +
                    $"ФД — {cashQRCodeVM.FiscalDocument},\n" +
                    $"ФП — {cashQRCodeVM.FiscalSignDocument},\n" +
                    $"Дата и время — {cashQRCodeVM.DateTime},\n" +
                    $"Сумма — {cashQRCodeVM.TotalSum}",
                    "Ок");
        }

        protected override Task ShowInfo()
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
