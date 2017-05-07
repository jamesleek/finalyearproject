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
        /// <summary>
        /// Creates all the necessary .csv files needed for creating the bubble charts
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            CreateCSVFiles();
            CreateCSVFiles2();
            CreateCSVFiles3();
            return View();
        }

        /// <summary>
        /// Creates the categories.csv file by getting the category name and number of bugs in each
        /// </summary>
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
        }

        /// <summary>
        /// Creates the months.csv file by searching through each bug report and getting the datetime
        /// then looks at the month and increments the month accordingly
        /// </summary>
        public void CreateCSVFiles2()
        {
            string directory2 = Server.MapPath("~/");
            string filename2 = "months.csv";
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
        }

        /// <summary>
        /// Creates the BugsByUser.csv file by looping through each user and comparing the user name
        /// to the user that submitted the bug report. Counts the number for each user and writes it to the file
        /// </summary>
        public void CreateCSVFiles3()
        {
            string directory3 = Server.MapPath("~/");
            string filename3 = "BugsByUser.csv";
            string path3 = Path.Combine(directory3, filename3);

            var testest = db.BugReports.ToList();

            using (var sw3 = new StreamWriter(path3))
            {
                sw3.WriteLine("\"id\",\"value\"");
                foreach (var item4 in db.Users)
                {
                    int bugCount = 0;
                    foreach (var item3 in testest)
                    {
                        if (item4.UserName.ToString() == item3.UserName)
                        {
                            bugCount += 1;
                        }
                    }
                    sw3.WriteLine(string.Format("\"{0}\",\"{1}\"",
                        item4.UserName.ToString().Split('@')[0], bugCount));
                }
            }
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
