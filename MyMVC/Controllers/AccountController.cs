using Microsoft.AspNetCore.Mvc;
using MyMVC.Models.EntityManager;
using MyMVC.Models.ViewModel;

namespace MyMVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult Users()
        {
            UserManager um = new UserManager();
            UsersModel user = um.GetAllUsers();

            return View(user);
        }


        [HttpPost]
        public ActionResult SignUp(UserModel user)
        {
            if(ModelState.IsValid)
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
        public async Task<ActionResult> Update([FromBody] UserModel user)
        {
            UserManager um = new UserManager();
            if(um.IsLoginNameExist(user.LoginName))
            {
                um.UpdateUserAccount(user);
                return RedirectToAction("Index");
            }
            return RedirectToAction("LoginNameNotFound");
        }

        [HttpGet]
        public ActionResult GetUsers() 
        {
            var users = new UserManager().GetAllUsers();
            return View();
        }
    }
}
