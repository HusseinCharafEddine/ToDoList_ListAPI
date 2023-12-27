using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Models;

namespace ToDoList_ListAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<ForgotPasswordResponseDTO> ForgotPassword(ForgotPasswordRequestDTO forgotPasswordRequestDTO);

    }
}
