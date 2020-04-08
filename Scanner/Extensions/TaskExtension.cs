using Scanner.Extensions.Interfaces;
using System;
using System.Threading.Tasks;

namespace Scanner.Extensions
{
    public static class TaskExtension
    {
        /// <summary>
        /// https://johnthiriet.com/mvvm-going-async-with-async-command/
        /// </summary>
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                //TODO: Вставить по умолчанию свой обработчик, а не замалчивать ошибку
                handler?.HandleError(ex);
            }
        }
    }
}
