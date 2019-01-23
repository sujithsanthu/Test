using ExamDotNetMVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using System.Web.Security;
using System.Web;

namespace ExamDotNetMVC.Controllers
{
    [Authorize]
    public class WebUsersController : Controller
    {
        private ExamDotNetMVCContext db = new ExamDotNetMVCContext();

        // GET: WebUsers
        public ActionResult Index()
        {
            return View();
        }

        // GET: WebUsers/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var uri = "http://localhost:53349/api/UserDetails/" + id;

        //    using (HttpClient obj = new HttpClient())
        //    {
        //        obj.Timeout = TimeSpan.FromMilliseconds(15000);
        //        obj.MaxResponseContentBufferSize = 256000;
        //        //var response = obj.PostAsync(uri, request).Result;
        //        HttpResponseMessage response = null;
        //        try
        //        {
        //            response = obj.GetAsync(uri).Result;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }

        //        if (id == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(JsonConvert.DeserializeObject<UserDetail>(response.Content.ReadAsStringAsync().Result));
        //    }
        //}

        // GET: WebUsers/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult ExamDetails()
        {
            var Name = System.Web.HttpContext.Current.User.Identity.Name;
            ViewBag.Name = Name;

            var a=db.Status.Where(w => w.Name.Equals(Name)).FirstOrDefault();
            //ExamDetail examDetail = (ExamDetail)db.ExamDetails.Where(w=>w.Name.Equals(Name));
            //if (examDetail == null)
            //{
            //    return HttpNotFound();
            //}
            //string Name = System.Web.HttpContext.Current.User.Identity.Name;
            if (a.IsEntered == false)
            {
                ViewBag.Name = Name;
                ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList();
                ExamDetail examDetail = db.ExamDetails.Where(w => w.Name.Equals(Name)).FirstOrDefault();
                ViewBag.Initial = examDetail;
                a.IsEntered = true;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                return View(examDetail);
            }

            return View("Final");

        }

       

        public ActionResult Final(string Name)
        {
            ViewBag.Name = Name;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult FeedBack( string Comment)
        {
            string Name = System.Web.HttpContext.Current.User.Identity.Name;
            string _connStr = ConfigurationManager.ConnectionStrings["ExamDotNetMVCContext"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                string sql = "Insert into FeedBack (UserName,Comment) Values(@UserName,@Comment)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Comment", Comment);
                    cmd.Parameters.AddWithValue("@UserName", Name);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            Response.Cookies["__RequestVerificationToken"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies[".AspNet.ApplicationCookie"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index");
        }


        // POST: WebUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Name,Experience,Email,Phone,Status")] UserDetail userDetail)
        {
            if (string.IsNullOrWhiteSpace(userDetail.Name))
            {
                ModelState.AddModelError("Name", "UserName is required Field");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }
            if (userDetail.Experience == 0)
            {
                ModelState.AddModelError("Experience", "Experience Must be selected");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }
            if (string.IsNullOrWhiteSpace(userDetail.Email))
            {
                ModelState.AddModelError("Email", "Email is required Field");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }

            if (string.IsNullOrWhiteSpace(userDetail.Phone))
            {
                ModelState.AddModelError("Phone", "PhoneNo is required Field");
                ModelState.AddModelError("", "Warning!!");
                return View();

            }

            int count = 0;
            var name = userDetail.Name;
            ViewBag.Exp = userDetail.Experience;
            Session["Name"] = userDetail.Name;
            Session["Experience"] = userDetail.Experience;
            var uri = "http://localhost:53349/api/UserDetails/";
            var json = JsonConvert.SerializeObject(userDetail);
            var request = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            request.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            List<UserDetail> distinctWords = db.UserDetails.ToList();
            foreach (var user in distinctWords)
            {
                if (user.Name.Equals(userDetail.Name) || user.Email.Equals(userDetail.Email) || user.Phone.Equals(userDetail.Phone))
                {
                    count = 1;

                }
            }

            if (count != 0)
            {
                ModelState.AddModelError("", "May be UserName,Email or PhoneNo Already Exists...!");
                return View();
                //return RedirectToActionPermanent("Create", "WebUsers", new { Name = name });
            }
            else
            {


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
                        var ran = new Random();
                        var questions = db.AddQuestions.Where(w => w.QLevel.Equals(userDetail.Experience)).Take(10).ToList()
                            .OrderBy(q => ran.Next()).ToList();
                        foreach (var question in questions)
                        {
                            ExamDetail exam = new ExamDetail();
                            exam.Name = userDetail.Name;
                            exam.QuestionNo = question.Question;
                            exam.Answer = "";
                            exam.Marks = question.Marks.ToString();
                            exam.Status = "UnChecked";
                            exam.AMarks = "0";
                            if (ModelState.IsValid)
                            {
                                db.ExamDetails.Add(exam);
                                db.SaveChanges();

                            }

                        }
                        return RedirectToActionPermanent("ExamDetails", "WebUsers", new { Name = name });

                    }
                    return RedirectToActionPermanent("ExamDetails", "WebUsers", new { Name = name });
                }

            }

        }

        public ActionResult Edit1(int? id)
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
            string Name = System.Web.HttpContext.Current.User.Identity.Name;
            ViewBag.Name = Name;
            ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(Name)).ToList();
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
            ViewBag.Name = System.Web.HttpContext.Current.User.Identity.Name;
            if (examDetail.Answer == null)
            {
                examDetail.Answer = "";
            }
            ViewBag.ExamDetail = db.ExamDetails.Where(w => w.Name.Equals(examDetail.Name)).ToList();

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
                    else
                    {
                        return RedirectToAction("Final");
                    }
                    throw new Exception(response.ReasonPhrase);

                }
            }
            return RedirectToAction("Edit1/" + examDetail.Id);
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
