using System;
using System.Runtime.Serialization;

namespace Bank
{
    class AmountNegativeOrZeroException : Exception
    {
        public AmountNegativeOrZeroException(string message) : base(message)
        { }

        public AmountNegativeOrZeroException(string message, Exception innerException) : base(message, innerException)
        { }

        protected AmountNegativeOrZeroException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }

    class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message) : base(message)
        {
        }

        public InsufficientFundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InsufficientFundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
