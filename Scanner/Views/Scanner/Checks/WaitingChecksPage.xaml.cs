using Ninject;
using Scanner.Models;
using Scanner.Services.Interfaces;
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
        private readonly WaitingChecksListViewModel viewModel;

        public WaitingChecksPage(WaitingChecksListViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;
            vm.UserAccountFNS.CurrentPage = this;

            BindingContext = viewModel = vm;
        }

        private bool block;
        protected async override void OnAppearing()
        {
            //TODO: убрать
            #region убрать
            if (!block)
            {
                for (var i = 0; i < 15; i++)
                {
                    var cashQRCode = App.Container.Get<CashQRCodeViewModel>();
                    cashQRCode.FiscalNumber = $"{i}";
                    cashQRCode.FiscalDocument = $"{i}";
                    cashQRCode.FiscalSignDocument = $"{i}";
                    cashQRCode.DateTime = System.DateTime.Now.AddDays(i);
                    cashQRCode.CheckAmount = i + 1000;
                    cashQRCode.TypeCashCheck = "1";
                    viewModel.List.Add(cashQRCode);
                }
                block = true;
            }
            #endregion

            if (!viewModel.UserAccountFNS.Sign.IsAuthorization)
                viewModel.UserAccountFNS.AuthorizationCommand.Execute(true);

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopToRootAsync();
        }
    }
}