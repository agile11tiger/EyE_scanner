using System;
using VerifyReceiptSDK.Results;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Checks
{
    public class CheckItemViewModel : BaseViewModel, IEquatable<CheckItemViewModel>
    {
        public CheckItemViewModel(int id, Item item, ImageSource markBoxImage)
        {
            Id = id;
            Item = item;
            this.markBoxImage = markBoxImage;
        }

        private ImageSource markBoxImage;
        private double selectedQuantity;
        public Item Item { get; private set; }
        public int Id { get; set; }
        public bool IsMarked { get; set; }
        public string Name { get => Item.Name; }
        public int Sum { get => Item.Sum; }
        public double SumRub { get => Item.Sum / 100; }
        public int Price { get => Item.Price; }
        public double PriceRub { get => Item.Price / 100; }

        public double Quantity
        {
            get => Item.Quantity;
            set
            {
                if (Item.Quantity != value)
                {
                    Item.Quantity = value;
                    Item.Sum = Price * (int)Quantity;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SumRub));
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

        public CheckItemViewModel Clone(ImageSource image)
        {
            var clone = (CheckItemViewModel)MemberwiseClone();
            clone.Item = Item.PartialClone();
            clone.MarkBoxImage = image;
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
