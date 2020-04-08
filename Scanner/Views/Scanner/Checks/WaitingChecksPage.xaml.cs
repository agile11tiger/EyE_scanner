using Scanner.ViewModels.Scanner.Checks;
using Scanner.ViewModels.Scanner.QRCodes;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Checks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaitingChecksPage : ContentPage
    {
        WaitingChecksListViewModel viewModel;

        public WaitingChecksPage(WaitingChecksListViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;
            vm.UserAccountFNS.CurrentPage = this;

            //TODO: убрать
            #region убрать
            for (var i = 0; i< 15; i++)
            {
                var cashQRCode = new CashQRCodeViewModel()
                {
                    FiscalNumber = $"{i}",
                    FiscalDocument = $"{i}",
                    FiscalSignDocument = $"{i}",
                    DateTime = System.DateTime.Now.AddDays(i),
                    CheckAmount = i + 1000,
                    TypeCashCheck = "1",
                };
                vm.List.Add(cashQRCode);
            }
            #endregion

            BindingContext = viewModel = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!viewModel.UserAccountFNS.Sign.IsAuthorization)
                viewModel.UserAccountFNS.AuthorizationCommand.Execute(true);
        }
    }
}