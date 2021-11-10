using MiniCrm.Models;
using MiniCrm.Repositories;
using System.Net;
using System.Web.Mvc;

namespace MiniCrm.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult UserList()
        {
            // Get all the users and populate the model.
            var model = UserRepository.GetAllUsers(string.Empty);

            return View("~/Views/MiniCrm/UserList.cshtml", model);
        }

        [HttpPost]
        public ActionResult UserList(string searchText)
        {
            // Execute a search on email and first name.
            var model = UserRepository.GetAllUsers(searchText);

            return View("~/Views/MiniCrm/UserList.cshtml", model);
        }

        [HttpGet]
        public ActionResult UserEdit(string id)
        {
            // Make sre we have an id.
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Define an empty model.
            var model = new UserModel();

            // If Id = 0 that means the Create User button was pressed.
            if (id != "0")
            {
                // Edit the user.
                model = UserRepository.GetUserById(id);
                ViewBag.Title = "Edit User";
            }
            else
            {
                // Create a new user.
                model.Id = "0";
                ViewBag.Title = "Create User";
            }

            // Make sure we have a model.
            if (model == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/MiniCrm/UserEdit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(UserModel model)
        {
            // Ensre the form has been filled ot completely.
            if (ModelState.IsValid)
            {
                // Update the user item.
                UserRepository.UpdateUser(model);

                // Go back to the user list.
                return Redirect("/UserList");
            }

            return View("~/Views/MiniCrm/UserEdit.cshtml", model);
        }

        [HttpGet]
        public ActionResult UserDelete(string id)
        {
            // Make sure we have an id.
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Delete the user item.
            UserRepository.DeleteUserById(id);

            return Redirect("/UserList");
        }

    }
}