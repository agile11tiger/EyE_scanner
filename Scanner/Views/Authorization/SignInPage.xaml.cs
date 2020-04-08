using Scanner.ViewModels.Authorization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Authorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        public readonly SignInViewModel ViewModel;

        public SignInPage(SignInViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;
            phoneEntry.Completed += (sender, e) => passwordEntry.Focus();

            BindingContext = ViewModel = vm;
        }
    }
}