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
using ProgrammersBlog.WebUI.Helpers.Abstract;
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
        private readonly IMapper _mapper;
        private readonly IImageHelper _imageHelper;
        public UserController(UserManager<User> userManager, IMapper mapper, SignInManager<User> signinManager, IImageHelper imageHelper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signinManager = signinManager;
            _imageHelper = imageHelper;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout() 
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" }); //kullanıcı logout yap. sonra Home Index e yönlend.
        
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
                var uploadedImageDtoResult = await _imageHelper.UploadedUserImage(userAddDto.UserName,userAddDto.PictureFile); //kull. resim dosyasının adını db de tutuyoruz.
                userAddDto.Picture = uploadedImageDtoResult.ResultStatus == ResultStatus.Success 
                    ? uploadedImageDtoResult.Data.FullName
                    :"userImages/defaultUser.png";
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
                    var uploadedImageDtoResult = await _imageHelper.UploadedUserImage(userUpdateDto.UserName, userUpdateDto.PictureFile); //kull. resim dosyasının adını db de tutuyoruz.
                    userUpdateDto.Picture = uploadedImageDtoResult.ResultStatus == ResultStatus.Success
                        ? uploadedImageDtoResult.Data.FullName
                        : "userImages/defaultUser.png";
                    if (oldUserPicture != "userImages/defaultUser.png") //kull. resmi ortak kull. resim şartı
                    {
                        isNewPictureUploaded = true;
                    }
                }
                var updatedUser = _mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser); //güncell. kullanıcı  user tipine dönüst.
                var result = await _userManager.UpdateAsync(updatedUser); //db de güncellenir
                if (result.Succeeded)
                {
                    if (isNewPictureUploaded)
                    {
                        _imageHelper.Delete(oldUserPicture);
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

        [Authorize]
        [HttpGet]
        public async Task<ViewResult> ChangeDetails() 
        {
            var user = await _userManager.GetUserAsync(HttpContext.User); //giris yap. kull. bilgilerini HttpContext.USer dan alıyoruz.
            var updateDto = _mapper.Map<UserUpdateDto>(user);
            
            return View(updateDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ViewResult> ChangeDetails(UserUpdateDto userUpdateDto)
        {
            if (ModelState.IsValid) //kayd. olan datalar dogru mu ???
            {
                bool isNewPictureUploaded = false; //yeni resim bilgisi
                var oldUser = await _userManager.GetUserAsync(HttpContext.User); //degist. olan kullanıcı
                var oldUserPicture = oldUser.Picture; //kullanıcının eski resmi
                if (userUpdateDto.PictureFile != null)
                {
                    // userUpdateDto.Picture = await ImageUpload(userUpdateDto.UserName, userUpdateDto.PictureFile); //kull yeni resim eklemek isterse 
                    var uploadedImageDtoResult = await _imageHelper.UploadedUserImage(userUpdateDto.UserName, userUpdateDto.PictureFile); //kull. resim dosyasının adını db de tutuyoruz.
                    userUpdateDto.Picture = uploadedImageDtoResult.ResultStatus == ResultStatus.Success
                        ? uploadedImageDtoResult.Data.FullName
                        : "userImages/defaultUser.png";
                    if (oldUserPicture!= "userImages/defaultUser.png") //kull. resmi ortak kull. resim şartı
                    {
                        isNewPictureUploaded = true;
                    }
                   
                }
                var updatedUser = _mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser); //güncell. kullanıcı  user tipine dönüst.
                var result = await _userManager.UpdateAsync(updatedUser); //db de güncellenir
                var count = 0;
                if (result.Succeeded)
                {
                    if (isNewPictureUploaded)
                    {
                        _imageHelper.Delete(oldUserPicture);
                    }
                    TempData.Add("SuccessMessage", $"{updatedUser.UserName} adlı kullanıcı basarıyla güncellenmistir.");
                    count++;
                    if (count>=1)
                    {
                        TempData.Remove("SuccessMessage");
                    }
                    return View(userUpdateDto);//güncel. veri
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(userUpdateDto);
                }

            }
            else
            {
                return View(userUpdateDto);
            }
        }

        [Authorize]
        [HttpGet]
        public ViewResult PasswordChange() 
        {
            return View();  
        
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PasswordChange(UserPasswordChangeDto userPasswordChangeDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var isVerified = await _userManager.CheckPasswordAsync(user, userPasswordChangeDto.CurrentPassword);
                if (isVerified)
                {
                    var result = await _userManager.ChangePasswordAsync(user, userPasswordChangeDto.CurrentPassword,
                        userPasswordChangeDto.NewPassword);
                    var count = 0;
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user); //Kull. ayarlar. değiş. yap. icin
                        await _signinManager.SignOutAsync(); //Kull. sifresini degist. icin cıkıs yapmasını sagl.
                        await _signinManager.PasswordSignInAsync(user, userPasswordChangeDto.NewPassword, true, false); //kull. giris yap.
                        TempData.Add("SuccessMessage", $"Şifreniz basarıyla güncellenmistir.");
                        count++;
                        if (count >= 1)
                        {
                            TempData.Remove("SuccessMessage");
                        }
                        return View();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(userPasswordChangeDto);
                    }
              
                }
                else
                {
                    ModelState.AddModelError("", "Lütfen girmis old. şimdiki şifrenizi kontrol ediniz...");
                    return View(userPasswordChangeDto);
                }
            }
            else
            {
                ModelState.AddModelError("", "Lütfen girmis old. şimdiki şifrenizi kontrol ediniz...");
                return View(userPasswordChangeDto);
            }
         

        }


        [HttpGet]
        public ViewResult AccessDenied() 
        {
            return View();
        }

    }
}
