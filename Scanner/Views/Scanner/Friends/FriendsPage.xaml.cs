using Scanner.ViewModels.Scanner.Friends;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.Friends
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendsPage : ContentPage
    {
        public FriendsViewModel ViewModel { get; private set; }

        public FriendsPage(FriendsViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CurrentPage = this;

            BindingContext = ViewModel = viewModel;
        }

        private bool block;
        protected override void OnAppearing()
        {
            //TODO: убрать
            #region создания друзей(потом убрать)
            if(!block)
            {
                for (var i = 0; i < 15; i++)
                {
                    var f = new FriendViewModel()
                    {
                        Id = i,
                        Name = "Вася" + i,
                        Phone = "+78768687",
                        Image = ImageSource.FromResource("Scanner.Resources.Images.filka.jpg")
                    };
                    ViewModel.List.Add(f);
                }
            }
            #endregion
            base.OnAppearing();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                ViewModel.SearchCommand.Execute("");
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                (sender as ListView).SelectedItem = null;
                ViewModel.ItemSelectedCommand.Execute((FriendViewModel)e.SelectedItem);
            }
        }
    }
}