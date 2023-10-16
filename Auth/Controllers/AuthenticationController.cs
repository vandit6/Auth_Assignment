////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;
////using Auth.Models;
////using Microsoft.AspNetCore.Mvc;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Auth.Models;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
//using System.Text.RegularExpressions;
//using BCrypt.Net;

//namespace Auth.Controllers
//{
//    public class AuthenticationController : Controller
//    {
//        private readonly AuthContext Auth;
//        public AuthenticationController(AuthContext _Auth)
//        {
//            Auth = _Auth;
//        }
//        public IActionResult Login()
//        {
//            return View();
//        }
//        [HttpPost]
//        public IActionResult Login(Users input)
//        {
//            var regUser = auth.Users.FirstOrDefault(d => d.Email == input.Email);
            //if (regUser == null)
            //    return BadRequest("No registered user of this Email");
            //if (!BCrypt.Net.BCrypt.Verify(input.Password, regUser.Password))
            //    return BadRequest("Wrong Password");
            //return RedirectToAction("Welcome");

//        }

//        public IActionResult Welcome()
//        {
//            return View();
//        }
//        [HttpPost]
//        public IActionResult Register(Users input)
//        {
////            var dupUsername = auth.Users.Any(d => d.UserName == input.UserName);
//var dupEmail = auth.Users.Any(d => d.Email == input.Email);
//EmailAddressAttribute e = new EmailAddressAttribute();
//            if (!e.IsValid(input.Email))
//                return BadRequest("Invalid email address");
//            if (dupEmail || dupUsername)
//                return BadRequest("Username or Email already exists");
//            if (input.Password.Length< 6)
//                return BadRequest("Password less than 6 letters");
//            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$";
//            if (!Regex.IsMatch(input.Password, pattern))
//                return BadRequest("Password must contain at least one capital letter, one number and one special character");
//input.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
//            auth.Users.Add(input);
//            auth.SaveChanges();
//            return RedirectToAction("Welcome");
//        }

//        public IActionResult Register()
//        {
//            return View();
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using BCrypt.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Auth.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AuthContext auth;
        public AuthenticationController(AuthContext _auth)
        {
            auth = _auth;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Welcome()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Users input)
        {
            var dupUsername = auth.Users.Any(d => d.UserName == input.UserName);
            var dupEmail = auth.Users.Any(d => d.Email == input.Email);
            EmailAddressAttribute e = new EmailAddressAttribute();
            if (!e.IsValid(input.Email))
                return BadRequest("Invalid email address");
            if (dupEmail || dupUsername)
                return BadRequest("Username or Email already exists");
            if (input.Password.Length < 6)
                return BadRequest("Password less than 6 letters");
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$";
            if (!Regex.IsMatch(input.Password, pattern))
                return BadRequest("Password must contain at least one capital letter, one number and one special character");
            input.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            auth.Users.Add(input);
            auth.SaveChanges();
            return RedirectToAction("Welcome");
        }
        //[HttpPost]
        //public IActionResult Login(Users input)
        //{
        //    var regUser = auth.Users.FirstOrDefault(d => d.Email == input.Email);
        //    if (regUser == null)
        //        return BadRequest("No registered user of this Email");
        //    if (!BCrypt.Net.BCrypt.Verify(input.Password, regUser.Password))
        //        return BadRequest("Wrong Password");
        //    return RedirectToAction("Welcome");
        //}
        [HttpPost]
        public IActionResult Login(Users user)
        {
            var storedUser = auth.Users.FirstOrDefault(d => d.Email == user.Email);

            if (storedUser != null && BCrypt.Net.BCrypt.Verify(user.Password, storedUser.Password))
            {
                // User authentication is successful. Generate a JWT token.
                var securityKey = "This_is_our_security_key"; // Your security key
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
                var claims = new Claim[]
{
    new Claim(ClaimTypes.Name, user.Email)  ,
    new Claim(ClaimTypes.Role,"Userr")
};

                var token = new JwtSecurityToken(
                    issuer: "smesk.in",
                    audience: "readers",
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials,
                    claims: claims
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // If the response is successful, redirect to the "Welcome" page
                return Ok(tokenString);
                    return RedirectToAction("Welcome");
                
            }
            else if (storedUser != null)
            {
                // User exists, but the password is incorrect
                return Ok("Invalid password");
            }
            else
            {
                // User does not exist, so prompt them to register
                TempData["Message"] = "User is not registered. Please register first.";
                return RedirectToAction("Register"); // Assuming you have a "Register" action
            }
        }
    }
}






