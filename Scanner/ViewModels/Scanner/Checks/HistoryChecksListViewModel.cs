using Scanner.Models;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий с историей чеков 
    /// </summary>
    public class HistoryChecksListViewModel : ChecksListViewModel
    {
        public HistoryChecksListViewModel() : base(CheckTypes.HistoryCheck)
        {
        }
    }
}
