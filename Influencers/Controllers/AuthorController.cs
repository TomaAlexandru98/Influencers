using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Influencers.BusinessLogic.Services;
using Influencers.BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Influencers.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AuthorService authorService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AuthorController(AuthorService authorService, IWebHostEnvironment webHostEnvironment)
        {
            this.authorService = authorService;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            try
            {
                return View(authorService.GetAll().OrderByDescending(author => author.Votes));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                return View(authorService.GetAuthorAndArticles(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult FilterByName(string searchAuthor)
        {
            try
            {
                return View("Index", authorService.FilterByName(searchAuthor).OrderBy(author => author.LastName));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public JsonResult GetAllAsJson()
        {
            return Json(authorService.GetAllAsString().ToList());
        }

        public IActionResult SendEmail(int id)
        {
            var authorDb = authorService.GetById(id);
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(authorDb.Email);
            mailMessage.Subject = "Influencers";
            string url = HttpUtility.HtmlEncode("https://localhost:44340/Author/Edit/" + id);
            mailMessage.Body = "Use the link in order in order to edit your personal information: " + url;
            mailMessage.From = new MailAddress("test.summer.camp.2020@gmail.com");
            mailMessage.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("test.summer.camp.2020@gmail.com", "jstoamnumuxkecjl");
            smtp.Send(mailMessage);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var authorDb = authorService.GetById(id);
                var viewModel = new AuthorViewModel
                {
                    Id = id,
                    Description = authorDb.Description
                };
                return View(viewModel);
            }
            catch(Exception e)
            { 
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Edit(AuthorViewModel viewModel)
        {
            try
            {
                string imageDirectory = Path.Combine(webHostEnvironment.WebRootPath, "AuthorImage");
                authorService.Edit(viewModel.Id, viewModel.Description, viewModel.Image, imageDirectory);
                return RedirectToAction("Details", new { id = viewModel.Id });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}