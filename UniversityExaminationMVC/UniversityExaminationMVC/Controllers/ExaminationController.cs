using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UniversityExaminationMVC.Models;

namespace UniversityExaminationMVC.Models
{
    public class ExaminationController : Controller
    {
        // GET: Examination
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateExam()
        {
            DataModelContext db = new DataModelContext();
            List<SelectListItem> Branch_Data = db.Branches.Select(x => new SelectListItem { Text = x.Name, Value = x.Branch_Id.ToString() }).ToList();
            ViewBag.Branch = new SelectList(Branch_Data, "Value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult CreateExam(string type,int sem,int BranchId, DateTime date)
        {

            int id;
            Exam_Work ew = new Exam_Work(type,sem.ToString(), BranchId, date);
            id=ew.Create_Exam();
            Session["eid"] = id;
            return RedirectToAction("Add_Paper");
        }
        [HttpGet]
        public ActionResult Add_Paper()
        {
            DataModelContext db = new DataModelContext();
            List<CheckModelPaper> list = db.Papers.Select(x => new CheckModelPaper {Id=x.Id,Name=x.Name,Date=x.paperDate.ToString(), check=false }).ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult Add_Paper(List<CheckModelPaper> list)
        {
            DataModelContext db = new DataModelContext();
            List<Paper> l = new List<Paper>();
            foreach (var i in list)
            {
                Paper p;
                if (i.check == true)
                {
                    p=db.Papers.SingleOrDefault(x=>x.Id==i.Id);
                    p.exam = db.Exams.Find(Session["eid"]);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Add_Paper");
        }
        public ActionResult DashBoard()
        {
            return RedirectToAction("Details", "Faculties", new { id = Session["UserID"] });
        }
        [HttpGet]
        public ActionResult Check()
        {
            DataModelContext db = new DataModelContext();
            return View();
        }
        [HttpPost]
        [ActionName("Check")]
        public ActionResult StudentList(int sroll,int lroll)
        {
            DataModelContext db = new DataModelContext();
            List<PaperScore> ps = new List<PaperScore>();
            ps = db.PaperScores.Select(x => x).ToList();
            ViewBag.data = ps;
            return View("StudentList");
        }

        public ActionResult Checking(int id)
        {
            TempData["id"] = id;
            DataModelContext db = new DataModelContext();
            string s = Encoding.ASCII.GetString(db.PaperScores.Find(id).Submition);
            
            String[] sparator = { "$" };
            String[] qa = s.Split(sparator, StringSplitOptions.RemoveEmptyEntries);
            ViewBag.ans = qa;
            return View();
        }
        [HttpPost]
        [ActionName("Checking")]
        public ActionResult Checkingscr(int scr)
        {
            DataModelContext db = new DataModelContext();
            PaperScore p = db.PaperScores.Find(TempData["id"]);
            p.Marks = scr;
            db.SaveChanges();
            return RedirectToAction("Check");
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
    }
}