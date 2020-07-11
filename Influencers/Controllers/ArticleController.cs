using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Influencers.BusinessLogic.DTOs;
using Influencers.BusinessLogic.Services;
using Influencers.BusinessLogic.ViewModels;
using Influencers.BusinessLogic.VIewModels;
using Influencers.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Influencers.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ArticleService articleService;
        private readonly AuthorService authorService;
        private readonly TagService tagService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ArticleController(ArticleService articleService,
                                 AuthorService authorService,
                                 TagService tagService,
                                 IWebHostEnvironment webHostEnvironment)
        {
            this.articleService = articleService;
            this.authorService = authorService;
            this.tagService = tagService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                return View(this.articleService.GetAll().OrderByDescending(article => article.Date));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateArticleViewModel { TagsDb = tagService.GetAllToString(), DoesAuthorExist = true }); ;
        }

        public IActionResult Create([FromForm]CreateArticleViewModel viewModel)
        {
            string imageDirectory = Path.Combine(webHostEnvironment.WebRootPath, "ArticleImage");
            try
            {
                if (ModelState.IsValid == false) return View(viewModel);
                if (authorService.DoesAuthorExist(viewModel.Email))
                {
                    tagService.AddMultipleTags(viewModel.ArticleTags);
                    var tags = tagService.GetForArticleFromString(viewModel.ArticleTags);
                    articleService.Add(viewModel.Title,
                                       viewModel.Content,
                                       viewModel.Email,
                                       viewModel.Image,
                                       imageDirectory,
                                       tags);
                    return RedirectToAction("Index");
                }
                if (viewModel.DoesAuthorExist == true)
                {
                    viewModel.DoesAuthorExist = false;
                    return View(viewModel);
                }
                tagService.AddMultipleTags(viewModel.ArticleTags);
                var myTags = tagService.GetForArticleFromString(viewModel.ArticleTags);
                authorService.Add(viewModel.Email, viewModel.FirstName, viewModel.LastName);
                articleService.Add(viewModel.Title,
                                   viewModel.Content,
                                   viewModel.Email,
                                   viewModel.Image,
                                   imageDirectory,
                                   myTags);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                return View(articleService.Details(id));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Vote(VoteArticleDTO voteArticleDTO)
        {
            try
            {
                articleService.Vote(voteArticleDTO.ArticleId, voteArticleDTO.VoteValue);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var articleDb = articleService.GetById(id);
                var viewModel = new EditArticleViewModel
                {
                    Id = id,
                    Content = articleDb.Content,
                    Title = articleDb.Title,
                    TagsDb = tagService.GetAllToString().ToList(),
                    ArticleTags = tagService.GetNamesAsAString(id)
                };
                return View(viewModel);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Edit(EditArticleViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            try
            {
                tagService.AddMultipleTags(viewModel.ArticleTags);
                var myTags = tagService.GetForArticleFromString(viewModel.ArticleTags);
                articleService.Edit(viewModel.Id, viewModel.Title, viewModel.Content, myTags);
                return RedirectToAction("Details", new { id = viewModel.Id });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            } 
        }

        [HttpGet]
        public IActionResult FilterAfterTag(int id)
        {
            try
            {
                return View("Index", articleService.FilterByTagId(id).OrderByDescending(article => article.Date));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult FilterByTagName(string searchTag)
        {
            try
            {
                return View("Index", articleService.FilterByTagName(searchTag).OrderByDescending(article => article.Date));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult Trending()
        {
            try
            {
                return View(articleService.GetTranding());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult SendView(int id)
        {
            try
            {
                articleService.SendView(id);
                return RedirectToAction("Details", new { id = id });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public JsonResult GetAllAsJson()
        {
            return Json(tagService.GetAllToString().ToList());
        }

        public IActionResult SendEmail(int id)
        {
            var articleDb = articleService.GetById(id);
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(articleDb.Email);
            mailMessage.Subject = "Influencers";
            string url = HttpUtility.HtmlEncode("https://localhost:44340/Article/Edit/" + id);
            mailMessage.Body = "Use the link in order to edit your article: " + url;
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
    }
}