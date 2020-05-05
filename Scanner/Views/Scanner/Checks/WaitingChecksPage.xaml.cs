using Scanner.ViewModels.Scanner.Checks;
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.UserAccountFNS.TryAuthorization();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopToRootAsync();
        }
    }
}