using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace fiskalizimi;

public class Pki
{    
    /// <summary>
    /// Generate a private ECDsa key using the named curve P-256
    /// </summary>
    /// <returns>private key</returns>
    public static ECDsa GeneratePrivateKey()
    {
        // Create a new ECDsa key using the named curve P-256 (equivalent to prime256v1)
        var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        return ecdsa;
    }
    
    /// <summary>
    /// Generates a CSR in PEM format
    /// </summary>
    /// <param name="privateKey"></param>
    /// <param name="company">The name of the company</param>
    /// <param name="nui">NUI (BusinessID) of the business</param>
    /// <param name="branchID">The BranchID</param>
    /// <param name="posID">PosID</param>
    /// <returns>CSR in PEM format</returns>
    public static string CreateCsr(ECDsa privateKey, CsrRequest csrRequest)
    {
        // Construct distinguished name
        string subject = $"C={csrRequest.Country}, O={csrRequest.Nui}, OU={csrRequest.PosId}, L={csrRequest.BranchId}, CN={csrRequest.BusinessName}";

        // Create the certificate request
        var request = new CertificateRequest(subject, privateKey, HashAlgorithmName.SHA256);

        // Generate CSR in DER format
        string csrPem = request.CreateSigningRequestPem();
        return csrPem;
    }
}