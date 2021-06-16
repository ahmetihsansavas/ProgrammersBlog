using Microsoft.AspNetCore.Identity;
using ProgrammersBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entities.Concrete
{
    public class Role:IdentityRole<int>
    {
       // public string Name { get; set; } 
       // public string Description { get; set; }
       // public ICollection<User> Users { get; set; } //Bir Rol birden fazla kullanıcıya sahip olab. ancak bir User bir Role sahip olacaktır.
    
    
    }
}
