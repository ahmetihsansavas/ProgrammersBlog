﻿using Microsoft.Extensions.DependencyInjection;
using ProgrammersBlog.DataAccess.Abstract;
using ProgrammersBlog.DataAccess.Concrete;
using ProgrammersBlog.DataAccess.Concrete.EntityFramework.Contexts;
using ProgrammersBlog.Entities.Concrete;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection LoadMyServices(this IServiceCollection serviceCollection) 
        {
            //WebUI kısmında Startup.cs içeris.de de yap. ancak biz düzenli olmasını isted. için yaptık.
            serviceCollection.AddDbContext<ProgrammersBlogContext>();
            serviceCollection.AddIdentity<User, Role>().AddEntityFrameworkStores<ProgrammersBlogContext>();
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<ICategoryService, CategoryManager>();
            serviceCollection.AddScoped<IArticleService, ArticleManager>();

            return serviceCollection;
        }
    }
}
