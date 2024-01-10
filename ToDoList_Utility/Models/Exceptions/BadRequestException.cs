namespace ToDoList_Utility.Models.Exceptions
{
    public class BadRequestException : Exception
    {
        public int ErrorCode { get; }

        public BadRequestException( int errorCode) 
        {
            ErrorCode = errorCode;
        }
    }
}
