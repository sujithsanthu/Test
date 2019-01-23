using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using ExamDotNetMVC.Models;

namespace ExamDotNetMVC.Controllers
{
    public class AddQuestionsController : ApiController
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: api/AddQuestions
        public IQueryable<AddQuestion> GetAddQuestions()
        {
            return db.AddQuestions;
        }

        //// GET: api/AddQuestions/5
        [ResponseType(typeof(AddQuestion))]
        public IHttpActionResult GetAddQuestion(int id)
        {
            AddQuestion addQuestion = db.AddQuestions.Find(id);
            if (addQuestion == null)
            {
                return NotFound();
            }

            return Ok(addQuestion);
        }

        // GET: api/AddQuestions/5
        [ResponseType(typeof(AddQuestion))]
        public IQueryable<AddQuestion> GetAddQuestion(string Year,int level)
        {
          
            var Data = db.AddQuestions.Where(w => w.QLevel.Equals(level));
            return Data ;
        }

        // PUT: api/AddQuestions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAddQuestion(int id, AddQuestion addQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != addQuestion.Questionid)
            {
                return BadRequest();
            }

            db.Entry(addQuestion).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddQuestionExists(id))
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

        // POST: api/AddQuestions
        [ResponseType(typeof(AddQuestion))]
        public IHttpActionResult PostAddQuestion(AddQuestion addQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AddQuestions.Add(addQuestion);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = addQuestion.Questionid }, addQuestion);
        }

        // DELETE: api/AddQuestions/5
        [ResponseType(typeof(AddQuestion))]
        public IHttpActionResult DeleteAddQuestion(int id)
        {
            AddQuestion addQuestion = db.AddQuestions.Find(id);
            if (addQuestion == null)
            {
                return NotFound();
            }

            db.AddQuestions.Remove(addQuestion);
            db.SaveChanges();

            return Ok(addQuestion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AddQuestionExists(int id)
        {
            return db.AddQuestions.Count(e => e.Questionid == id) > 0;
        }
    }
}