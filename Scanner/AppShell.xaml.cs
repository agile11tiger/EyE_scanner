using Scanner.ViewModels;
using Scanner.Views;

namespace Scanner
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell(AppShellViewModel viewModel, MainPage main)
        {
            InitializeComponent();
            viewModel.CurrentPage = this;
            viewModel.UserAccount.CurrentPage = this;

            mainPage.Content = main;
            BindingContext = viewModel;
        }
    }
}
