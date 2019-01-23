using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ExamDotNetMVC.Models;

namespace ExamDotNetMVC.Controllers
{
    public class MarksDetailsController : ApiController
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: api/MarksDetails
        public IQueryable<MarksDetail> GetMarksDetails()
        {
            return db.MarksDetails;
        }

        // GET: api/MarksDetails/5
        [ResponseType(typeof(MarksDetail))]
        public IHttpActionResult GetMarksDetail(string id)
        {
            MarksDetail marksDetail = db.MarksDetails.Find(id);
            if (marksDetail == null)
            {
                return NotFound();
            }

            return Ok(marksDetail);
        }

        // PUT: api/MarksDetails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMarksDetail(string id, MarksDetail marksDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != marksDetail.Name)
            {
                return BadRequest();
            }

            db.Entry(marksDetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarksDetailExists(id))
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

        // POST: api/MarksDetails
        [ResponseType(typeof(MarksDetail))]
        public IHttpActionResult PostMarksDetail(MarksDetail marksDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MarksDetails.Add(marksDetail);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MarksDetailExists(marksDetail.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = marksDetail.Name }, marksDetail);
        }

        // DELETE: api/MarksDetails/5
        [ResponseType(typeof(MarksDetail))]
        public IHttpActionResult DeleteMarksDetail(string id)
        {
            MarksDetail marksDetail = db.MarksDetails.Find(id);
            if (marksDetail == null)
            {
                return NotFound();
            }

            db.MarksDetails.Remove(marksDetail);
            db.SaveChanges();

            return Ok(marksDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MarksDetailExists(string id)
        {
            return db.MarksDetails.Count(e => e.Name == id) > 0;
        }
    }
}