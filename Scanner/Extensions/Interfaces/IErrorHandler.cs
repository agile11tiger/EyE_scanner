using System;

namespace Scanner.Extensions.Interfaces
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
