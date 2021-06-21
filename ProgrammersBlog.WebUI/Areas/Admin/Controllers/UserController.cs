using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public UserController(UserManager<User> userManager, IWebHostEnvironment env, IMapper mapper)
        {
            _userManager = userManager;
            _env = env;
            _mapper = mapper;
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

        public async Task<JsonResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userListDto = JsonSerializer.Serialize(new UserListDto
            {
                Users = users,
                ResultStatus = ResultStatus.Success,
                Message = "Kullanıcılar basarılı bir sekilde gösterildi"


            },new JsonSerializerOptions 
            {
                 ReferenceHandler = ReferenceHandler.Preserve
            
            }) ;


            return Json(userListDto);
            
        }


        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("_UserAddPArtial");
        }
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            if (ModelState.IsValid)
            {
                userAddDto.Picture = await ImageUpload(userAddDto); //kull. resim dosyasının adını db de tutuyoruz.
                var user =  _mapper.Map<User>(userAddDto); //userAddDto kull AutoMApper ile User nesnesi olust.
                var result = await _userManager.CreateAsync(user, userAddDto.Password); //result döner
                if (result.Succeeded)
                {
                    var userAddAjaxViewModel = JsonSerializer.Serialize(new UserAddAjaxViewModel 
                    {
                        UserDto = new UserDto 
                        {
                            ResultStatus = ResultStatus.Success,
                            Message = $"{user.UserName} adlı kullanıcı başarıyla eklenmiştir.",
                            User = user
                        },
                      UserAddPartial = await this.RenderViewToStringAsync("_UserAddPartial",userAddDto)
                                        
                    });
                    return Json(userAddAjaxViewModel);
                }
                else 
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description); //result.error gelirse içeris. error leri foreach le dön.
                    
                    }
                    var userAddAjaxErrorModel = JsonSerializer.Serialize(new UserAddAjaxViewModel 
                    {
                         UserAddDto = userAddDto,
                         UserAddPartial = await this.RenderViewToStringAsync("_UserAddPartial",userAddDto)
                    
                    });
                    return Json(userAddAjaxErrorModel);
                }
            
            }

            var userAddajaxModelStateErrorModel = JsonSerializer.Serialize(new UserAddAjaxViewModel
            {
                UserAddDto = userAddDto,
                UserAddPartial = await this.RenderViewToStringAsync("_UserAddPartial",userAddDto)

            }) ;

            return Json(userAddajaxModelStateErrorModel);
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
