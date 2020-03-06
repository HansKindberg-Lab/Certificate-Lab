using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Business.Security;
using Application.Business.Security.Cryptography;
using Application.Business.Security.Cryptography.Configuration;
using Application.Business.Web.Mvc;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application
{
	public class Startup
	{
		#region Constructors

		public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
		}

		#endregion

		#region Properties

		protected internal virtual IConfiguration Configuration { get; }
		protected internal virtual IHostEnvironment HostEnvironment { get; }

		#endregion

		#region Methods

		public void Configure(IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder.UseDeveloperExceptionPage();

			if(!this.HostEnvironment.IsDevelopment())
				applicationBuilder.UseHsts();

			applicationBuilder.UseHttpsRedirection();
			applicationBuilder.UseStaticFiles();
			applicationBuilder.UseRouting();
			applicationBuilder.UseAuthentication();
			applicationBuilder.UseAuthorization();
			applicationBuilder.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
				.AddCertificate(options =>
				{
					this.Configuration.GetSection("CertificateAuthentication").Bind(options);

					options.Events = new CertificateAuthenticationEvents
					{
						OnCertificateValidated = context =>
						{
							var certificate = context.ClientCertificate;
							var logger = context.HttpContext.RequestServices.GetService<LoggerFactory>()?.CreateLogger(typeof(CertificateValidator));

							try
							{
								context.HttpContext.RequestServices.GetRequiredService<ICertificateValidator>().Validate(certificate);

								var claims = new[]
								{
									new Claim(
										ClaimTypes.Name,
										certificate.Subject,
										ClaimValueTypes.String,
										context.Options.ClaimsIssuer)
								};

								context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
								context.Success();
							}
							catch(Exception exception)
							{
								const string message = "Invalid certificate.";

								if(logger != null && logger.IsEnabled(LogLevel.Error))
									logger.LogError(exception, message);

								context.Fail(message);
							}

							return Task.CompletedTask;
						}
					};
				});

			services.AddControllersWithViews(options => { options.Filters.Add<AuthorizationFilter>(); });
			services.AddSingleton<ICertificateValidator, CertificateValidator>();
			services.Configure<AuthorizationOptions>(this.Configuration.GetSection("Authorization"));
			services.Configure<CertificateAuthenticationOptions>(this.Configuration.GetSection("CertificateAuthentication"));
			services.Configure<CertificateValidatorOptions>(this.Configuration.GetSection("CertificateValidator"));
		}

		#endregion
	}
}