using System.Security.Cryptography;

namespace fiskalizimi;

public interface ISigner
{
    // returns base64 encoded signature of the signed bytes
    string SignBytes(byte[] data);
}

public class Signer : ISigner
{
    private readonly string _key;
    
    public Signer(string pemKey)
    {
        _key = pemKey;
    }
    
    public string SignBytes(byte[] data)
    {
        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(_key);

        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(data);
            var signature = ecdsa.SignHash(hash);
            
            var encodedSignature = Convert.ToBase64String(signature);
            return encodedSignature;
        }
    }
}