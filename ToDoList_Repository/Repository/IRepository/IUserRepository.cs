using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;

namespace ToDoList_Repository.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<ForgotPasswordResponseDTO> ForgotPassword(ForgotPasswordRequestDTO forgotPasswordRequestDTO);

    }
}
