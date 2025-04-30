using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace fiskalizimi;

public class Pki
{
    /// <summary>
    /// Generates a pem format from the byte array
    /// </summary>
    /// <param name="type">The type of the pem</param>
    /// <param name="data">data to be encoded</param>
    /// <returns>pem</returns>
    public static string ExportToPem(string type, byte[] data)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"-----BEGIN {type}-----");
        builder.AppendLine(Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine($"-----END {type}-----");
        return builder.ToString();
    }
    
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
    /// Generates a CSR
    /// </summary>
    /// <param name="privateKey"></param>
    /// <param name="company">The name of the company</param>
    /// <param name="nui">NUI (BusinessID) of the business</param>
    /// <param name="branchID">The BranchID</param>
    /// <param name="posID">PosID</param>
    /// <returns>CSR in DER format</returns>
    public static byte[] CreateCsr(ECDsa privateKey, string company, string nui, string branchID, string posID)
    {
        // Construct distinguished name
        string subject = $"C=RKS, O={nui}, OU={posID}, L={branchID}, CN={company}";

        // Create the certificate request
        var request = new CertificateRequest(subject, privateKey, HashAlgorithmName.SHA256);

        // Generate CSR in DER format
        byte[] csrDer = request.CreateSigningRequest();
        return csrDer;
    }
}