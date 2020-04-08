using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий со списком действительных чеков 
    /// </summary>
    public class ChecksListsViewModel : BaseViewModel
    {
        public ChecksListsViewModel(ObservableCollection<ChecksListViewModel> checks) : base()
        {
            Checks = checks;
            var commonChecksVM = new CommonChecksListViewModel(this);
            var myChecksVM = new MyChecksListViewModel(this);

            commonChecksVM.Title = AppConstants.NAME_PAGE_COMMON_CHECKS;
            myChecksVM.Title = AppConstants.NAME_PAGE_MY_CHECKS;
            Checks.Add(commonChecksVM);
            Checks.Add(myChecksVM);

            SearchCommand = new AsyncCommand<string>(search);
            InfoCommand = new AsyncCommand(showInfo);
        }

        public ObservableCollection<ChecksListViewModel> Checks { get; set; }
        public new TabbedPage CurrentPage { get; set; }
        public new INavigation Navigation { get => CurrentPage.Navigation; }
        public IAsyncCommand<string> SearchCommand { get; set; }
        public IAsyncCommand InfoCommand { get; set; }

        public Task AddToCommonChecks(FriendsChecksViewModel friendsChecksVM)
        {
            return Checks.First(c => c.Title == AppConstants.NAME_PAGE_COMMON_CHECKS)
                .AddCommand
                .ExecuteAsync(friendsChecksVM);
        }

        public Task AddToMyChecks(FriendsChecksViewModel friendsChecksVM)
        {
            return Checks.First(c => c.Title == AppConstants.NAME_PAGE_MY_CHECKS)
                .AddCommand
                .ExecuteAsync(friendsChecksVM);
        }

        //TODO: Возможно это можно сделать как нибудь в xaml
        private Task search(string searchText)
        {
            var currentChecksVM = (ChecksListViewModel)CurrentPage.SelectedItem;

            return Checks.First(c => c.Title == currentChecksVM.Title)
                .SearchCommand
                .ExecuteAsync(searchText);
        }

        //TODO: Возможно это можно сделать как нибудь в xaml
        private Task showInfo()
        {
            var currentChecksVM = (ChecksListViewModel)CurrentPage.SelectedItem;

            return Checks.First(c => c.Title == currentChecksVM.Title)
                .InfoCommand
                .ExecuteAsync();
        }
    }
}
