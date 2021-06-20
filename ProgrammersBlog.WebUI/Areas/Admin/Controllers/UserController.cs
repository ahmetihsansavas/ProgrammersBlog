using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(new UserListDto
            {
                Users = users,
                 ResultStatus = ResultStatus.Success
                 
            }) ;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("_UserAddPArtial");
        }


        public async Task<string> ImageUpload(UserAddDto userAddDto) 
        {
            // ~/img/user.Picture
            string wwroot = _env.WebRootPath;
            //ahmetsavas 
           // string fileName = Path.GetFileNameWithoutExtension(userAddDto.Picture.FileName); //dosya adını uzantısı olmadan alma
                                                                                             //  .png
            string fileExtension = Path.GetExtension(userAddDto.PictureFile.FileName);
            DateTime dateTime = DateTime.Now;
            // AhmetSavas_587_38_12_3_10_2021.png
            string fileName = $"{userAddDto.UserName}_{dateTime.FullDateAndTimeStringwithUnderscore()}{fileExtension}";
            var path = Path.Combine($"{wwroot}/img",fileName); //resim dosyasının kayd. path
            await using(var stream = new FileStream(path,FileMode.Create))
            {
                await userAddDto.PictureFile.CopyToAsync(stream);
            }

            return fileName;
        }

    }
}
