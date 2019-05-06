using System;

namespace Core.Exception
{
    public class UniqueRecordAlreadyExistsException : ApplicationException
    {
        public UniqueRecordAlreadyExistsException()
        {
        }

        public UniqueRecordAlreadyExistsException(string message)
        : base(message)
        {
        }
    }
}
