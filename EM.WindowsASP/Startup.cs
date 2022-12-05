using EM.Domain;
using EM.Repository;
using EM.WindowsASP;

namespace EM.WindowsASP;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        //service do IHttpContextAccessor
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddControllersWithViews();


        //registrando middleware da sessao
        services.AddMemoryCache();
        services.AddSession();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //if (env.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
        //}
        //else
        //{
        //    app.UseExceptionHandler("/Home/Error");
        //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //    app.UseHsts();
        //}
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();


        //habilitando sessão middleware
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        });
    }
}