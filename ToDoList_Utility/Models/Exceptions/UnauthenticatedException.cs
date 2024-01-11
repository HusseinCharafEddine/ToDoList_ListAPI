namespace ToDoList_Utility.Models.Exceptions
{
    public class UnauthenticatedException : Exception
    {
        public int ErrorCode { get; }

        public UnauthenticatedException( int errorCode) 
        {
            ErrorCode = errorCode;
        }
    }
}
