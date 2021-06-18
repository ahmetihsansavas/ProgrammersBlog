using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ProgrammersBlog.Services.AutoMapper.Profiles;
using ProgrammersBlog.Services.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions(opt=> 
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());//ResultStatus = 1 success gibi say�sal enum de�erlerini kullan. i�in
                //e�er ResultStatus === "success" gibi ResultStatus enum � m�z varsa opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)),
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; //Ic ice olan objelerde objeler birb. referans ettiklerinde sorun yasamamak icin
            }); //MVC yap. i�in.., ve uyg. derlemeden kaydettikten sonra sonuclar� gorucez.
            services.AddSession(); // kullan�c� oturumu ekl.
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ArticleProfile)); //Services Katman�nda kulland. AutoMapper Class'lar�n�n oto. taranmas�            
            services.LoadMyServices(); //ServiceCollectionExtensions class i�eris. yazm�s old. metod
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); //Hata yasad. bize yard�mc� olacak
            }
            app.UseSession(); // kullan�c� oturumu
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
          FileProvider = new PhysicalFileProvider(
         Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles"
            });
            app.UseRouting();
            //projemize kimlik dogrulamay� eklemek icin. routing den sonra kull. cunk� routing cal�t. sonra yetki sorgulama yap�l�r. 
            app.UseAuthentication();
            //yetki dogrulama
            app.UseAuthorization();
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
