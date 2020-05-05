using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, взаимодействующий со списком чего-либо
    /// </summary>
    public abstract class ListViewModel<T> : BaseViewModel
    {
        protected ListViewModel() : base()
        {
            AddCommand = new AsyncCommand<T>(Add);
            RemoveCommand = new AsyncCommand<T>(Remove);
        }

        public Task CallInitializeListFromDatabase()
        {
            //Не знаю почему, но это работает...
            //https://overcoder.net/q/19992/%D0%B2%D1%8B%D0%B7%D1%8B%D0%B2%D0%B0%D1%82%D1%8C-%D0%B0%D1%81%D0%B8%D0%BD%D1%85%D1%80%D0%BE%D0%BD%D0%BD%D1%8B%D0%B9-%D0%BC%D0%B5%D1%82%D0%BE%D0%B4-%D0%B2-%D0%BA%D0%BE%D0%BD%D1%81%D1%82%D1%80%D1%83%D0%BA%D1%82%D0%BE%D1%80%D0%B5
            return Task.Run(async () =>
            {
                try
                {
                    await InitializeListFromDatabase().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    //TODO как-то обрабатывать
                }
            });
        }

        private ObservableCollection<T> list;
        public ObservableCollection<T> List
        {
            get => list;
            set
            {
                if (list != value)
                {
                    list = value;
                    OnPropertyChanged();
                }
            }
        }

        public IAsyncCommand<T> AddCommand { get; }
        public IAsyncCommand<T> RemoveCommand { get; }
        protected abstract Task InitializeListFromDatabase();
        protected abstract Task Add(T item);
        protected abstract Task Remove(T item);
    }
}
