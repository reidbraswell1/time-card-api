namespace TimeCard.Shared
{
    public class ErrorException
    {
        public ErrorExceptionMessage ErrorExceptionMessage { get; set; }
    }
    public class ErrorExceptionMessage
    {
        public string Message { get; set; }
    }
}