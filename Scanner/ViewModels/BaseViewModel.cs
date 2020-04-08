using Ninject;
using Scanner.Services.Interfaces;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Scanner.ViewModels
{
    /// <summary>
    /// Базовый класс для всех ViewModel, который реализует INotifyPropertyChanged
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel()
        {
            isBusy = false;
            title = string.Empty;
            AsyncDatabase = App.Container.Get<IAsyncDatabase>();
        }

        private bool isBusy;
        private string title;
        [Ignore]
        public static IAsyncDatabase AsyncDatabase { get; private set; }
        [Ignore]
        public Page CurrentPage { get; set; }
        [Ignore]
        public INavigation Navigation { get => CurrentPage.Navigation;}

        [Ignore]
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected bool SetProperty<T>(ref T backingStore, T value,
        //    [CallerMemberName]string propertyName = "",
        //    Action onChanged = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(backingStore, value))
        //        return false;

        //    backingStore = value;
        //    onChanged?.Invoke();
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    var changed = PropertyChanged;
        //    if (changed == null)
        //        return;

        //    changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        #endregion
    }
}
