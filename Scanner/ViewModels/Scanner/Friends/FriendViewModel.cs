﻿using Scanner.Models;
using Scanner.Models.Interfaces;
using System;
using Xamarin.Forms;

namespace Scanner.ViewModels.Scanner.Friends
{
    public class FriendViewModel : BaseViewModel, IEquatable<FriendViewModel>, IClone<FriendViewModel>
    {
        public FriendViewModel(Friend friend)
        {
            Friend = friend;
        }

        public FriendViewModel()
        {
            Friend = new Friend();
        }

        public Friend Friend { get; private set; }

        public int Id
        {
            get => Friend.Id;
            set
            {
                if (Friend.Id != value)
                {
                    Friend.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => Friend.Name;
            set
            {
                if (Friend.Name != value)
                {
                    Friend.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource Image
        {
            get => Friend.Image;
            set
            {
                if (Friend.Image != value)
                {
                    Friend.Image = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Phone
        {
            get => Friend.Phone;
            set
            {
                if (Friend.Phone != value)
                {
                    Friend.Phone = value;
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

        public bool Equals(FriendViewModel other)
        {
            return Id == other.Id;
        }

        public FriendViewModel Clone()
        {
            var clone = (FriendViewModel)MemberwiseClone();
            clone.Friend = Friend.Clone();
            return clone;
        }
    }
}
