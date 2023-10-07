using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using MyMVC.Models.DB;
using MyMVC.Models.ViewModel;

namespace MyMVC.Models.EntityManager
{
    public class UserManager
    {
        public void AddUserAccount(UserModel user)
        {
            using (MyDBContext db = new MyDBContext()) 
            {
                SystemUsers newSysUser = new SystemUsers
                {
                    LoginName = user.LoginName,
                    CreatedBy = 1,
                    PasswordEncryptedText = GetMd5Hash(user.Password),
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = 1,
                    ModifiedDateTime = DateTime.Now
                };

                db.SystemUsers.Add(newSysUser);
                db.SaveChanges();

                int newUserID = db.SystemUsers.First(u => u.LoginName.Equals(newSysUser.LoginName)).UserID;

                Users newUser = new Users
                {
                    UserID = newUserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = "1",
                    CreatedBy = 1,
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = 1,
                    ModifiedDateTime = DateTime.Now,
                    AccountImage = user.AccountImage
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public void UpdateUserAccount(UserModel user)
        {
            using (MyDBContext db = new MyDBContext())
            {
                SystemUsers existingSysUser = db.SystemUsers.FirstOrDefault(u => u.LoginName == user.LoginName);
                Users existingUser = db.Users.FirstOrDefault(u => u.UserID == existingSysUser.UserID);

                if(existingSysUser != null && existingSysUser != null)
                {
                    existingSysUser.ModifiedBy = 1;
                    existingSysUser.ModifiedDateTime = DateTime.Now;

                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Gender = user.Gender;

                    db.SaveChanges();
                }
            }
        }

        //public List<Users> GetAllUsers()
        //{
        //    List<Users> users = new List<Users>();

        //    using (MyDBContext db = new MyDBContext())
        //    {
        //        users = db.Users.ToList();
        //    }

        //    return users;
        //}

        public UsersModel GetAllUsers()
        {
            UsersModel list = new UsersModel();

            using(MyDBContext db = new MyDBContext())
            {
                var users = from u in db.Users join us in db.SystemUsers on u.UserID equals us.UserID select new { u, us };

                list.Users = users.Select(records => new UserModel()
                {
                    LoginName = records.us.LoginName,
                    FirstName = records.u.FirstName,
                    LastName = records.u.LastName,
                    Gender = records.u.Gender,
                    CreatedBy = records.u.CreatedBy,
                    AccountImage = records.u.AccountImage ?? string.Empty
                }).ToList();
            }

            return list;
        }

        public bool IsLoginNameExist(string loginName)
        {
            using (MyDBContext db = new MyDBContext()) 
            {
                return db.SystemUsers.Where(u => u.LoginName.Equals(loginName)).Any();
            }
        }

        public string GetMd5Hash(string text)
        {
            using MD5 hash = MD5.Create();
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
