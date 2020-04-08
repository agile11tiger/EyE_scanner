using SQLite;
using System;
using VerificationCheck.Core.Interfaces;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Friends
{
    public class FriendViewModel : BaseViewModel, ISerializable, IDBItem, IEquatable<FriendViewModel>
    {
        public FriendViewModel()
        {
        }

        private int id = -1;
        private string name = "Выберите друга";
        private ImageSource image;
        private string phone;

        public int Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public ImageSource Image
        {
            get => image;
            set
            {
                if (image != value)
                {
                    image = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Change(FriendViewModel friendVM)
        {
            Id = friendVM.Id;
            Name = friendVM.Name;
            Image = friendVM.Image;
            Phone = friendVM.Phone;
        }

        public void Deserialize()
        {
        }

        public void Serialize()
        {
        }

        public bool Equals(FriendViewModel other)
        {
            return Id == other.Id;
        }
    }
}
