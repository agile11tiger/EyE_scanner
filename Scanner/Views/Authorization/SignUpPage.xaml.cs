using Scanner.ViewModels.Authorization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Authorization
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public readonly SignUpViewModel ViewModel;

        public SignUpPage(SignUpViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;

            nameEntry.Completed += (sender, e) => emailEntry.Focus();
            emailEntry.Completed += (sender, e) => phoneEntry.Focus();

            BindingContext = ViewModel = vm;
        }
    }
}