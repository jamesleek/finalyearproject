using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalYearProject.Models;
using Microsoft.AspNet.Identity;

namespace FinalYearProject.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        /// <summary>
        /// Gets all of the categories from the database and puts them into a lsit
        /// </summary>
        /// <returns>list of categories</returns>
        [Authorize]
        private IEnumerable<Category> GetMyCategories()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            return db.Categories.ToList();

        }

        /// <summary>
        /// Builds the categories table
        /// </summary>
        /// <returns>Partial view of categoires table</returns>
        [Authorize]
        public ActionResult BuildCategoriesTable()
        {
            return PartialView("_CategoriesTable", GetMyCategories());
        }

        /// <summary>
        /// Called when the user wishes to create a new category
        /// </summary>
        /// <returns>View for creating a new category</returns>
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Called when the user has inputted their new category this adds it to the database
        /// </summary>
        /// <param name="category"></param>
        /// <returns>The user back to the categories table</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryName,Keywords")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.NumberOfBugs = 0;
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        /// <summary>
        /// Gets the view for editting a category
        /// </summary>
        /// <param name="id"></param>
        /// <returns>view for edtting a category</returns>
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        /// <summary>
        /// sends the editted cateory to the database
        /// </summary>
        /// <param name="category"></param>
        /// <returns>the user back to the categories table</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryName,Keywords")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        /// <summary>
        /// This is called when the sweet alert gets confirmation from the user
        /// this delets the category and updates the table
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the view with the updated category table</returns>
        [HttpPost]
        [Authorize]
        public ActionResult AjaxDelete(int? id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return PartialView("_CategoriesTable", GetMyCategories());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
