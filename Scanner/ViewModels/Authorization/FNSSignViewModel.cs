using Scanner.Models;
using System;
using System.Threading.Tasks;
using VerificationCheck.Core;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей авторизации пользователя в ФНС
    /// </summary>
    public class FNSSignViewModel : SignViewModel
    {
        public FNSSignViewModel(FNS fns, Func<Sign, Task> syncWithUserAccount, Sign sign) : base(sign)
        {
            FNS = fns;
            SyncWithUserAccount = syncWithUserAccount;
        }

        public Func<Sign, Task> SyncWithUserAccount { get; set; }
        public FNS FNS { get; set; }
    }
}
