using Scanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CurrentPage = this;
            BindingContext = viewModel;
        }
    }
}