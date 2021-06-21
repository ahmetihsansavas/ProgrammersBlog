using ProgrammersBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Areas.Admin.Models
{
    public class UserAddAjaxViewModel
    {
        public UserAddDto UserAddDto { get; set; } //
        public string UserAddPartial { get; set; } // Post islemi yapt. bize post islemini parse etmis olacak, eğer model.IsStateValid durumunu kontrol etmek için hataları geri dönmek için
        public UserDto UserDto { get; set; }

    }
}
