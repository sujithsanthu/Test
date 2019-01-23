using ExamDotNetMVC.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ExamDotNetMVC.Controllers
{
    public class ExamDetailsController : Controller
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: ExamDetails
        public ActionResult Index(string id)
        {

            return View(db.ExamDetails.Where(w => w.Name.Equals(id)).ToList());
        }

        // GET: ExamDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamDetail examDetail = db.ExamDetails.Find(id);
            if (examDetail == null)
            {
                return HttpNotFound();
            }
            return View(examDetail);
        }

        // GET: ExamDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExamDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public void Create([Bind(Include = "Id,Name,QuestionNo,Answer,Marks,Status")] ExamDetail examDetail)
        {
            if (ModelState.IsValid)
            {
                db.ExamDetails.Add(examDetail);
                db.SaveChanges();

            }

            //return RedirectToAction("StartExam", "WebUsers");
        }

        // GET: ExamDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamDetail examDetail = db.ExamDetails.Find(id);
            if (examDetail == null)
            {
                return HttpNotFound();
            }
            return View(examDetail);
        }

        // POST: ExamDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Name,QuestionNo,Answer,Marks,Status")] ExamDetail examDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(examDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(examDetail);
        }

        // GET: ExamDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamDetail examDetail = db.ExamDetails.Find(id);
            if (examDetail == null)
            {
                return HttpNotFound();
            }
            return View(examDetail);
        }

        // POST: ExamDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamDetail examDetail = db.ExamDetails.Find(id);
            db.ExamDetails.Remove(examDetail);
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
