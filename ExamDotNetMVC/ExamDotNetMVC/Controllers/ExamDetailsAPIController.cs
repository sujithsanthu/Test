using System;
using ExamDotNetMVC.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace ExamDotNetMVC.Controllers
{
    public class ExamDetailsAPIController : ApiController
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: api/ExamDetailsAPI
        public IQueryable<ExamDetail> GetExamDetails()
        {
            return db.ExamDetails;
        }

        // GET: api/ExamDetailsAPI/5
        [ResponseType(typeof(ExamDetail))]
        public IHttpActionResult GetExamDetail(int id)
        {
            ExamDetail examDetail = db.ExamDetails.Find(id);
            if (examDetail == null)
            {
                return NotFound();
            }

            return Ok(examDetail);
        }

        [ResponseType(typeof(MarksStatus))]
        public MarksStatus GetExamMarks(string Marks, int id)
        {
            ExamDetail examDetail = db.ExamDetails.Find(id);
            MarksStatus a=new MarksStatus();
            a.UnAttempted=db.ExamDetails.Where(w=>w.Name.Equals(examDetail.Name)).Count(s => s.Answer.Equals(string.Empty));
            a.Attempted = 10 - int.Parse(a.UnAttempted.ToString());

            return a;
        }

        // PUT: api/ExamDetailsAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutExamDetail(int id, ExamDetail examDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != examDetail.Id)
            {
                return BadRequest();
            }

            db.Entry(examDetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ExamDetailsAPI
        [ResponseType(typeof(ExamDetail))]
        public IHttpActionResult PostExamDetail(ExamDetail examDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExamDetails.Add(examDetail);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = examDetail.Id }, examDetail);
        }

        // DELETE: api/ExamDetailsAPI/5
        [ResponseType(typeof(ExamDetail))]
        public IHttpActionResult DeleteExamDetail(int id)
        {
            ExamDetail examDetail = db.ExamDetails.Find(id);
            if (examDetail == null)
            {
                return NotFound();
            }

            db.ExamDetails.Remove(examDetail);
            db.SaveChanges();

            return Ok(examDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ExamDetailExists(int id)
        {
            return db.ExamDetails.Count(e => e.Id == id) > 0;
        }
    }
}