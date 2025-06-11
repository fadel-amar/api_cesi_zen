namespace CesiZen_API.Helper.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    public class InternalErrorException : Exception
    {
        public InternalErrorException(string message) : base(message) { }
    }
}
