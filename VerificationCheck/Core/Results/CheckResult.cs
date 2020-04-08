namespace VerificationCheck.Core.Results
{
    /// <summary>
    /// Класс, представляющий ответ, полученный в результате проверки существования чека и 
    /// описывающий информацию, получаемую в результате запроса от ФНС детальной информации по чеку
    /// </summary>
    public sealed class CheckResult : Result
    {
        /// <summary>
        /// Существует ли чек в базе ФНС?
        /// </summary>
        public bool CheckExists { get; set; }

        /// <summary>
        /// Информация о документе, которая приходит из ФНС
        /// </summary>
        public Document Document { get; set; }

        internal CheckResult()
        { }
    }
}
