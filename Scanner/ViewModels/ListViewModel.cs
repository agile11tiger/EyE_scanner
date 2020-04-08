using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.ViewModels.Scanner.QRCodes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VerificationCheck.Core.Interfaces;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Класс, взаимодействующий со списком чего-либо
    /// </summary>
    public abstract class ListViewModel<T> : BaseViewModel
        where T : IDBItem, ISerializable, new()
    {
        public ListViewModel() : base()
        {
            AddCommand = new AsyncCommand<T>(add);
            RemoveCommand = new AsyncCommand<T>(remove);
            SwipeCommand = new AsyncCommand<T>(swipe);

            //Не знаю почему, но это работает...
            //https://overcoder.net/q/19992/%D0%B2%D1%8B%D0%B7%D1%8B%D0%B2%D0%B0%D1%82%D1%8C-%D0%B0%D1%81%D0%B8%D0%BD%D1%85%D1%80%D0%BE%D0%BD%D0%BD%D1%8B%D0%B9-%D0%BC%D0%B5%D1%82%D0%BE%D0%B4-%D0%B2-%D0%BA%D0%BE%D0%BD%D1%81%D1%82%D1%80%D1%83%D0%BA%D1%82%D0%BE%D1%80%D0%B5
            Task.Run(() => setUpList().ConfigureAwait(false)).Wait();
        }

        private async Task setUpList()
        {
            await AsyncDatabase.CreateTableAsync<T>();
            var items = await AsyncDatabase.GetItemsAsync<T>();
            items.ForEach(i => i.Deserialize());

            List = new ObservableCollection<T>(items);
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

        public IAsyncCommand<T> AddCommand { get; set; }
        public IAsyncCommand<T> RemoveCommand { get; set; }
        public IAsyncCommand<T> SwipeCommand { get; set; }

        private async Task add(T item)
        {
            //if (!List.Contains(item))
            //{
            List.Add(item);
            item.Serialize();
            //await AsyncDatabase.AddItemAsync(item);
            //var a = await AsyncDatabase.GetItemsAsync<T>();  //убрать
            //}
        }

        private async Task remove(T item)
        {
            //List.FirstOrDefault(i => i.Id = item.Id);
            var index = List.IndexOf(item);
            if (index != -1)
            {
                List.RemoveAt(index);
                //await AsyncDatabase.RemoveItemAsync<T>(item.Id);
            }
        }

        private async Task swipe(T item)
        {
            List.Remove(item);
            //await AsyncDatabase.RemoveItemAsync<T>(item.Id);
        }
    }
}
