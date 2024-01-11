namespace ToDoList_Utility.Models.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public int ErrorCode { get; }

        public UnauthorizedException( int errorCode) 
        {
            ErrorCode = errorCode;
        }
    }
}
