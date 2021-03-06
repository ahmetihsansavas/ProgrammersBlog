using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Dtos
{
    public class CategoryAddDto
    {
        //Data Transfer Object ; Entity Sınıflarının sadece kullanacağımız kısımlarının dışarı acıldıgı model..
        [DisplayName("Kategori Adı")]
        [Required(ErrorMessage ="{0} Boş geçilmemelidir")]
        [MaxLength(70,ErrorMessage ="{0} {1} karakterden büyük olmamalıdır.")]
        [MinLength(3,ErrorMessage ="{0} {1} karakterden az olmamalıdır.")]
        public string Name { get; set; }

        [DisplayName("Kategori Açıklaması")]
        [MaxLength(500, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden az olmamalıdır.")]
        public string Description { get; set; }

        [DisplayName("Kategori Özel Not Açıklaması")]
        [MaxLength(500, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden az olmamalıdır.")]
        public string Note { get; set; }



        [DisplayName("Aktif mi?")]
        [Required(ErrorMessage = "{0} Boş geçilmemelidir")]
        public bool IsActive { get; set; }

    }
}
