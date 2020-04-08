using Newtonsoft.Json;
using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.ViewModels.Scanner.Friends;
using Scanner.Views.Scanner.Friends;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VerificationCheck.Core.Interfaces;
using VerificationCheck.Core.Results;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий с чеком 
    /// </summary>
    public class FriendsChecksViewModel : BaseViewModel, ISerializable, IDBItem
    {
        public FriendsChecksViewModel(Check check) : this()
        {
            this.check = check;
            SetCommonCheck();
        }

        public FriendsChecksViewModel()
        {
            FriendsChecks = new ObservableCollection<FriendCheckViewModel>();
            SelectedFriendVM = new FriendViewModel();
            tempFriendCheckVM = new FriendCheckViewModel();

            InfoCommand = new AsyncCommand(showInfo);
            SelectFriendCommand = new AsyncCommand(selectFriend);
            MarkProductCommand = new AsyncCommand<CheckItemViewModel>(markProduct);
            CreateCheckCommand = new AsyncCommand(createCheck);
            AddToMyChecksCommand = new AsyncCommand(addToMyChecks);
            SendCheckCommand = new AsyncCommand<FriendCheckViewModel>(sendCheck);
            RemoveCheckCommand = new AsyncCommand<FriendCheckViewModel>(removeCheck);
        }

        //TODO: убрать 
        private bool isSetCommonCheck;
        /// <summary>
        /// Если этот класс создается базой данных, а значит check устанавливает бд, то нужно вызывать этот метод сразу после бд.
        /// </summary>
        public void SetCommonCheck()
        {
            if (isSetCommonCheck == false)
            {
                isSetCommonCheck = true;

                var friendVM = new FriendViewModel()
                {
                    Id = -1,
                    Name = "Общий чек",
                    Image = ImageSource.FromResource("Scanner.Resources.Images.Scanner.checks.png")
                };
                var counter = 0;
                var markBoxImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.emptyMarkBox.png");
                var checkItemsVM = check.Items.Select(i => new CheckItemViewModel(counter++, i, markBoxImage));
                var items = new ObservableCollection<CheckItemViewModel>(checkItemsVM);

                commonCheck = new FriendCheckViewModel(friendVM, items);
                FriendsChecks.Add(commonCheck);
            }
        }

        private Check check;
        private FriendCheckViewModel commonCheck;
        private FriendCheckViewModel tempFriendCheckVM;


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CheckJson { get; set; }


        [Ignore]
        public new TabbedPage CurrentPage { get; set; }
        [Ignore]
        public new INavigation Navigation { get => CurrentPage.Navigation; }
        [Ignore]
        public FriendViewModel SelectedFriendVM { get; set; }
        [Ignore]
        public ObservableCollection<FriendCheckViewModel> FriendsChecks { get; set; }
        [Ignore]
        public Func<FriendsChecksViewModel, Task> AddToMyChecks { get; set; }

        [Ignore]
        public string TitleSelectedPage { get => (CurrentPage.SelectedItem as FriendCheckViewModel)?.TitlePage; }
        [Ignore]
        public ImageSource MarkBoxImage { get => ImageSource.FromResource("Scanner.Resources.Images.Scanner.markBox.png"); }
        [Ignore]
        public ImageSource EmptyMarkBoxImage { get => ImageSource.FromResource("Scanner.Resources.Images.Scanner.emptyMarkBox.png"); }


        [Ignore]
        public string RetailPlaceAddress { get => check.RetailPlaceAddress; }
        [Ignore]
        public DateTime DateTime { get => check.CheckDateTime; }
        [Ignore]
        public int CheckAmount
        {
            get => check.TotalSum;
            set
            {
                if (check.TotalSum != value)
                {
                    check.TotalSum = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Command
        [Ignore]
        public IAsyncCommand InfoCommand { get; set; }
        [Ignore]
        public IAsyncCommand SelectFriendCommand { get; set; }
        [Ignore]
        public IAsyncCommand<CheckItemViewModel> MarkProductCommand { get; set; }
        [Ignore]
        public IAsyncCommand CreateCheckCommand { get; set; }
        [Ignore]
        public IAsyncCommand AddToMyChecksCommand { get; set; }
        [Ignore]
        public IAsyncCommand<FriendCheckViewModel> SendCheckCommand { get; set; }
        [Ignore]
        public IAsyncCommand<FriendCheckViewModel> RemoveCheckCommand { get; set; }
        #endregion

        #region ISerializable
        public void Serialize()
        {
            CheckJson = JsonConvert.SerializeObject(check, ISerializable.JsonSettings);
        }

        public void Deserialize()
        {
            check = JsonConvert.DeserializeObject<Check>(CheckJson, ISerializable.JsonSettings);
        }
        #endregion

        private Task selectFriend()
        {
            var friendsPage = App.Container.Get<FriendsPage>();
            friendsPage.ViewModel.ProcessFriendSelected = processFriendSelected;
            friendsPage.ViewModel.IsFriendForCheck = true;
            return Navigation.PushAsync(friendsPage);
        }

        private void processFriendSelected(FriendViewModel friendVM)
        {
            if (!tempFriendCheckVM.FriendVM.Equals(friendVM))
            {
                tempFriendCheckVM.FriendVM.Change(friendVM);
                SelectedFriendVM.Change(friendVM);
            }
        }

        private async Task markProduct(CheckItemViewModel checkItemVM)
        {
            //TODO: checkItem.Quantity может быть ещё чем то кроме количества? Почему оно double
            checkItemVM.IsMarked = !checkItemVM.IsMarked;

            if (checkItemVM.IsMarked)
            {
                var cloneItemVM = checkItemVM.Clone();

                if (checkItemVM.Quantity != 1)
                {
                    var strSelectedQuantity = await showSelectQuantity((int)checkItemVM.Quantity);

                    if (strSelectedQuantity == null)
                    {
                        checkItemVM.IsMarked = !checkItemVM.IsMarked;
                        return;
                    }

                    var valueSelectedQuantity = int.Parse(strSelectedQuantity);
                    checkItemVM.SelectedQuantity = valueSelectedQuantity;
                    cloneItemVM.Quantity = valueSelectedQuantity;
                }

                tempFriendCheckVM.Items.Add(cloneItemVM);
                checkItemVM.MarkBoxImage = MarkBoxImage;
            }
            else
            {
                checkItemVM.SelectedQuantity = 0;
                tempFriendCheckVM.Items.Remove(checkItemVM);
                checkItemVM.MarkBoxImage = EmptyMarkBoxImage;
            }
        }

        private async Task createCheck()
        {
            if (!await validateCreateCheck())
                return;

            unMarkAllItemsCommonCheck();
            changeCommonCheckWhenCreating();
            addTempFriendCheckVMToFriendsCheck();
        }

        private async Task<bool> validateCreateCheck()
        {
            if (SelectedFriendVM.Id == -1)
            {
                await showNoticeSelectFriend();
                return false;
            }

            var friendCheckVM = FriendsChecks.FirstOrDefault(c => c.FriendVM.Id == SelectedFriendVM.Id);

            if (friendCheckVM != null)
            {
                if (!await showCreateNewCheck())
                    return false;
            }

            return true;
        }

        private void changeCommonCheckWhenCreating()
        {
            foreach (var markedItem in tempFriendCheckVM.Items)
            {
                var checkItemVM = commonCheck.Items.First(i => i.Id == markedItem.Id);

                if (checkItemVM.Quantity == 1 || checkItemVM.Quantity == markedItem.Quantity)
                    commonCheck.Items.Remove(checkItemVM);
                else
                {
                    checkItemVM.Quantity -= markedItem.Quantity;
                    checkItemVM.SelectedQuantity = 0;
                }
            }

            SelectedFriendVM.Change(new FriendViewModel());
            CheckAmount = commonCheck.Items.Sum(i => i.Sum);
        }

        private void addTempFriendCheckVMToFriendsCheck()
        {
            var indexFriendCheckVM = FriendsChecks.IndexOf(tempFriendCheckVM);

            if (indexFriendCheckVM != -1)
                FriendsChecks.ElementAt(indexFriendCheckVM).Items = tempFriendCheckVM.Items;
            else
                FriendsChecks.Add(tempFriendCheckVM);

            tempFriendCheckVM = new FriendCheckViewModel();
        }

        private void unMarkAllItemsCommonCheck()
        {
            foreach (var item in commonCheck.Items)
            {
                item.IsMarked = false;
                item.MarkBoxImage = EmptyMarkBoxImage;
            }
        }

        private Task addToMyChecks()
        {
            return AddToMyChecks(this);
        }

        private async Task sendCheck(FriendCheckViewModel friendCheckVM)
        {
            FriendsChecks.Remove(friendCheckVM);
        }

        private async Task removeCheck(FriendCheckViewModel friendCheckVM)
        {
            unMarkAllItemsCommonCheck();
            changeCommonCheckWhenRemoving(friendCheckVM);
        }

        private void changeCommonCheckWhenRemoving(FriendCheckViewModel friendCheckVM)
        {
            foreach (var item in friendCheckVM.Items)
            {
                var indexItemInCommonCheck = commonCheck.Items.IndexOf(item);
                item.IsMarked = true;
                item.MarkBoxImage = MarkBoxImage;
                item.SelectedQuantity = item.Quantity;

                if (indexItemInCommonCheck != -1)
                {
                    var detectedItem = commonCheck.Items.ElementAt(indexItemInCommonCheck);
                    commonCheck.Items.RemoveAt(indexItemInCommonCheck);
                    item.Quantity += detectedItem.Quantity;
                }

                commonCheck.Items.Add(item);
            }

            tempFriendCheckVM = friendCheckVM;
            SelectedFriendVM.Change(friendCheckVM.FriendVM);
            FriendsChecks.Remove(friendCheckVM);
        }

        #region messagesForUser
        private Task showInfo()
        {
            var checkFriendVM = FriendsChecks.First(c => c.TitlePage == TitleSelectedPage);
            return checkFriendVM.InfoCommand.ExecuteAsync();
        }

        private Task showNoticeSelectFriend()
        {
            return CurrentPage.DisplayAlert(
                "Упссс",
                "Вы не выбрали друга",
                "Ок");
        }

        private Task<string> showSelectQuantity(int maxQuantity)
        {
            var numbers = Enumerable.Range(1, maxQuantity).Select(n => n.ToString()).ToArray();

            return CurrentPage.DisplayActionSheet(
                "Выберите количество",
                null,
                null,
                numbers);
        }

        private Task<bool> showCreateNewCheck()
        {
            return CurrentPage.DisplayAlert(
                   "Упсс",
                   "Для этого друга у вас уже есть созданный чек",
                   "Создать новый",
                   "Отмена");
        }
        #endregion
    }
}
