using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using AuthorizationOptions = Application.Business.Security.AuthorizationOptions;

namespace Application.Business.Web.Mvc
{
	public class AuthorizationFilter : AuthorizeFilter
	{
		#region Constructors

		public AuthorizationFilter(IOptions<AuthorizationOptions> options) : base(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
		{
			this.Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<AuthorizationOptions> Options { get; }

		#endregion

		#region Methods

		public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			if(!this.Options.Value.AllowAnonymous)
				await base.OnAuthorizationAsync(context).ConfigureAwait(false);
		}

		#endregion
	}
}