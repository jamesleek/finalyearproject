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
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the bug reports from the database
        /// </summary>
        /// <returns>A list of all bug reports</returns>
        private IEnumerable<BugReport> GetMyBugReports()
        {
            return db.BugReports.ToList();

        }


        /// <summary>
        /// This implements the Rabin-Karp algorithm to search for keywords from a category in the description of
        /// a bug report
        /// </summary>
        /// <param name="bugDescription"></param>
        /// <returns>the category name where the keyword is found in the bug description</returns>
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

        /// <summary>
        /// This method is called from the ajax request to submit a new bug report
        /// </summary>
        /// <param name="bugReport"></param>
        /// <returns>Updates the bug report table</returns>
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Called when a bug is marked as resolved or deleted
        /// so reduces the category the bug report was in by 1
        /// </summary>
        /// <param name="bugId"></param>
        public void DecrementBugs(int bugId)
        {
            BugReport bugreport = db.BugReports.Find(bugId);
            string category = bugreport.Category;

            foreach(var categorySearch in db.Categories)
            {
                if (categorySearch.CategoryName == category)
                {
                    if (categorySearch.NumberOfBugs > 0)
                    {
                        categorySearch.NumberOfBugs -= 1;
                    }
                    
                }
            }
        }

        /// <summary>
        /// When a bug report is put into a category that categories bug count is increased by 1
        /// </summary>
        /// <param name="bugId"></param>
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

        /// <summary>
        /// Called when the is resolved checkbox is clicked
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns>updated bug report table</returns>
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// This is called when the sweet alert gets confirmation from the user
        /// this delets the bug report and updates the table
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the view with the updated bug report table</returns>
        [HttpPost]
        [Authorize]
        public ActionResult AjaxDelete(int? id)
        {
            BugReport bugReport = db.BugReports.Find(id);
            int id2 = id ?? default(int);
            DecrementBugs(id2);
            db.BugReports.Remove(bugReport);
            db.SaveChanges();
            return PartialView("_BugReportTable", GetMyBugReports());
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
