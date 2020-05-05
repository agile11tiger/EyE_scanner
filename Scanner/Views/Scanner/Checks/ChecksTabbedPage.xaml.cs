using Scanner.ViewModels.Scanner.Checks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Checks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChecksTabbedPage : TabbedPage
    {
        private readonly ChecksListsViewModel viewModel;

        public ChecksTabbedPage(ChecksListsViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;

            foreach (var checksVM in vm.Checks)
                checksVM.CurrentPage = this;

            BindingContext = viewModel = vm;
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            viewModel.SearchCommand.ExecuteAsync(e.NewDate);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Navigation.PopToRootAsync();
        }
    }
}