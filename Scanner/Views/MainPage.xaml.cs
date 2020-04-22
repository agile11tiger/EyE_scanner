using Ninject;
using Scanner.ViewModels;
using Scanner.Views.Scanner;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel, ScannerPage scannerPage)
        {
            InitializeComponent();
            viewModel.CurrentPage = this;
            BindingContext = viewModel;


            //Приходиться делать так из-за бага "Чёрный экран" в MainPageViewModel
            Navigation.PushAsync(scannerPage);
        }
    }
}