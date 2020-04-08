using Scanner.ViewModels.Authorization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Authorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationPage : ContentPage
    {
        public AuthorizationPage(AuthorizationViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CurrentPage = this;

            BindingContext = viewModel;
        }
    }
}