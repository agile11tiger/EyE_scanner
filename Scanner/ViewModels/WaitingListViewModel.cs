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
        where T : IDBItem, ISerializable, new()
    {
        public WaitingListViewModel() : base()
        {
            RefreshCommand = new AsyncCommand<T>(Refresh);
            RefreshAllCommand = new AsyncCommand(RefreshAll);
            DisplayDataCommand = new AsyncCommand<T>(DisplayData);
        }

        public IAsyncCommand<T> RefreshCommand { get; set; }
        public IAsyncCommand RefreshAllCommand { get; set; }
        public IAsyncCommand<T> DisplayDataCommand { get; set; }
        protected abstract Task Refresh(T item);
        protected abstract Task RefreshAll();
        protected abstract Task DisplayData(T item);
    }
}
