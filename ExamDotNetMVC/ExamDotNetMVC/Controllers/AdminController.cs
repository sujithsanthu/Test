using ExamDotNetMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;


namespace ExamDotNetMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View(db.AddQuestions.ToList());
        }

        public ActionResult DataView(string id)
        {
            Session["DataView"] = id;
            return RedirectToAction("Index1", "Admin");
        }

        public ActionResult Index1()
        {

            int Id = int.Parse(Session["DataView"].ToString());
            string Name= db.UserDetails.Where(w => w.UserId==Id).FirstOrDefault().Name;
            ViewBag.UserDetails = db.UserDetails.Where(w => w.Name.Equals(Name)).FirstOrDefault();

            return View(db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList());
        }

        public ActionResult ViewResult()
        {
            return View(db.MarksDetails.ToList());
        }

        [HttpPost]
        public ActionResult ViewResult(string Option, string Name)
        {
            if (Option == "Name")
            {
                return View(db.MarksDetails.Where(w => w.Name.Contains(Name.ToString())).ToList());
            }
            else if (Option == "AssignedMarks")
            {
                return View(db.MarksDetails.Where(w => w.AssignedMarks.ToString().Equals(Name.ToString())).ToList());
            }
            else if (Option == "Percentage")
            {
                return View(db.MarksDetails.Where(w => w.Percentage.ToString().Equals(Name.ToString())).ToList()); ;
            }
            else if (Option == "TotalMarks")
            {
                return View(db.MarksDetails.Where(w => w.TotalMarks.ToString().Equals(Name.ToString())).ToList());
            }
            else
            {
                return View(db.MarksDetails.ToList());
            }         
        }

        public ActionResult PostMarks(string Name)
        {
            double objMarks = 0;
            double AMarks = 0;
            var User = db.UserDetails.Where(w => w.Name.Equals(Name)).ToList();
            UserDetail userDetail=new UserDetail();
            foreach (var obj in User)
            {
                userDetail.Name = obj.Name;
                userDetail.Status = obj.Status;
                userDetail.Email = obj.Email;
                userDetail.Experience = obj.Experience;
                userDetail.UserId = obj.UserId;
                userDetail.Phone = obj.Phone;
            }

            if (userDetail.Status == "Checked")
            {
                return RedirectToAction("Verify");
            }

            userDetail.Status = "Checked";

            var Marks = db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList();
            MarksDetail Data=new MarksDetail();
            Data.Name = Name;
            foreach (var Mark in Marks)
            {               
                AMarks = AMarks + double.Parse(Mark.AMarks);
                objMarks = objMarks + double.Parse(Mark.Marks);

            }

            Data.AssignedMarks = AMarks;
            Data.TotalMarks = objMarks;
            Data.Percentage = (Data.AssignedMarks / Data.TotalMarks) * 100;
            var uri = "http://localhost:53349/api/MarksDetails/";
            var uri1 = "http://localhost:53349/api/UserDetails/"+userDetail.UserId;
            var json = JsonConvert.SerializeObject(Data);
            var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var json1 = JsonConvert.SerializeObject(userDetail);
            var request1 = new StringContent(json1.ToString(), Encoding.UTF8, "application/json");
            request1.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (HttpClient obj = new HttpClient())
            {
                obj.Timeout = TimeSpan.FromMilliseconds(15000);
                obj.MaxResponseContentBufferSize = 256000;
                //var response = obj.PostAsync(uri, request).Result;
                HttpResponseMessage response = null;
                HttpResponseMessage response1 = null;
                try
                {
                    response = obj.PostAsync(uri, request).Result;
                    response1 = obj.PutAsync(uri1, request1).Result;

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (response.IsSuccessStatusCode&& response1.IsSuccessStatusCode)
                {
                    return RedirectToAction("Verify");
                }
                throw new Exception(response.ReasonPhrase);

            }

            
        }

       

        public ActionResult Edit1(int? id)
        {
            string Name = Session["AName"].ToString();
            ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList();
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
        public ActionResult Edit1([Bind(Include = "Id,Name,QuestionNo,Answer,Marks,AMarks,Status")] ExamDetail examDetail)
        {
            if (examDetail.Answer == null)
            {
                examDetail.Answer = "";
            }
            ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(examDetail.Name)).ToList();
            //string Url = "http://localhost:53349/Admin/Index1/" + examDetail.Name;
            if (ModelState.IsValid)
            {
                var uri = "http://localhost:53349/api/ExamDetailsAPI/" + examDetail.Id;
                var json = JsonConvert.SerializeObject(examDetail);
                var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpClient obj = new HttpClient())
                {
                    obj.Timeout = TimeSpan.FromMilliseconds(15000);
                    obj.MaxResponseContentBufferSize = 256000;
                    //var response = obj.PostAsync(uri, request).Result;
                    HttpResponseMessage response = null;
                    try
                    {
                        response = obj.PutAsync(uri, request).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Edit1/" + examDetail.Id);
                    }
                    throw new Exception(response.ReasonPhrase);

                }
            }
            return RedirectToAction("Edit1/" + examDetail.Id);
        }

        public ActionResult Verify()
        {
            //ViewBag.Data = db.ExamDetails.GroupBy(w => w.Name).ToList();
            return View(db.UserDetails.ToList());
        }

        public ActionResult Check(string id)
        {
            Response.Cookies["AnswerCount"].Value = "0";
            ViewBag.ExamDetails = db.ExamDetails.Where(w => w.Name.Equals(id)).ToList();
            //Session["QName"] = id;
            return View();
        }

        public ActionResult VerifyAnswer()
        {
            string Name = @Session["QName"].ToString();
            Response.Cookies["AnswerCount"].Value = "0";
            ViewBag.ExamDetails= db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList();      
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult VerifyAnswer([Bind(Include = "Id,Name,QuestionNo,Answer,Marks,Status")] ExamDetail examDetail)
        {
            ViewBag.ExamDetails = db.ExamDetails.Where(w => w.Name.Equals(examDetail.Name)).ToList();
            string variable = Request.Cookies["AnswerCount"].Value;
            int CountData = (int.Parse(variable) + 1);
            Response.Cookies["AnswerCount"].Value = CountData.ToString();
            if (CountData > 10)
            {
                Response.Cookies["AnswerCount"].Value = "0";
                return View("Successful");
            }
            ExamDetail examDetailid = db.ExamDetails.Find(examDetail.Id);
            if (ModelState.IsValid)
            {
                db.Entry(examDetailid).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }
            return View();
        }



        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var uri = "http://localhost:53349/api/AddQuestions/" + id;

            using (HttpClient obj = new HttpClient())
            {
                obj.Timeout = TimeSpan.FromMilliseconds(15000);
                obj.MaxResponseContentBufferSize = 256000;
                //var response = obj.PostAsync(uri, request).Result;
                HttpResponseMessage response = null;
                try
                {
                    response = obj.GetAsync(uri).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (id == null)
                {
                    return HttpNotFound();
                }

                return View(JsonConvert.DeserializeObject<AddQuestion>(response.Content.ReadAsStringAsync().Result));
            }
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Questionid,Question,QLevel,Marks")] AddQuestion addQuestion)
        {


            var uri = "http://localhost:53349/api/AddQuestions/";
            var json = JsonConvert.SerializeObject(addQuestion);
            var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (HttpClient obj = new HttpClient())
            {
                obj.Timeout = TimeSpan.FromMilliseconds(15000);
                obj.MaxResponseContentBufferSize = 256000;
                //var response = obj.PostAsync(uri, request).Result;
                HttpResponseMessage response = null;
                try
                {
                    response = obj.PostAsync(uri, request).Result;

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                throw new Exception(response.ReasonPhrase);

            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var uri = "http://localhost:53349/api/AddQuestions/" + id;

            using (HttpClient obj = new HttpClient())
            {
                obj.Timeout = TimeSpan.FromMilliseconds(15000);
                obj.MaxResponseContentBufferSize = 256000;
                //var response = obj.PostAsync(uri, request).Result;
                HttpResponseMessage response = null;
                try
                {
                    response = obj.GetAsync(uri).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (id == null)
                {
                    return HttpNotFound();
                }

                return View(JsonConvert.DeserializeObject<AddQuestion>(response.Content.ReadAsStringAsync().Result));
            }
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Questionid,Question,QLevel,Marks")] AddQuestion addQuestion)
        {
            if (ModelState.IsValid)
            {
                var uri = "http://localhost:53349/api/AddQuestions/" + addQuestion.Questionid;
                var json = JsonConvert.SerializeObject(addQuestion);
                var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpClient obj = new HttpClient())
                {
                    obj.Timeout = TimeSpan.FromMilliseconds(15000);
                    obj.MaxResponseContentBufferSize = 256000;
                    //var response = obj.PostAsync(uri, request).Result;
                    HttpResponseMessage response = null;
                    try
                    {
                        response = obj.PutAsync(uri, request).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    throw new Exception(response.ReasonPhrase);

                }
            }
            return View(addQuestion);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var uri = "http://localhost:53349/api/AddQuestions/" + id;

            using (HttpClient obj = new HttpClient())
            {
                obj.Timeout = TimeSpan.FromMilliseconds(15000);
                obj.MaxResponseContentBufferSize = 256000;
                //var response = obj.PostAsync(uri, request).Result;
                HttpResponseMessage response = null;
                try
                {
                    response = obj.GetAsync(uri).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (id == null)
                {
                    return HttpNotFound();
                }

                return View(JsonConvert.DeserializeObject<AddQuestion>(response.Content.ReadAsStringAsync().Result));
            }
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var uri = "http://localhost:53349/api/AddQuestions/" + id;



            using (HttpClient obj = new HttpClient())
            {
                obj.Timeout = TimeSpan.FromMilliseconds(15000);
                obj.MaxResponseContentBufferSize = 256000;
                //var response = obj.PostAsync(uri, request).Result;
                HttpResponseMessage response = null;
                try
                {
                    response = obj.DeleteAsync(uri).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                throw new Exception(response.ReasonPhrase);

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
