using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoList_ListAPI.Data;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Repository.IRepository;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace ToDoList_ListAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");

        }
        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
            && u.Password == loginRequestDTO.Password);
            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                }; ;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role , user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };
            return loginResponseDTO;
        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            LocalUser user = new()
            {
                UserName = registrationRequestDTO.UserName,
                Password = registrationRequestDTO.Password,
                Name = registrationRequestDTO.Name,
                Role = registrationRequestDTO.Role,
                Email = registrationRequestDTO.Email,
            };
            _db.LocalUsers.Add(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;
        }
        public async Task<ForgotPasswordResponseDTO> ForgotPassword (ForgotPasswordRequestDTO forgotPasswordRequestDTO)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.Email.ToLower() == forgotPasswordRequestDTO.Email.ToLower());
            if (user == null)
            {
                return new ForgotPasswordResponseDTO()
                {
                    User = null,
                    Email = ""
                }; ;
            }
            var resetToken = GenerateResetToken();

            user.ResetToken = resetToken;
            user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1); 

            await _db.SaveChangesAsync();

            await SendResetEmail(user.Email, user.UserName, resetToken);
            String newPassword = "test1234";
            user.Password = newPassword;
            await _db.SaveChangesAsync();
            return new ForgotPasswordResponseDTO()
            {
                User = user,
                Email = user.Email 
            };


        }
        private async Task SendResetEmail(string userEmail, string userName, string resetToken)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("hu.sharafeddine@gmail.com"); 
                mail.To.Add(userEmail);
                mail.Subject = "Password Reset";
                mail.Body = $"Click the following link to reset your password: www.facebook.com/{resetToken}";

                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential("hu.sharafeddine@gmail.com", "zyly odfu yoas bhqc");
                smtpServer.EnableSsl = true;

                await smtpServer.SendMailAsync(mail);
                Console.WriteLine("Password reset email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

        }

        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
