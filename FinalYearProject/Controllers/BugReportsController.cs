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
            return db.BugReports.ToList();

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
            ulong siga = 0;
            ulong sigb = 0;
            ulong Q = 100007;
            ulong D = 256;
            string A = bugDescription.ToLower();
            string B;

            try
            {
                    foreach (var item in db.Categories)
                    {
                    System.Diagnostics.Debug.WriteLine(item.CategoryName);

                        keywords = item.Keywords;
                        List<string> listKeywords = keywords.Split(',').ToList<string>();
                    
                    foreach (var item2 in listKeywords)
                        {
                            B = item2.ToLower();
                            System.Diagnostics.Debug.WriteLine(B);
                            siga = 0;
                            sigb = 0;
                            Q = 100007;
                            D = 256;
                        for (int i = 0; i < B.Length; i++)
                            {
                                siga = (siga * D + (ulong)A[i]) % Q;
                                sigb = (sigb * D + (ulong)B[i]) % Q;
                            }
                            if (siga == sigb)
                            {
                                Console.WriteLine(string.Format(">>{0}<<{1}", A.Substring(0, B.Length), A.Substring(B.Length)));
                            db.Categories.Find(item.Id).NumberOfBugs += 1;
                            return item.CategoryName;
                            }

                            ulong pow = 1;
                            for (int k = 1; k <= B.Length - 1; k++)
                                pow = (pow * D) % Q;

                            for (int j = 1; j <= A.Length - B.Length; j++)
                            {
                                siga = (siga + Q - pow * (ulong)A[j - 1] % Q) % Q;
                                siga = (siga * D + (ulong)A[j + B.Length - 1]) % Q;
                                if (siga == sigb)
                                {
                                    if (A.Substring(j, B.Length) == B)
                                    {
                                        Console.WriteLine(string.Format("{0}>>{1}<<{2}", A.Substring(0, j),
                                                                                            A.Substring(j, B.Length),
                                                                                            A.Substring(j + B.Length)));
                                    db.Categories.Find(item.Id).NumberOfBugs += 1;
                                    return item.CategoryName;
                                        
                                    }
                                }
                            }
                        }
                    }
                
            }
            catch(Exception e)
            {
                
            }
            
            return "Default";
        }

        public ActionResult BuildBugReportTable()
        {
            return PartialView("_BugReportTable",GetMyBugReports());
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
                if (bugReport.BugDescription != null)
                {
                    string currentUserId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                    bugReport.User = currentUser;
                    bugReport.isResolved = false;
                    bugReport.DateAdded = DateTime.Now;
                    bugReport.Category = GetACategory(bugReport.BugDescription);
                    bugReport.UserName = currentUser.UserName.ToString();
                    db.BugReports.Add(bugReport);
                    db.SaveChanges();
                }
            }

            return PartialView("_BugReportTable",GetMyBugReports());
        }   

        
       public void DecrementBugs(int bugId)
        {
            BugReport bugreport = db.BugReports.Find(bugId);
            string category = bugreport.Category;

            foreach(var categorySearch in db.Categories)
            {
                if (categorySearch.CategoryName == category)
                {
                    categorySearch.NumberOfBugs -= 1;
                }
            }
        }

        public void IncrementBugs(int bugId)
        {
            BugReport bugreport = db.BugReports.Find(bugId);
            string category = bugreport.Category;

            foreach (var categorySearch in db.Categories)
            {
                if (categorySearch.CategoryName == category)
                {
                    categorySearch.NumberOfBugs += 1;
                }
            }
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
                int id2 = id ?? default(int);
                bugReport.isResolved = value;
                if (value)
                {                    
                    DecrementBugs(id2);
                }
                else
                {
                    IncrementBugs(id2);
                }
                
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
            DecrementBugs(id);
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
