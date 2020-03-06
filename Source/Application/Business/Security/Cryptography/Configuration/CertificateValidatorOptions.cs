using System.Security.Cryptography.X509Certificates;

namespace Application.Business.Security.Cryptography.Configuration
{
	public class CertificateValidatorOptions
	{
		#region Properties

		public virtual StoreLocation StoreLocation { get; set; } = StoreLocation.CurrentUser;
		public virtual StoreName StoreName { get; set; } = StoreName.My;
		public virtual string Thumbprint { get; set; }
		public virtual bool ValidOnly { get; set; }

		#endregion
	}
}