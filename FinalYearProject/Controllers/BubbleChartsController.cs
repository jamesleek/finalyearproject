using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalYearProject.Models;
using System.Web.UI.WebControls;
using System.IO;

namespace FinalYearProject.Controllers
{
    public class BubbleChartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BubbleCharts
        public ActionResult Index()
        {
            CreateCSVFiles();
            return View(db.Categories.ToList());
        }


        //Create .CSV files for each charrt
        public void CreateCSVFiles()
        {

            string directory = Server.MapPath("~/");
            string filename = "categories.csv";
            string path = Path.Combine(directory, filename);


            using (var sw = new StreamWriter(path))
            {
                sw.WriteLine("\"id\",\"value\"");

                foreach (var item in db.Categories)
                {
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        item.CategoryName, item.NumberOfBugs));
                }

            }

            string directory2 = Server.MapPath("~/");
            string filename2= "months.csv";
            string path2 = Path.Combine(directory2, filename2);
            int jan = 0;
            int feb = 0;
            int mar = 0;
            int apr = 0;
            int may = 0;
            int jun = 0;
            int jul = 0;
            int aug = 0;
            int sep = 0;
            int oct = 0;
            int nov = 0;
            int dec = 0;


            using (var sw2 = new StreamWriter(path2))
            {
                sw2.WriteLine("\"id\",\"value\"");

                foreach (var item2 in db.BugReports)
                {
                    switch (item2.DateAdded.Month)
                    {
                        case (1):
                            jan += 1;
                            break;
                        case (2):                            
                            feb += 1;
                            break;
                        case (3):                           
                            mar += 1;
                            break;
                        case (4):                            
                            apr += 1;
                            break;
                        case (5):
                            may += 1;
                            break;
                        case (6):
                            jun += 1;
                            break;
                        case (7):
                            jul += 1;
                            break;
                        case (8):
                            aug += 1;
                            break;
                        case (9):
                            sep += 1;
                            break;
                        case (10):
                            oct += 1;
                            break;
                        case (11):
                            nov += 1;
                            break;
                        case (12):
                            dec += 1;
                            break;
                        default:
                            break;
                    }
                }

                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "Janurary", jan));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "February", feb));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "March", mar));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "April", apr));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "May", may));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "June", jun));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "July", jul));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "Aug", aug));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "September", sep));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "October", oct));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "November", nov));
                sw2.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        "December", dec));

            }

            string directory3 = Server.MapPath("~/");
            string filename3 = "BugsByUser.csv";
            string path3 = Path.Combine(directory3, filename3);
            

            using(var sw3 = new StreamWriter(path3))
            {
                sw3.WriteLine("\"id\",\"value\"");

                foreach(var item3 in db.BugReports)
                {
                    
                    sw3.WriteLine(string.Format("\"{0}\",\"{1}\"",
                         item3.User.Email.ToString(), 1));
                }
            }


        }

        // GET: BubbleCharts/Details/5
        public ActionResult Details(int? id)
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

        // GET: BubbleCharts/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: BubbleCharts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryName,Keywords,NumberOfBugs")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: BubbleCharts/Edit/5
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

        // POST: BubbleCharts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryName,Keywords,NumberOfBugs")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: BubbleCharts/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: BubbleCharts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
