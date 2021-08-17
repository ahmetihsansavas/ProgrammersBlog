using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var result= await _articleService.GetAllByNonDeleted();
            if (result.ResultStatus==ResultStatus.Success)
            {
                return View(result.Data);
            }

            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }
    }
}
