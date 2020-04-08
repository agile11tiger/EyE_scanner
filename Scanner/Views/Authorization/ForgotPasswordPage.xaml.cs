using Scanner.ViewModels.Authorization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Authorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        public readonly ForgotPasswordViewModel ViewModel;

        public ForgotPasswordPage(ForgotPasswordViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;

            BindingContext = ViewModel = vm;
        }
    }
}