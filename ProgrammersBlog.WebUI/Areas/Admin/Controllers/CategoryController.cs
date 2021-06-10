using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task <IActionResult> Index()
        {
            var result = await _categoryService.GetAll();

            return View(result.Data);
           //if (result.ResultStatus == ResultStatus.Success)
           // {
           //     return View(result.Data);
           // }
            
        }

        [HttpGet]
        public IActionResult Add() 
        {
            return PartialView("_CategoryAddPartial");
        }
        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
        {
            var categoryAjaxModel = new CategoryAddAjaxViewModel()
            {
                CategoryAddPartial = await this.RenderViewToStringAsync("_CategoryAddPartial",categoryAddDto), //Projemize dısarıdan ekled. ControllerExtensions sınıfımızdan cektig. metod bu metod sayesinde partial view e json olarak veri aktarımı yap.
                
            };
            return PartialView("_CategoryAddPartial");
        }
    }
}
