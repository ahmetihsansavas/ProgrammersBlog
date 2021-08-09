using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.WebUI.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.ViewComponents
{
    public class UserMenuViewComponent:ViewComponent
    {
        public readonly UserManager<User> _userManager;

        public UserMenuViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public ViewViewComponentResult Invoke() 
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            return View(new UserViewModel //yeni bir view model olusturmamızın nedeni ilerde menuye bazı özel. ekleyeb.Components klaso. isim de aynı olmalı
            {
                User = user
            }) ;
        }
    }
}
