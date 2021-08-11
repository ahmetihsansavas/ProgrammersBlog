using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ProgrammersBlog.Services.AutoMapper.Profiles;
using ProgrammersBlog.Services.Extensions;
using ProgrammersBlog.WebUI.AutoMapper.Profiles;
using ProgrammersBlog.WebUI.Helpers.Abstract;
using ProgrammersBlog.WebUI.Helpers.Concrete;
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
        public IConfiguration Configuration { get;} //AppSettings e erismek icin kullan.

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

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
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ArticleProfile),typeof(UserProfile)); //Services Katman�nda kulland. AutoMapper Class'lar�n�n oto. taranmas�            
            services.LoadMyServices(connectionString:Configuration.GetConnectionString("LocalDb")); //ServiceCollectionExtensions class i�eris. yazm�s old. metod ve AppSettings iceris. olan connectionString imizi ekledik
            services.AddScoped<IImageHelper, ImageHelper>();
            services.ConfigureApplicationCookie(options => 
            {
                options.LoginPath = new PathString("/Admin/User/Login"); //user giris path i
                options.LogoutPath = new PathString("/Admin/User/Logout");// user c�k�s path i
                options.Cookie = new CookieBuilder
                {
                    Name = "ProgrammersBlog", //cookie ad�
                    HttpOnly = true, //cookie bilg. ele gecirilmemesi icin
                    SameSite = SameSiteMode.Strict, //cookie bilg. sadece kendi sitemizden geld. zaman. CSRF sald�r�n�n onune gecer. 
                    SecurePolicy = CookieSecurePolicy.SameAsRequest //Always olmal�,test asamas�nda old. icin b�rakt�k yoksa bir gucenlik ac�g�
                
                };
                options.SlidingExpiration = true; // kull. siteye girs. yapt. cookilerin zamanl� bir sek. tut.
                options.ExpireTimeSpan = System.TimeSpan.FromDays(7); //cookie nin silinme s�resi
                options.AccessDeniedPath = new PathString("/Admin/User/AccessDenied"); //yetkisi olmayan bir yere giris yap. kull. icin
            });
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
