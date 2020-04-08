using System.Runtime.Serialization;

namespace VerificationCheck.Core.Results
{
    /// <summary>
    /// Документ, который приходит из ФНС
    /// </summary>
    [DataContract]
    public class Document
    {
        /// <summary>
        /// Внутренняя информация о чеке
        /// </summary>
        [DataMember]
        public Check Check { get; set; }
    }
}
