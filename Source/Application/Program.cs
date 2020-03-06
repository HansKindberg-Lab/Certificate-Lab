using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;

namespace Application
{
	public class Program
	{
		#region Methods

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.ConfigureKestrel(kestrelServerOptions => { kestrelServerOptions.ConfigureHttpsDefaults(options => { options.ClientCertificateMode = ClientCertificateMode.RequireCertificate; }); });
					webBuilder.UseStartup<Startup>();
				});
		}

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		#endregion
	}
}