using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ProgrammersBlog.Services.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); //MVC yap. i�in.., ve uyg. derlemeden kaydettikten sonra sonuclar� gorucez.
            services.AddAutoMapper(typeof(Startup)); //Services Katman�nda kulland. AutoMapper Class'lar�n�n oto. taranmas�
            services.LoadMyServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); //Hata yasad. bize yard�mc� olacak
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
          FileProvider = new PhysicalFileProvider(
         Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles"
            });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name:"Admin",
                    areaName:"Admin",
                    pattern:"Admin/{controller=Home}/{action=Index}/{id?}"
                    
                    );
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
