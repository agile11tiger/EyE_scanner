using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models.Iterfaces;
using System.Threading.Tasks;

namespace Scanner.ViewModels.Scanner.QRCodes
{
    /// <summary>
    /// Класс, взаимодействующий с кодом 
    /// </summary>
    public abstract class CodeViewModel<Request, Result> : BaseViewModel
    {
        protected CodeViewModel(ICode code, Request request, Result result)
        {
            Code = code;
            RequestList = request;
            ResultList = result;
            failMessage = "";
            ProcessCodeCommand = new AsyncCommand(ProcessQRCode);
        }

        private string failMessage;
        public ICode Code { get; }
        public Request RequestList { get; }
        public Result ResultList { get; }
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

        protected abstract Task ProcessQRCode();
    }
}
