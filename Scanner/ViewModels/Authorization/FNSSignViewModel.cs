using Scanner.Models;
using System;
using System.Threading.Tasks;
using VerificationCheck.Core;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей авторизации пользователя в ФНС
    /// </summary>
    public abstract class FNSSignViewModel : SignViewModel
    {
        protected FNSSignViewModel(FNS fns, Sign sign) : base(sign)
        {
            FNS = fns;
        }

        public FNS FNS { get; set; }
    }
}
