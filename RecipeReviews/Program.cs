using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RecipeReviews
{
    public class Program
    {
        public static int MaxRecipeRating { get; } = 5;

        public static string UploadsDirectory { get; } = "uploads";
        public static string ImagesDirectory { get; } = "images";

        public static string GetRequestPath(string fileName) => $"/{UploadsDirectory}/{ImagesDirectory}/{fileName}";
 
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
