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
        public CodeViewModel(ICode code, WaitingChecksListViewModel waitingChecksListVM, ChecksListsViewModel checksListsVM)
        {
            Code = code;
            RequestList = waitingChecksListVM;
            ResultLists = checksListsVM;
            failMessage = "";
            ProcessCodeCommand = new AsyncCommand(ProcessCode);
        }

        private string failMessage;
        [Ignore]
        public WaitingChecksListViewModel RequestList { get; private set; }
        [Ignore]
        public ChecksListsViewModel ResultLists { get; private set; }
        [Ignore]
        public ICode Code { get; set; }

        [Ignore]
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

        [Ignore]
        public IAsyncCommand ProcessCodeCommand { get; private set; }

        protected abstract Task ProcessCode();
    }
}
