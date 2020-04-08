using System.Collections.ObjectModel;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий с историей чеков 
    /// </summary>
    public class HistoryChecksListViewModel : ChecksListViewModel
    {
        public HistoryChecksListViewModel(ChecksListsViewModel checksListsVM) : base(checksListsVM)
        {
        }
    }
}
