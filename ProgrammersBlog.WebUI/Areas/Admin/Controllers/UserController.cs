using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly SignInManager<User> _signinManager;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public UserController(UserManager<User> userManager, IWebHostEnvironment env, IMapper mapper, SignInManager<User> signinManager)
        {
            _userManager = userManager;
            _env = env;
            _mapper = mapper;
            _signinManager = signinManager;
        }

        [Authorize(Roles ="Admin")]
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
        public IActionResult Login() 
        {
            return View("UserLogin");
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
                if (user!=null)
                {
                    var result = await _signinManager.PasswordSignInAsync(user, userLoginDto.Password, userLoginDto.RememberMe, false);
                    //result return Task<SignInResult>
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                      
                    }
                    
                    else
                    {
                        ModelState.AddModelError("", "E-posta adresiniz veya şifreniz yanlıştır");
                        return View("UserLogin");
                    }


                }
                else
                {
                    ModelState.AddModelError("", "E-posta adresiniz veya şifreniz yanlıştır");
                    return View("UserLogin");
                }
            }

            else
            {
                return View("UserLogin");
            }

           
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("_UserAddPArtial");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            if (ModelState.IsValid)
            {
                userAddDto.Picture = await ImageUpload(userAddDto.UserName,userAddDto.PictureFile); //kull. resim dosyasının adını db de tutuyoruz.
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

        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> Delete(int userId) 
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                var deletedUser = JsonSerializer.Serialize(new UserDto 
                {
                     ResultStatus = ResultStatus.Success,
                     Message = $"{user.UserName} adlı kullanıcı başarıyla silinmiştir. "
                });
                return Json(deletedUser);
            }
            else 
            {
                string errorMessages = String.Empty;
                foreach (var error in result.Errors)
                {
                    errorMessages = $"*{error.Description}\n";
                }
                var deletedUserErrorModel = JsonSerializer.Serialize(new UserDto
                {
                    ResultStatus = ResultStatus.Error,
                    Message = $"{user.UserName} adlı kullanıcı silinirken bazı hatalar olustu.",
                    User = user
                });
                return Json(deletedUserErrorModel);
            }
        
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<PartialViewResult> Update(int userId) 
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u=>u.Id==userId);
            var userUpdateDto = _mapper.Map<UserUpdateDto>(user);
            return PartialView("_UserUpdatePartial",userUpdateDto);
        
        
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto) 
        {
            if (ModelState.IsValid) //kayd. olan datalar dogru mu ???
            {
                bool isNewPictureUploaded = false; //yeni resim bilgisi
                var oldUser = await _userManager.FindByIdAsync(userUpdateDto.Id.ToString()); //degist. olan kullanıcı
                var oldUserPicture = oldUser.Picture; //kullanıcının eski resmi
                if (userUpdateDto.PictureFile!=null)
                {
                    userUpdateDto.Picture = await ImageUpload(userUpdateDto.UserName,userUpdateDto.PictureFile); //kull yeni resim eklemek isterse 
                    isNewPictureUploaded = true;
                }
                var updatedUser = _mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser); //güncell. kullanıcı  user tipine dönüst.
                var result = await _userManager.UpdateAsync(updatedUser); //db de güncellenir
                if (result.Succeeded)
                {
                    if (isNewPictureUploaded)
                    {
                        ImageDelete(oldUserPicture);
                    }
                    var userUpdateViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModel
                    {
                        UserDto = new UserDto
                        {
                            ResultStatus = ResultStatus.Success,
                            Message = $"{updatedUser.UserName} adlı kullanıcı basarıyla güncellenmistir.",
                            User = updatedUser
                        },
                        UserUpdatePartial = await this.RenderViewToStringAsync("_UserUpdatePartial", userUpdateDto)
                    }) ;
                    return Json(userUpdateViewModel);
                }
                else 
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    var userUpdateErrorViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModel
                    {
                        UserUpdateDto = userUpdateDto,
                        UserUpdatePartial = await this.RenderViewToStringAsync("_UserUpdatePartial", userUpdateDto)
                    });
                    return Json(userUpdateErrorViewModel);
                }
            
            }
            else
            {
                var userUpdateErrorModelStateViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModel
                {
                    UserUpdateDto = userUpdateDto,
                    UserUpdatePartial = await this.RenderViewToStringAsync("_UserUpdatePartial", userUpdateDto)
                });
                return Json(userUpdateErrorModelStateViewModel);
            }
        
        }

        [Authorize(Roles = "Admin,Editor")]
        public async Task<string> ImageUpload(string userName,IFormFile pictureFile) 
        {
            // ~/img/user.Picture
            string wwroot = _env.WebRootPath;
            //ahmetsavas 
           // string fileName = Path.GetFileNameWithoutExtension(picture.FileName); //dosya adını uzantısı olmadan alma
                                                                                             //  .png
            string fileExtension = Path.GetExtension(pictureFile.FileName);
            DateTime dateTime = DateTime.Now;
            // AhmetSavas_587_38_12_3_10_2021.png
            string fileName = $"{userName}_{dateTime.FullDateAndTimeStringwithUnderscore()}{fileExtension}";
            var path = Path.Combine($"{wwroot}/img",fileName); //resim dosyasının kayd. path
            await using(var stream = new FileStream(path,FileMode.Create))
            {
                await pictureFile.CopyToAsync(stream);
            }

            return fileName;
        }

        [Authorize(Roles = "Admin,Editor")]
        public bool ImageDelete(string pictureName) //kull. edit isl. eski resmi silmek icin kull.
        {
            string wwwroot = _env.WebRootPath;
            var fileToDelete = Path.Combine($"{wwwroot}/img", pictureName);
            if (System.IO.File.Exists(fileToDelete)) //böyle bir dosya var mı
            {
                System.IO.File.Delete(fileToDelete); //dosyayı silme
                return true;
            }
            else 
            {
                return false;
            }
        
        }

    }
}
