using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Extra;
using Server.Models;
using Server.Models.DTOs;
using Server.Repository.IRepository;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Repository
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public UserAccountRepository(AppDbContext appDb, IConfiguration configuration)
        {
            _appDbContext = appDb;
            _configuration = configuration;
        }

        public async Task<GeneralResponse> RegisterAsync(Register user)
        {
            if (user is null)
                return new GeneralResponse(false, "Model is empty");
            var checkUser = FindUserByEmail(user.Email);
            if (checkUser == true) return new GeneralResponse(false, "User registered Already");

            //Save User
            var applicatioinUser = await AddToDatabase(new ApplicationUser()
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            });

            // check , create and assign role 
            var checkAdminRole = await _appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));

            if (checkAdminRole == null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicatioinUser.Id });
                return new GeneralResponse(true, "Account created!");
            }

            var checkUserRole = await _appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicatioinUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicatioinUser.Id });
            }
            return new GeneralResponse(true, "Account Created!");
        }
        public async Task<LoginResponse> LoginAsync(Login user)
        {
            if (user is null) return new LoginResponse(false, "Model is Empty");

            var applicationUser = FindUserByEmail(user.Email);
            if (applicationUser == false) return new LoginResponse(false, "User Not Found");

            var appUser = _appDbContext.ApplicationUsers.FirstOrDefault(_x => _x.Email.ToLower() == user.Email.ToLower());
            if (!BCrypt.Net.BCrypt.Verify(user.Password, appUser.Password))
                return new LoginResponse(false, "Email/Password Not Valid");

            var getUserRole = _appDbContext.UserRoles.FirstOrDefault(_x => _x.UserId == appUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "user role not found");

            var getRoleName = _appDbContext.SystemRoles.FirstOrDefault(_x => _x.Id == getUserRole.RoleId);
            if (getRoleName is null) return new LoginResponse(false, "user role not found");

            string jwtToken = GenerateToken(appUser, getRoleName!.Name);

            return new LoginResponse(true, "Login successfully", jwtToken);
        }
        public string GenerateToken(ApplicationUser user, string role)
        {

            var claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("UserName", user.Fullname),
                        new Claim("Email", user.Email),
                    };

            claims.Add(new Claim(ClaimTypes.Role, role));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }
        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = _appDbContext.Add(model!);
            await _appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }
        public bool FindUserByEmail(string email)
        {
            var IsEmal = _appDbContext.ApplicationUsers.FirstOrDefault(_x => _x.Email!.ToLower()!.Equals(email!.ToLower()));
            if (IsEmal != null) return true;
            return false;
        }

    }
}
