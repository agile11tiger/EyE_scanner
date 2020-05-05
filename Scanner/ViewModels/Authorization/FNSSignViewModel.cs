using Scanner.Models;

namespace Scanner.ViewModels.Authorization
{
    /// <summary>
    /// Класс, взаимодействующий со страницей авторизации пользователя в ФНС
    /// </summary>
    public abstract class FNSSignViewModel : SignViewModel
    {
        protected FNSSignViewModel(Sign sign) : base(sign)
        {
        }
    }
}
