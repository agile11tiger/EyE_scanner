using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models.Iterfaces;
using Scanner.ViewModels.Scanner.Checks;
using SQLite;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Scanner.QRCodes
{
    /// <summary>
    /// Класс, взаимодействующий с кодом 
    /// </summary>
    public abstract class CodeViewModel<Request, Result> : BaseViewModel
    {
        protected CodeViewModel(ICode code, WaitingChecksListViewModel waitingChecksListVM, ChecksListsViewModel checksListsVM)
        {
            Code = code;
            RequestList = waitingChecksListVM;
            ResultLists = checksListsVM;
            failMessage = "";
            ProcessCodeCommand = new AsyncCommand(ProcessCode);
        }

        private string failMessage;
        public ICode Code { get; }
        public WaitingChecksListViewModel RequestList { get; }
        public ChecksListsViewModel ResultLists { get; }
        public string FailMessage
        {
            get => failMessage;
            set
            {
                if (failMessage != value)
                {
                    failMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public IAsyncCommand ProcessCodeCommand { get; }

        protected abstract Task ProcessCode();
    }
}
