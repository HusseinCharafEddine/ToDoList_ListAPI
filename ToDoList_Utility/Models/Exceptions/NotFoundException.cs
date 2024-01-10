namespace ToDoList_Utility.Models.Exceptions
{
    public class NotFoundException : Exception
    {
        public int ErrorCode { get; }

        public NotFoundException(int errorCode) 
        {
            ErrorCode = errorCode;
        }
    }
}
