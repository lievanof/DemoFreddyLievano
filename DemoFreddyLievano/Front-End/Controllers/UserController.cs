using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Demo.DataAccessLayer.Factory;
using Demo.DataServices;
using System.Data.Entity;
using Frontend.Authorization;
using Frontend.Models;
using Demo.DataAccessLayer.Utils;
using Demo.DataAccessLayer.Context;
using Demo.Model;

namespace Frontend.Controllers
{
    [Autorize]
    public class UserController : BaseController
    {
        private DemoContext db = new DemoContext();
        private IUserService _userService;

        public UserController()
        {
            var dbf = new DbFactory();
            _userService = new UserService(dbf);
        }

        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Session).Include(u => u.UserData);
            return View(users.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Sessions, "Id", "Token");
            ViewBag.Id = new SelectList(db.UserData, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                #region Validate duplicated
                bool duplicated = db.Users.Any(u => u.Username == user.Username && u.Enabled) || db.Users.Any(u => u.UserData.Email == user.Email && u.Enabled);
                if (duplicated)
                {
                    Error("A user with the same Username or Email already exists");
                    return View(user);
                }
                #endregion
                var _user = new User();
                _user.Created = DateTime.Now;
                _user.Enabled = true;
                _user.FirstName = user.FirstName;
                _user.LastName = user.LastName;
                _user.Username = user.Username;
                _user.Password = PasswordGenerator.Encrypt(user.Password);
                _user.Rol = (user.Rol != Roles.CommonUser && user.Rol != Roles.SuperUser) ? Roles.CommonUser : user.Rol;
                _user.UserData = new UserData { BirthDate = DateTime.Now, Gender = user.Gender, Email = user.Email };
                int success = await _userService.Create(_user);
                if (success > 0)
                {
                    Success("User created successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Error on create user", "A error has ocurred when a user was created");
                    return View(user);
                }
            }

            ViewBag.Id = new SelectList(db.Sessions, "Id", "Token", user.Id);
            ViewBag.Id = new SelectList(db.UserData, "Id", "Id", user.Id);
            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            UserViewModel userModel = new UserViewModel();

            userModel.BirthDate = user.UserData.BirthDate;
            userModel.Created = user.Created;
            userModel.Email = user.UserData.Email;
            userModel.Enabled = user.Enabled;
            userModel.FirstName = user.FirstName;
            userModel.Gender = user.UserData.Gender;
            userModel.LastName = user.LastName;
            userModel.Password = "";
            userModel.PasswordConfirmation = "";
            userModel.Rol = user.Rol;
            userModel.Username = user.Username;

            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Sessions, "Id", "Token", user.Id);
            ViewBag.Id = new SelectList(db.UserData, "Id", "Id", user.Id);
            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel user)
        {
            var myUser = await _userService.Get(user.Id);

            if (myUser == null)
            {
                Error("User is invalid or doesn´t exists");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    #region Validate duplicated
                    bool duplicated = db.Users.Any(u => u.Id != user.Id && u.Username == user.Username && u.Enabled) || db.Users.Any(u => u.Id != user.Id && u.UserData.Email == user.Email && u.Enabled);
                    if (duplicated)
                    {
                        Error("A user with the same Username or Email already exists");
                        return View(user);
                    }
                    #endregion

                    myUser.Created = user.Created;
                    myUser.Enabled = user.Enabled;
                    myUser.FirstName = user.FirstName;
                    myUser.LastName = user.LastName;
                    myUser.Username = user.Username;
                    myUser.Password = PasswordGenerator.Encrypt(user.Password);
                    myUser.Rol = (user.Rol != Roles.CommonUser && user.Rol != Roles.SuperUser) ? Roles.CommonUser : user.Rol;
                    myUser.UserData.Email = user.Email;

                    int success = await _userService.Update(myUser);
                    if (success > 0)
                    {
                        Success("User updated successfully");
                        return RedirectToAction("Index");
                    }
                    else { throw new Exception("Cannot update user at this time"); }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message, ex.StackTrace);
                    Error("An error ocurred while updating the entries. Try again later.");
                    return View(user);
                }
            }
            ViewBag.Id = new SelectList(db.Sessions, "Id", "Token", user.Id);
            ViewBag.Id = new SelectList(db.UserData, "Id", "Id", user.Id);
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var old = await _userService.Get(id);

            if (old == null)
            {
                Error("User is invalid or doesn´t exists");
                return View(old);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    old.Enabled = false;
                    old.PasswordConfirmation = old.Password;

                    int success = await _userService.Update(old);
                    if (success > 0)
                    {
                        Success("User updated successfully");
                        return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else { throw new Exception("Cannot update user at this time"); }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message, ex.StackTrace);
                    throw new Exception("User cannot be deleted");
                }
            }
            ViewBag.Id = new SelectList(db.Sessions, "Id", "Token", old.Id);
            ViewBag.Id = new SelectList(db.UserData, "Id", "Id", old.Id);
            return Json(new { success = true, message = "" }, JsonRequestBehavior.AllowGet);                       
        }
    }
}