using Scanner.Messages;
using Scanner.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyReceiptSDK.Results;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей авторизации пользователя
    /// </summary>
    public abstract class SignViewModel : BaseViewModel
    {
        protected SignViewModel(Sign sign) : base()
        {
            Sign = sign;
        }

        public Sign Sign { get; set; }

        public bool IsAuthorization
        {
            get => Sign.IsAuthorization;
            set
            {
                if (Sign.IsAuthorization != value)
                {
                    Sign.IsAuthorization = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FailMessage
        {
            get => Sign.FailMessage;
            set
            {
                if (Sign.FailMessage != value)
                {
                    Sign.FailMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => Sign.Name;
            set
            {
                if (Sign.Name != value)
                {
                    Sign.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => Sign.Email;
            set
            {
                if (Sign.Email != value)
                {
                    Sign.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Phone
        {
            get => Sign.Phone;
            set
            {
                if (Sign.Phone != value)
                {
                    Sign.Phone = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => Sign.Password;
            set
            {
                if (Sign.Password != value)
                {
                    Sign.Password = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GetClearPhone()
        {
            return Regex.Replace(Phone, @"[^+\d]", "");
        }

        public async Task<bool> TryExecute(Task<Result> task)
        {
            if (task != await Task.WhenAny(task, Task.Delay(5000)))
            {
                FailMessage = CommonMessages.NoInternet;
                return false;
            }

            if (task.Result.IsSuccess)
                return true;
            else
                FailMessage = task.Result.Message;

            return false;
        }
    }
}
