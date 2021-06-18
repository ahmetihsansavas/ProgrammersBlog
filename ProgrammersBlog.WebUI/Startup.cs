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
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());//ResultStatus = 1 success gibi sayýsal enum deðerlerini kullan. için
                //eðer ResultStatus === "success" gibi ResultStatus enum ý mýz varsa opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)),
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; //Ic ice olan objelerde objeler birb. referans ettiklerinde sorun yasamamak icin
            }); //MVC yap. için.., ve uyg. derlemeden kaydettikten sonra sonuclarý gorucez.
            services.AddSession(); // kullanýcý oturumu ekl.
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ArticleProfile)); //Services Katmanýnda kulland. AutoMapper Class'larýnýn oto. taranmasý            
            services.LoadMyServices(); //ServiceCollectionExtensions class içeris. yazmýs old. metod
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); //Hata yasad. bize yardýmcý olacak
            }
            app.UseSession(); // kullanýcý oturumu
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
          FileProvider = new PhysicalFileProvider(
         Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles"
            });
            app.UseRouting();
            //projemize kimlik dogrulamayý eklemek icin. routing den sonra kull. cunkü routing calýt. sonra yetki sorgulama yapýlýr. 
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
