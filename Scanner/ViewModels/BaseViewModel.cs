using Ninject;
using Scanner.Services.Interfaces;
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
        }

        private bool isBusy;
        private string title;
        public static IAsyncDatabase AsyncDatabase { get; private set; } = App.Container.Get<IAsyncDatabase>();
        public Page CurrentPage { get; set; }
        public INavigation Navigation { get => CurrentPage.Navigation; }

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
        #endregion
    }
}
