using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            if (ModelState.IsValid)
            {
                //eğer eklenecek olan veride bir hata yoksa dn ye ekliyoruz daha sonra Success olan veriyi kull. tekrardan göst. icin
               //Json'a donust. ve eklenen result verisinin datasını alıyoruz ve göst. olan sayfaya render ediyoruz.
                var result = await _categoryService.Add(categoryAddDto, "Ahmet");
                if (result.ResultStatus == ResultStatus.Success)
                {
                    var categoryAddAjaxModel = JsonSerializer.Serialize(new CategoryAddAjaxViewModel()
                    {
                        CategoryDto = result.Data,
                        CategoryAddPartial = await this.RenderViewToStringAsync("_CategoryAddPartial", categoryAddDto)


                    }) ;
                    return Json(categoryAddAjaxModel);
                }
            }


            var categoryAddAjaxErrorModel = JsonSerializer.Serialize(new CategoryAddAjaxViewModel()
            {
                CategoryAddPartial = await this.RenderViewToStringAsync("_CategoryAddPartial", categoryAddDto)


            });
            return Json(categoryAddAjaxErrorModel);

        }
    }
}
