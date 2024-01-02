namespace ToDoList_ListAPI.Models
{
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException(string message) : base(message)
        {
        }
    }
}
