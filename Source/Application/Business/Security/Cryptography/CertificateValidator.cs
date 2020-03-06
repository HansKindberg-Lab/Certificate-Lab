using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Application.Business.Security.Cryptography.Configuration;
using Microsoft.Extensions.Options;

namespace Application.Business.Security.Cryptography
{
	public class CertificateValidator : ICertificateValidator
	{
		#region Constructors

		public CertificateValidator(IOptions<CertificateValidatorOptions> options)
		{
			this.Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<CertificateValidatorOptions> Options { get; }

		#endregion

		#region Methods

		[SuppressMessage("Style", "IDE0063:Use simple 'using' statement")]
		public virtual void Validate(X509Certificate2 certificate)
		{
			try
			{
				if(certificate == null)
					throw new ArgumentNullException(nameof(certificate));

				var options = this.Options.Value;

				if(!string.Equals(options.Thumbprint, certificate.Thumbprint, StringComparison.Ordinal))
					throw new InvalidOperationException($"The certificate-thumbprint \"{certificate.Thumbprint}\" does not math \"{options.Thumbprint}\".");

				// ReSharper disable All
				using(var store = new X509Store(options.StoreName, options.StoreLocation))
				{
					store.Open(OpenFlags.ReadOnly);

					var storeCertificate = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, options.ValidOnly).Cast<X509Certificate2>().FirstOrDefault();

					if(storeCertificate == null)
						throw new InvalidOperationException($"The certificate with subject \"{certificate.Subject}\" and thumbprint \"{certificate.Thumbprint}\" was not found in store \"CERT:\\{options.StoreLocation}\\{options.StoreName}\".");
				}
				// ReSharper restore All
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Invalid certificate.", exception);
			}
		}

		#endregion
	}
}