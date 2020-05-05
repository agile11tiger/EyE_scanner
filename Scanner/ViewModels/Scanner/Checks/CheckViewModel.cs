using Scanner.Models;
using Scanner.ViewModels.Scanner.Friends;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using VerifyReceiptSDK.Results;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    /// <summary>
    /// Класс, взаимодействующий с чеком 
    /// </summary>
    public class CheckViewModel : BaseViewModel, IEquatable<CheckViewModel>
    {
        public CheckViewModel(Check check)
        {
            Check = check;
        }

        private ObservableCollection<CheckItemViewModel> items;
        public Check Check { get; }
        public ObservableCollection<CheckItemViewModel> Items
        {
            get => items;
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RetailPlaceAddress { get => Check.Receipt.RetailPlaceAddress; }
        public DateTime DateTime { get => Check.Receipt.ReceiptDateTime; }

        public int TotalSum
        {
            get => Check.Receipt.TotalSum;
            set
            {
                if (Check.Receipt.TotalSum != value)
                {
                    Check.Receipt.TotalSum = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalSumRub));
                }
            }
        }

        public double TotalSumRub
        {
            get => Check.Receipt.TotalSum / 100;
        }

        public void ReCountTotalSum()
        {
            TotalSum = Items.Sum(i => i.Sum);
        }

        public void SyncCheckItemsWithItems()
        {
            var counter = 0;

            Items = Check.Receipt.Items.Count != 0
                ? new ObservableCollection<CheckItemViewModel>(
                    Check.Receipt.Items.Select(i => new CheckItemViewModel(
                        counter++,
                        i.PartialClone(),
                        ImageSource.FromResource(ImagePaths.EmptyMarkBox))))
                : new ObservableCollection<CheckItemViewModel>();
        }

        public void SyncItemsWithCheckItems()
        {
            if (Items.Count != 0)
            {
                var items = Items.Select(i => i.Item.PartialClone());
                Check.Receipt.Items = new List<Item>(items);
                ReCountTotalSum();
            }
        }

        public bool Equals(CheckViewModel other)
        {
            return Check.Id == other.Check.Id;
        }
    }
}
