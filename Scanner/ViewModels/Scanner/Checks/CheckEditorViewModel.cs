using Ninject;
using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using Scanner.Models;
using Scanner.ViewModels.Scanner.Friends;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий с чеком 
    /// </summary>
    public class CheckEditorViewModel : BaseViewModel
    {
        public CheckEditorViewModel(CheckViewModel checkVM)
        {
            checksListsVM = App.Container.Get<ChecksListsViewModel>();
            Checks = new ObservableCollection<FriendCheckViewModel>();
            SelectedFriendVM = new FriendViewModel();
            SetCommonCheck(checkVM);
            friendCheckVM = GetFriendCheckVM();

            InfoCommand = new AsyncCommand(ShowInfo);
            SelectFriendCommand = new AsyncCommand(SelectFriend);
            MarkProductCommand = new AsyncCommand<CheckItemViewModel>(MarkProduct);
            CreateCheckCommand = new AsyncCommand(CreateCheck);
            AddToMyChecksCommand = new AsyncCommand(AddToMyChecks);
            SendCheckCommand = new AsyncCommand<FriendCheckViewModel>(SendCheck);
            RemoveCheckCommand = new AsyncCommand<FriendCheckViewModel>(RemoveCheck);
        }

        public void SetCommonCheck(CheckViewModel checkVM)
        {
            var friendVM = new FriendViewModel(new Friend()
            {
                Name = PageTitles.COMMON_CHECK,
                Image = ImageSource.FromResource(ImagePaths.Checks)
            });

            Checks.Add(new FriendCheckViewModel(friendVM, checkVM.Check));
        }

        private readonly ChecksListsViewModel checksListsVM;
        private FriendCheckViewModel friendCheckVM;
        private bool isBackCommand = true;

        public FriendViewModel SelectedFriendVM { get; }
        public ObservableCollection<FriendCheckViewModel> Checks { get; }
        public FriendCheckViewModel CommonCheck { get => Checks.First(); }
        public string TitleSelectedPage { get => ((CurrentPage as TabbedPage).SelectedItem as FriendCheckViewModel)?.TitlePage; }
        public ImageSource MarkBoxImage { get => ImageSource.FromResource(ImagePaths.MarkBox); }
        public ImageSource EmptyMarkBoxImage { get => ImageSource.FromResource(ImagePaths.EmptyMarkBox); }

        #region Command
        public IAsyncCommand InfoCommand { get; }
        public IAsyncCommand SelectFriendCommand { get; }
        public IAsyncCommand<CheckItemViewModel> MarkProductCommand { get; }
        public IAsyncCommand CreateCheckCommand { get; }
        public IAsyncCommand AddToMyChecksCommand { get; }
        public IAsyncCommand<FriendCheckViewModel> SendCheckCommand { get; }
        public IAsyncCommand<FriendCheckViewModel> RemoveCheckCommand { get; }
        #endregion

        public async void OnDisappearing()
        {
            if (!isBackCommand)
                return;

            ConcatAllChecks();

            if (CommonCheck.Items.Count == 0)
                await checksListsVM.Remove(CommonCheck, PageTitles.COMMON_CHECKS);
            else
            {
                CommonCheck.SyncItemsWithCheckItems();
                await checksListsVM.Remove(CommonCheck, PageTitles.COMMON_CHECKS);
                await checksListsVM.Add(CommonCheck, PageTitles.COMMON_CHECKS);
            }
        }

        private FriendCheckViewModel GetFriendCheckVM()
        {
            var check = CommonCheck.Check.PartialClone();
            return new FriendCheckViewModel(new FriendViewModel(), check);
        }

        private async Task AddToMyChecks()
        {
            if (!await VerifyItemsExist())
                return;

            UnMarkAllItemsCommonCheck();
            SubtractFriendCheckFromCommonCheck();
            friendCheckVM.SyncItemsWithCheckItems();
            await checksListsVM.Add(new CheckViewModel(friendCheckVM.Check), PageTitles.MY_CHECKS);
            friendCheckVM = GetFriendCheckVM();
        }

        private async Task SelectFriend()
        {
            Pages.FriendsPage.ViewModel.ProcessFriendSelected = ProcessFriendSelected;
            Pages.FriendsPage.ViewModel.IsFriendForCheck = true;
            isBackCommand = false;
            await Navigation.PushAsync(Pages.FriendsPage);
            isBackCommand = true;
        }

        private void ProcessFriendSelected(FriendViewModel friendVM)
        {
            if (!friendCheckVM.FriendVM.Equals(friendVM))
            {
                friendCheckVM.FriendVM.Change(friendVM);
                SelectedFriendVM.Change(friendVM);
            }
        }

        private async Task MarkProduct(CheckItemViewModel item)
        {
            item.IsMarked = !item.IsMarked;

            if (item.IsMarked)
            {
                int valueSelectedQuantity;

                if (item.Quantity != 1)
                {
                    var strSelectedQuantity = await ShowSelectQuantity((int)item.Quantity);

                    if (strSelectedQuantity == null)
                    {
                        item.IsMarked = !item.IsMarked;
                        return;
                    }

                    valueSelectedQuantity = int.Parse(strSelectedQuantity);
                }
                else
                    valueSelectedQuantity = 1;

                item.MarkBoxImage = MarkBoxImage;
                item.SelectedQuantity = valueSelectedQuantity;

                var cloneItem = item.Clone(MarkBoxImage);
                cloneItem.Quantity = valueSelectedQuantity;
                friendCheckVM.Items.Add(cloneItem);
            }
            else
            {
                item.SelectedQuantity = 0;
                friendCheckVM.Items.Remove(item);
                item.MarkBoxImage = EmptyMarkBoxImage;
            }
        }

        private async Task CreateCheck()
        {
            if (!await ValidateCreateCheck())
                return;

            UnMarkAllItemsCommonCheck();
            SubtractFriendCheckFromCommonCheck();

            friendCheckVM.ReCountTotalSum();
            Checks.Add(friendCheckVM);
            friendCheckVM = GetFriendCheckVM();
            SelectedFriendVM.Change(new FriendViewModel());
        }

        private async Task<bool> ValidateCreateCheck()
        {
            if (!await VerifyItemsExist())
                return false;

            if (SelectedFriendVM.Id == -1)
            {
                await ShowNoticeSelectFriend();
                return false;
            }

            if (Checks.FirstOrDefault(c => c.FriendVM.Id == SelectedFriendVM.Id) != null)
            {
                await ShowCheckExist();
                return false;
            }

            return true;
        }

        private async Task<bool> VerifyItemsExist()
        {
            if (CommonCheck.Items.Count != 0)
                return true;
            else
            {
                await ShowCommonCheckIsEmpty();
                return false;
            }
        }

        private void UnMarkAllItemsCommonCheck()
        {
            foreach (var item in CommonCheck.Items)
            {
                item.IsMarked = false;
                item.MarkBoxImage = EmptyMarkBoxImage;
                item.SelectedQuantity = 0;
            }
        }

        private void SubtractFriendCheckFromCommonCheck()
        {
            foreach (var markedItem in friendCheckVM.Items)
            {
                var item = CommonCheck.Items.First(i => i.Id == markedItem.Id);

                if (item.Quantity == 1 || item.Quantity == markedItem.Quantity)
                    CommonCheck.Items.Remove(item);
                else
                {
                    item.Quantity -= markedItem.Quantity;
                    item.SelectedQuantity = 0;
                }
            }

            CommonCheck.ReCountTotalSum();
        }

        private async Task SendCheck(FriendCheckViewModel friendCheckVM)
        {
            //TODO: сериализовать данные и отправить другому пользователю
            friendCheckVM.Check.FriendId = friendCheckVM.FriendVM.Id;
            friendCheckVM.SyncItemsWithCheckItems();
            await checksListsVM.Add(friendCheckVM, PageTitles.OWE_ME);
            Checks.Remove(friendCheckVM);
        }

        private async Task RemoveCheck(FriendCheckViewModel friendCheckVM)
        {
            UnMarkAllItemsCommonCheck();
            AddToCommonCheck(friendCheckVM);
        }

        private void AddToCommonCheck(FriendCheckViewModel friendCheckVM)
        {
            foreach (var markedItem in friendCheckVM.Items)
            {
                var item = markedItem.Clone(MarkBoxImage);
                var indexOfItemInCommonCheck = CommonCheck.Items.IndexOf(item);

                if (indexOfItemInCommonCheck != -1)
                {
                    var itemInCommonCheck = CommonCheck.Items.ElementAt(indexOfItemInCommonCheck);
                    CommonCheck.Items.RemoveAt(indexOfItemInCommonCheck);
                    item.Quantity += itemInCommonCheck.Quantity;
                }

                CommonCheck.Items.Add(item);
            }

            this.friendCheckVM = friendCheckVM;
            SelectedFriendVM.Change(friendCheckVM.FriendVM);
            Checks.Remove(friendCheckVM);
            CommonCheck.ReCountTotalSum();
        }

        private void ConcatAllChecks()
        {
            if (Checks.Count <= 1)
                return;

            var items = CommonCheck.Items;

            foreach (var check in Checks.Skip(1))
                foreach (var item in check.Items)
                {
                    var index = items.IndexOf(item);

                    if (index != -1)
                        items.ElementAt(index).Quantity += item.Quantity;
                    else
                        items.Add(item);
                }
        }

        #region messagesForUser
        private Task ShowInfo()
        {
            return Checks
                .First(c => c.TitlePage == TitleSelectedPage)
                .InfoCommand
                .ExecuteAsync(CurrentPage);
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

        private Task ShowCheckExist()
        {
            return CurrentPage.DisplayAlert(
                   "Упсс",
                   "Для этого друга у вас уже есть созданный чек. " +
                   "Отмените уже созданный чек и создайте новый.",
                   "Ок");
        }
        private Task ShowCommonCheckIsEmpty()
        {
            return CurrentPage.DisplayAlert(
                   "Упсс",
                   "Общий чек пуст",
                   "Ок");
        }
        #endregion
    }
}
