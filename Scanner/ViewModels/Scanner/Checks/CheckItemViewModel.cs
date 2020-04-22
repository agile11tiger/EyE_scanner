using Scanner.Extensions;
using Scanner.Extensions.Interfaces;
using System;
using System.Threading.Tasks;
using VerificationCheck.Core.Results;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    public class CheckItemViewModel : BaseViewModel, IEquatable<CheckItemViewModel>
    {
        public CheckItemViewModel(int id, CheckItem checkItem, ImageSource markBoxImage)
        {
            Id = id;
            this.checkItem = checkItem;
            this.markBoxImage = markBoxImage;
        }

        private readonly CheckItem checkItem;
        private ImageSource markBoxImage;
        private double selectedQuantity;
        public int Id { get; set; }
        public bool IsMarked { get; set; }
        public string Name { get => checkItem.Name; }
        public int Sum { get => checkItem.Sum; }
        public int Price { get => checkItem.Price; }

        public double Quantity
        {
            get => checkItem.Quantity;
            set
            {
                if (checkItem.Quantity != value)
                {
                    checkItem.Quantity = value;
                    checkItem.Sum = Price * (int)Quantity;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Sum));
                }
            }
        }

        public double SelectedQuantity
        {
            get => selectedQuantity;
            set
            {
                if (selectedQuantity != value)
                {
                    selectedQuantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource MarkBoxImage
        {
            get => markBoxImage;
            set
            {
                if (markBoxImage != value)
                {
                    markBoxImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public CheckItemViewModel Clone()
        {
            var cloneCheckItem = (CheckItem)checkItem.Clone();
            var clone = new CheckItemViewModel(Id, cloneCheckItem, null);
            return clone;
        }

        public bool Equals(CheckItemViewModel other)
        {
            if (Id == other.Id)
                return true;

            return false;
        }
    }
}
