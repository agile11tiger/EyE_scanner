using Scanner.ViewModels.Scanner.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VerificationCheck.Core.Results;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Checks
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChecksTabbedPage : TabbedPage
    {
        ChecksListsViewModel viewModel;

        public ChecksTabbedPage(ChecksListsViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
            viewModel.CurrentPage = this;

            foreach (var checksVM in viewModel.Checks)
                checksVM.CurrentPage = this;

            //TODO: убрать
            #region создания чеков(потом убрать)
            var items = new List<CheckItem>();
            var item = new CheckItem()
            {
                Sum = 150,
                Name = "Карпачо",
                Price = 50,
                Quantity = 3
            };
            var item1 = new CheckItem()
            {
                Sum = 300,
                Name = "Сыр",
                Price = 100,
                Quantity = 3
            };
            items.Add(item);
            items.Add(item1);

            var check = new Check()
            {
                Items = items,
                TotalSum = 450,
                RetailPlaceAddress = "Магнит ул.ФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФФ",
                CheckDateTime = System.DateTime.Now,
            };
            var checkVM = new FriendsChecksViewModel(check);

            var list = viewModel.Checks.First(c => c.Title == AppConstants.NAME_PAGE_COMMON_CHECKS).List;
            for (var i = 0; i < 15; i++)
                list.Add(checkVM);
            #endregion

            BindingContext = viewModel;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                var nameCurrentVM = ((ChecksListViewModel)SelectedItem).Title;
                viewModel.Checks.First(c => c.Title == nameCurrentVM).SearchCommand.Execute("");
            }
        }
    }
}