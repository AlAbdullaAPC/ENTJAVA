using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyMVC.Models.DB;
using MyMVC.Models.EntityManager;
using MyMVC.Models.ViewModel;
using MyMVC.Security;
using System.Security.Claims;

namespace MyMVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [AuthorizeRole("Admin")]
        public ActionResult Users()
        {
            UserManager um = new UserManager();
            UsersModel user = um.GetAllUsers();

            return View(user);
        }
        public ActionResult MyProfile()
        {
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            UserModel user = new UserModel();

            if (userClaim != null)
            {
                UserManager um = new UserManager();
                user = um.GetUser(userClaim.Value);
            }
            return View(user);
        }


        [HttpPost]
        public ActionResult SignUp(UserModel user)
        {
            ModelState.Remove("AccountImage");
            ModelState.Remove("RoleName");

            if (ModelState.IsValid)
            {
                UserManager um = new UserManager();
                if (!um.IsLoginNameExist(user.LoginName))
                {
                    um.AddUserAccount(user);
                    return RedirectToAction("", "Home");
                }
                else
                    ModelState.AddModelError("", "Login Name already taken.");
            }
            return View();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UserModel userData)
        {
            UserManager um = new UserManager();
            if(um.IsLoginNameExist(userData.LoginName))
            {
                um.UpdateUserAccount(userData);
                return RedirectToAction("Index");
            }
            return RedirectToAction("LoginNameNotFound");
        }

        [HttpPost]
        public ActionResult Login(UserLoginModel ulm)
        {
            if(ModelState.IsValid)
            {
                UserManager um = new UserManager();

                if(string.IsNullOrEmpty(ulm.Password))
                {
                    ModelState.AddModelError("", "The user login or password provided is incorrect.");
                }
                else
                {
                    if(um.GetUserPassword(ulm.LoginName).Equals(um.GetMd5Hash(ulm.Password)))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, ulm.LoginName)
                        };

                        var userIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

                        HttpContext.SignInAsync(principal);

                        return RedirectToAction("MyProfile");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The password provided is incorrect.");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult GetUsers()
        {
            var users = new UserManager().GetAllUsers();
            return View();
        }
    }
}
