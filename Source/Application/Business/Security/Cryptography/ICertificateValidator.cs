using System.Security.Cryptography.X509Certificates;

namespace Application.Business.Security.Cryptography
{
	public interface ICertificateValidator
	{
		#region Methods

		void Validate(X509Certificate2 certificate);

		#endregion
	}
}