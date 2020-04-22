using Scanner.Extensions.Interfaces;
using Scanner.ViewModels.Scanner.Checks;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Checks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckTabbedPage : TabbedPage
    {
        private readonly FriendsChecksViewModel viewModel;

        public CheckTabbedPage(FriendsChecksViewModel vm)
        {
            InitializeComponent();
            vm.CurrentPage = this;
            vm.SetCommonCheck();

            foreach (var check in vm.FriendsChecks)
                check.CurrentPage = this;

            BindingContext = viewModel = vm;
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