using Scanner.ViewModels.Scanner.Checks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Checks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckTabbedPage : TabbedPage
    {
        private readonly CheckEditorViewModel viewModel;

        public CheckTabbedPage(CheckEditorViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;

            BindingContext = viewModel = vm;
        }

        protected override void OnDisappearing()
        {
            viewModel.OnDisappearing();
            base.OnDisappearing();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                (sender as ListView).SelectedItem = null;

                if ((SelectedItem as FriendCheckViewModel).IsCommonCheck)
                    viewModel.MarkProductCommand.Execute((CheckItemViewModel)e.SelectedItem);
            }
        }
    }
}