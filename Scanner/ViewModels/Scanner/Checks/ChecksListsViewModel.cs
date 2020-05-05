using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System;
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
            Checks.Add(new CommonChecksListViewModel() { Title = PageTitles.COMMON_CHECKS });
            Checks.Add(new MyChecksListViewModel() { Title = PageTitles.MY_CHECKS });
            Checks.Add(new IOweChecksListViewModel() { Title = PageTitles.I_OWE });
            Checks.Add(new OweMeChecksListViewModel() { Title = PageTitles.OWE_ME });

            SearchCommand = new AsyncCommand<DateTime>(Search);
            InfoCommand = new AsyncCommand(ShowInfo);
        }

        public ObservableCollection<ChecksListViewModel> Checks { get; }
        public ChecksListViewModel CurrentChecksListVM { get => (ChecksListViewModel)(CurrentPage as TabbedPage).SelectedItem; }
        public IAsyncCommand<DateTime> SearchCommand { get; }
        public IAsyncCommand InfoCommand { get; }

        public Task Add(CheckViewModel сheckVM, string pageTitle)
        {
            return Checks.First(c => c.Title == pageTitle)
                .AddCommand
                .ExecuteAsync(сheckVM);
        }

        public Task Remove(CheckViewModel сheckVM, string pageTitle)
        {
            return Checks.First(c => c.Title == pageTitle)
                .RemoveCommand
                .ExecuteAsync(сheckVM);
        }

        private Task Search(DateTime date)
        {
            return Checks.First(c => c.Title == CurrentChecksListVM.Title)
                .SearchCommand
                .ExecuteAsync(date);
        }

        private Task ShowInfo()
        {
            return Checks.First(c => c.Title == CurrentChecksListVM.Title)
                .InfoCommand
                .ExecuteAsync();
        }
    }
}
