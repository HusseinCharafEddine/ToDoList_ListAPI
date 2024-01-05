
namespace ToDoList_Utility.Models
{
    public class LocalUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string ?ResetToken { get; internal set; }
        public DateTime ?ResetTokenExpiration { get; internal set; }
    }
}
