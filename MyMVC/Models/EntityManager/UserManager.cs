﻿using System;
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
                    ProfileID = 0,
                    UserID = newUserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    CreatedBy = 1,
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = 1,
                    ModifiedDateTime = DateTime.Now,
                    AccountImage = user.AccountImage
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                int roleId = db.Role.First(r => r.RoleName == "Admin").RoleID;

                UserRole userRole = new UserRole
                {
                    UserID = newUserID,
                    LookUpRoleID = roleId,
                    IsActive = true,
                    CreatedBy = newUserID,
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = newUserID,
                    ModifiedDateTime = DateTime.Now
                };

                db.UserRole.Add(userRole);
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
                    existingUser.AccountImage = user.AccountImage;

                    UserRole userRole = db.UserRole.FirstOrDefault(ur => ur.UserID == existingUser.UserID);

                    if (userRole != null)
                    {
                        userRole.LookUpRoleID = user.RoleID;
                    }

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
                var users = from u in db.Users 
                            join us in db.SystemUsers 
                                on u.UserID equals us.UserID 
                            join ur in db.UserRole
                                on u.UserID equals ur.UserID
                            join r in db.Role
                                on ur.LookUpRoleID equals r.RoleID
                            select new { u, us, r, ur };

                list.Users = users.Select(records => new UserModel()
                {
                    LoginName = records.us.LoginName,
                    FirstName = records.u.FirstName,
                    LastName = records.u.LastName,
                    Gender = records.u.Gender,
                    CreatedBy = records.u.CreatedBy,
                    AccountImage = records.u.AccountImage ?? string.Empty,
                    RoleID = records.ur.LookUpRoleID,
                    RoleName = records.r.RoleName
                }).ToList();
            }

            return list;
        }
        public UserModel GetUser(string loginName)
        {
            using(MyDBContext db = new MyDBContext())
            {
                var users = from u in db.Users
                            join us in db.SystemUsers
                                on u.UserID equals us.UserID
                            join ur in db.UserRole
                                on u.UserID equals ur.UserID
                            join r in db.Role
                                on ur.LookUpRoleID equals r.RoleID
                            where us.LoginName == loginName
                            select new { u, us, r, ur };

                UserModel userModel = users.Select(r => new UserModel()
                {
                    LoginName = r.us.LoginName,
                    FirstName = r.u.FirstName,
                    LastName = r.u.LastName,
                    Gender = r.u.Gender,
                    CreatedBy = r.u.CreatedBy,
                    AccountImage = r.u.AccountImage ?? string.Empty,
                    RoleID = r.ur.LookUpRoleID,
                    RoleName = r.r.RoleName
                }).FirstOrDefault();

                return userModel;
            }
        }

        public bool IsLoginNameExist(string loginName)
        {
            using (MyDBContext db = new MyDBContext()) 
            {
                return db.SystemUsers.Where(u => u.LoginName.Equals(loginName)).Any();
            }
        }

        public string GetUserPassword(string loginName)
        {
            using (MyDBContext db = new MyDBContext())
            {
                var user = db.SystemUsers.Where(o => o.LoginName.ToLower().Equals(loginName.ToLower()));

                if (user.Any())
                    return user.FirstOrDefault().PasswordEncryptedText;
                else
                    return string.Empty;
            }
        }

        public bool IsUserInRole(string loginName, string roleName)
        {
            using (MyDBContext db = new MyDBContext())
            {
                SystemUsers su = db.SystemUsers.Where(o => o.LoginName.ToLower().Equals(loginName))?.FirstOrDefault();

                if(su != null)
                {
                    var roles = from ur in db.UserRole
                                join r in db.Role on ur.LookUpRoleID equals r.RoleID
                                where r.RoleName.Equals(roleName) && ur.UserID.Equals(su.UserID) select r.RoleName;

                    if (roles != null)
                        return roles.Any();
                }
            }
            return false;
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
