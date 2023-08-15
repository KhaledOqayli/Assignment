using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Api.EFCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _context;

        public AuthController(UserContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDto userDto)
        {
            if (_context.Users.Any(u => u.Email == userDto.Email))
            {
                return Conflict(new { Message = "User already exists" });
            }

            string hashedPassword = HashPassword(userDto.Password);

            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Birthdate = userDto.Birthdate,
                Photo = userDto.Photo,
                Password = hashedPassword
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("update-graduate-program")]
        public IActionResult UpdateGraduateProgramInfo(UpdateGraduateProgramDto updateDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == updateDto.Email);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            user.LevelOfStudy = updateDto.LevelOfStudy;
            user.Program = updateDto.Program;
            user.FacultyDivision = updateDto.FacultyDivision;

            _context.SaveChanges();

            return Ok(new { Message = "Graduate program information updated successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel loginModel)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            bool isPasswordValid = VerifyPassword(loginModel.Password, user.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { Message = "Invalid password" });
            }


            return Ok(new { Message = "User logged in successfully" });
        }
        [HttpGet("get-user-data")]
        public IActionResult GetUserData(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                user.Birthdate,
                user.LevelOfStudy,
                user.Program,
                user.FacultyDivision,
                user.Photo 
            });
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInputPassword = HashPassword(password);
            return hashedInputPassword == hashedPassword;
        }

    }

    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public string Photo { get; set; }
        public string Password { get; set; }
        public string LevelOfStudy { get; set; }
        public string Program { get; set; }
        public string FacultyDivision { get; set; }
    }
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateGraduateProgramDto
    {
        public string Email { get; set; }
        public string LevelOfStudy { get; set; }
        public string Program { get; set; }
        public string FacultyDivision { get; set; }
    }
}