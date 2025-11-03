using InformationRetrieval.BLL.Services;

namespace InformationRetrieval.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add Memory Cache for storing processing results
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<IIndexBuilderService, IndexBuilderService>();
            builder.Services.AddScoped<ITextProcessorService, TextProcessorService>();
            builder.Services.AddScoped<IBooleanQueryService, BooleanQueryService>();
            builder.Services.AddScoped<IPositionalQueryService, PositionalQueryService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
