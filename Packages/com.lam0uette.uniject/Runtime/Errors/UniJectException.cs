using System;

namespace LaM0uette.UniJect
{
    public abstract class UniJectException : Exception
    {
        protected UniJectException(string message)
            : base(message)
        {
        }

        protected UniJectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
