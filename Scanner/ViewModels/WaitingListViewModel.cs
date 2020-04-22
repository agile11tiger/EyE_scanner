using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System.Threading.Tasks;
using VerificationCheck.Core.Interfaces;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, взаимодействующий со списком ожидания чего-либо через команды
    /// </summary>
    public abstract class WaitingListViewModel<T> : ListViewModel<T>
    {
        protected WaitingListViewModel() : base()
        {
            RefreshCommand = new AsyncCommand<T>(Refresh);
            RefreshAllCommand = new AsyncCommand(RefreshAll);
            DisplayDataCommand = new AsyncCommand<T>(DisplayData);
        }

        public IAsyncCommand<T> RefreshCommand { get; }
        public IAsyncCommand RefreshAllCommand { get; }
        public IAsyncCommand<T> DisplayDataCommand { get; }
        protected abstract Task Refresh(T item);
        protected abstract Task RefreshAll();
        protected abstract Task DisplayData(T item);
    }
}
