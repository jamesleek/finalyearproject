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
    public class BugReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BugReports
        public ActionResult Index()
        {
            return View();
        }

        private IEnumerable<BugReport> GetMyBugReports()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            return db.BugReports.ToList().Where(x => x.User == currentUser);

        }


        /*
         * For every category get each keyword
         * turn these keywords into a hash
         * Use this had to search through bug description
         * if a match set catgeory as one where the keyword is
         * return this category
         */
        private string GetACategory(string bugDescription)
        {

            string keywords;

            foreach (var item in db.Categories)
            {
                keywords = item.Keywords;
                List<string> listKeywords = keywords.Split(',').ToList<string>();
                foreach (var item2 in listKeywords)
                {
                   
                }
            }

            return "Default";
        }

        public ActionResult BuildBugReportTable()
        {
            return PartialView("_BugReportTable",GetMyBugReports());
        }

        // GET: BugReports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BugReport bugReport = db.BugReports.Find(id);
            if (bugReport == null)
            {
                return HttpNotFound();
            }
            return View(bugReport);
        }

        // GET: BugReports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BugReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BugDescription,Category,isResolved")] BugReport bugReport)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                bugReport.User = currentUser;
                db.BugReports.Add(bugReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bugReport);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AJAXCreate([Bind(Include = "Id,BugDescription,Category")] BugReport bugReport)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                bugReport.User = currentUser;
                bugReport.isResolved = false;
                bugReport.DateAdded = DateTime.Now;
                db.BugReports.Add(bugReport);
                db.SaveChanges();
                
            }

            return PartialView("_BugReportTable",GetMyBugReports());
        }   


        // GET: BugReports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BugReport bugReport = db.BugReports.Find(id);
            if (bugReport == null)
            {
                return HttpNotFound();
            }
            return View(bugReport);
        }

        // POST: BugReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BugDescription,Category,isResolved")] BugReport bugReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bugReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bugReport);
        }

        [HttpPost]
        public ActionResult AJAXEdit(int? id, bool value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BugReport bugReport = db.BugReports.Find(id);
            if (bugReport == null)
            {
                return HttpNotFound();
            }else
            {
                bugReport.isResolved = value;
                db.Entry(bugReport).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_BugReportTable", GetMyBugReports());
            }              
        }

        // GET: BugReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BugReport bugReport = db.BugReports.Find(id);
            if (bugReport == null)
            {
                return HttpNotFound();
            }
            return View(bugReport);
        }

        // POST: BugReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BugReport bugReport = db.BugReports.Find(id);
            db.BugReports.Remove(bugReport);
            db.SaveChanges();
            return RedirectToAction("Index");
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
