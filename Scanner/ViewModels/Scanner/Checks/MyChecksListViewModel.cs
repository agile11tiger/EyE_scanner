using Scanner.Models;

namespace Scanner.ViewModels.Scanner.Checks
{
    public class MyChecksListViewModel : ChecksListViewModel
    {
        public MyChecksListViewModel() : base(CheckTypes.MyCheck)
        {
        }
    }
}
