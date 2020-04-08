using System.Net;

namespace VerificationCheck.Core.Results
{
    /// <summary>
    /// Класс, используемый для представления ответа, полученного от ФНС
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Стандартный HTTP код
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Ответ, полученный от ФНС. Может отсутствовать.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Успешно ли выполнен запрос?
        /// </summary>
        public bool IsSuccess { get; set; }

        internal Result()
        { }

        internal Result(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
