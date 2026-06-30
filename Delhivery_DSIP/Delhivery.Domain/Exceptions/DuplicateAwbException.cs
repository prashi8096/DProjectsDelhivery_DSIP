using System;

namespace Delhivery.Domain.Exceptions
{
    public class DuplicateAwbException : Exception
    {
        public DuplicateAwbException()
            : base("AWB Number already exists.")
        {
        }

        public DuplicateAwbException(string message)
            : base(message)
        {
        }
    }
}