using Scanner.ViewModels.Scanner.Checks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Checks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckPage : ContentPage
    {
        private readonly CheckViewModel viewModel;

        public CheckPage(FriendCheckViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;
            vm.SyncCheckItemsWithItems();

            BindingContext = viewModel = vm;
        }
    }
}