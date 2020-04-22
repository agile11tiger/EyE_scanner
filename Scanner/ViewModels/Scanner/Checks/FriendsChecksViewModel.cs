using Newtonsoft.Json;
using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
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
    public class FriendsChecksViewModel : BaseViewModel
    {
        public FriendsChecksViewModel(Check check) : this()
        {
            Check = check;
            SetCommonCheck();
        }

        //Пустой конструктор для бд, так как она предоставит чек.
        public FriendsChecksViewModel()
        {
            checksListsVM = App.Container.Get<ChecksListsViewModel>();
            friendsPage = App.Container.Get<FriendsPage>();
            FriendsChecks = new ObservableCollection<FriendCheckViewModel>();
            SelectedFriendVM = new FriendViewModel();
            tempFriendCheckVM = new FriendCheckViewModel();

            InfoCommand = new AsyncCommand(ShowInfo);
            SelectFriendCommand = new AsyncCommand(SelectFriend);
            MarkProductCommand = new AsyncCommand<CheckItemViewModel>(MarkProduct);
            CreateCheckCommand = new AsyncCommand(CreateCheck);
            AddToMyChecksCommand = new AsyncCommand(AddToMyChecks);
            SendCheckCommand = new AsyncCommand<FriendCheckViewModel>(SendCheck);
            RemoveCheckCommand = new AsyncCommand<FriendCheckViewModel>(RemoveCheck);

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
                var friend = new Friend()
                {
                    Name = "Общий чек",
                    Image = ImageSource.FromResource("Scanner.Resources.Images.Scanner.checks.png")
                };
                var friendVM = new FriendViewModel(friend);
                var counter = 0;
                var markBoxImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.emptyMarkBox.png");
                var checkItemsVM = Check.Items.Select(i => new CheckItemViewModel(counter++, i, markBoxImage));
                var items = new ObservableCollection<CheckItemViewModel>(checkItemsVM);

                commonCheck = new FriendCheckViewModel(friendVM, items);
                FriendsChecks.Add(commonCheck);
            }
        }

        private readonly ChecksListsViewModel checksListsVM;
        private readonly FriendsPage friendsPage;
        private FriendCheckViewModel commonCheck;
        private FriendCheckViewModel tempFriendCheckVM;

        public Check Check { get; }
        public new TabbedPage CurrentPage { get; set; }
        public new INavigation Navigation { get => CurrentPage.Navigation; }
        public FriendViewModel SelectedFriendVM { get; }
        public ObservableCollection<FriendCheckViewModel> FriendsChecks { get; }
        public string TitleSelectedPage { get => (CurrentPage.SelectedItem as FriendCheckViewModel)?.TitlePage; }
        public ImageSource MarkBoxImage { get => ImageSource.FromResource("Scanner.Resources.Images.Scanner.markBox.png"); }
        public ImageSource EmptyMarkBoxImage { get => ImageSource.FromResource("Scanner.Resources.Images.Scanner.emptyMarkBox.png"); }
        public string RetailPlaceAddress { get => Check.RetailPlaceAddress; }
        public DateTime DateTime { get => Check.CheckDateTime; }
        public int CheckAmount
        {
            get => Check.TotalSum;
            set
            {
                if (Check.TotalSum != value)
                {
                    Check.TotalSum = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Command
        public IAsyncCommand InfoCommand { get; }
        public IAsyncCommand SelectFriendCommand { get; }
        public IAsyncCommand<CheckItemViewModel> MarkProductCommand { get; }
        public IAsyncCommand CreateCheckCommand { get; }
        public IAsyncCommand AddToMyChecksCommand { get; }
        public IAsyncCommand<FriendCheckViewModel> SendCheckCommand { get; }
        public IAsyncCommand<FriendCheckViewModel> RemoveCheckCommand { get; }
        #endregion

        private Task AddToMyChecks()
        {
            return checksListsVM.AddToMyChecks(this);
        }

        private Task SelectFriend()
        {
            friendsPage.ViewModel.ProcessFriendSelected = ProcessFriendSelected;
            friendsPage.ViewModel.IsFriendForCheck = true;
            return Navigation.PushAsync(friendsPage);
        }

        private void ProcessFriendSelected(FriendViewModel friendVM)
        {
            if (!tempFriendCheckVM.FriendVM.Equals(friendVM))
            {
                tempFriendCheckVM.FriendVM.Change(friendVM);
                SelectedFriendVM.Change(friendVM);
            }
        }

        private async Task MarkProduct(CheckItemViewModel checkItemVM)
        {
            //TODO: checkItem.Quantity может быть ещё чем то кроме количества? Почему оно double
            checkItemVM.IsMarked = !checkItemVM.IsMarked;

            if (checkItemVM.IsMarked)
            {
                var cloneItemVM = checkItemVM.Clone();

                if (checkItemVM.Quantity != 1)
                {
                    var strSelectedQuantity = await ShowSelectQuantity((int)checkItemVM.Quantity);

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

        private async Task CreateCheck()
        {
            if (!await ValidateCreateCheck())
                return;

            UnMarkAllItemsCommonCheck();
            ChangeCommonCheckWhenCreating();
            AddTempFriendCheckVMToFriendsCheck();
        }

        private async Task<bool> ValidateCreateCheck()
        {
            if (SelectedFriendVM.Id == -1)
            {
                await ShowNoticeSelectFriend();
                return false;
            }

            var friendCheckVM = FriendsChecks.FirstOrDefault(c => c.FriendVM.Id == SelectedFriendVM.Id);

            if (friendCheckVM != null)
            {
                if (!await ShowCreateNewCheck())
                    return false;
            }

            return true;
        }

        private void ChangeCommonCheckWhenCreating()
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

        private void AddTempFriendCheckVMToFriendsCheck()
        {
            var indexFriendCheckVM = FriendsChecks.IndexOf(tempFriendCheckVM);

            if (indexFriendCheckVM != -1)
                FriendsChecks.ElementAt(indexFriendCheckVM).Items = tempFriendCheckVM.Items;
            else
                FriendsChecks.Add(tempFriendCheckVM);

            tempFriendCheckVM = new FriendCheckViewModel();
        }

        private void UnMarkAllItemsCommonCheck()
        {
            foreach (var item in commonCheck.Items)
            {
                item.IsMarked = false;
                item.MarkBoxImage = EmptyMarkBoxImage;
            }
        }

        private async Task SendCheck(FriendCheckViewModel friendCheckVM)
        {
            FriendsChecks.Remove(friendCheckVM);
        }

        private async Task RemoveCheck(FriendCheckViewModel friendCheckVM)
        {
            UnMarkAllItemsCommonCheck();
            ChangeCommonCheckWhenRemoving(friendCheckVM);
        }

        private void ChangeCommonCheckWhenRemoving(FriendCheckViewModel friendCheckVM)
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
        private Task ShowInfo()
        {
            var checkFriendVM = FriendsChecks.First(c => c.TitlePage == TitleSelectedPage);
            return checkFriendVM.InfoCommand.ExecuteAsync();
        }

        private Task ShowNoticeSelectFriend()
        {
            return CurrentPage.DisplayAlert(
                "Упссс",
                "Вы не выбрали друга",
                "Ок");
        }

        private Task<string> ShowSelectQuantity(int maxQuantity)
        {
            var numbers = Enumerable.Range(1, maxQuantity).Select(n => n.ToString()).ToArray();

            return CurrentPage.DisplayActionSheet(
                "Выберите количество",
                null,
                null,
                numbers);
        }

        private Task<bool> ShowCreateNewCheck()
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
