using ProgrammersBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Models
{
    public class UserUpdateAjaxViewModel
    {
        public UserUpdateDto UserUpdateDto { get; set; } //
        public string UserUpdatePartial { get; set; } // Post islemi yapt. bize post islemini parse etmis olacak, eğer model.IsStateValid durumunu kontrol etmek için hataları geri dönmek için
        public UserDto UserDto { get; set; }

    }
}
