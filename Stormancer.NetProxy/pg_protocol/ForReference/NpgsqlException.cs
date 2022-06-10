
using System;
using System.Data.Common;


namespace Netproxy
{


    [Serializable]
    public class NpgsqlException : DbException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlException"/> class.
        /// </summary>
        public NpgsqlException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<string>Nothing</string> in Visual Basic) if no inner exception is specified.</param>
        public NpgsqlException(string? message, Exception? innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NpgsqlException(string? message)
            : base(message) { }




    }

}
