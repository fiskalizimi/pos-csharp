using System.Net.Http.Json;
using System.Text;
using Atk;
using Google.Protobuf;

namespace fiskalizimi;

public static class Fiskalizimi
{
    
    private const string PrivateKeyPem = @"
-----BEGIN EC PRIVATE KEY-----
MHcCAQEEIAj6fmj1djrjdWXIzyaSUi1Bp8/p/vT9rhPSNJhnfgSUoAoGCCqGSM49
AwEHoUQDQgAEZ0mBPR5oPsP+GJl0gAOvtZsubvTs/9nTL1p5qOEWRrKZJZU8Rrv8
UwqQASBfimRaEsdQli0DtCx0yKDZ3zE73Q==
-----END EC PRIVATE KEY-----";
    

    /// This method digitally signs the CitizenCoupon provided and returns the string that
    /// should be encoded into QR Code
    public static string SignCitizenCoupon(CitizenCoupon citizenCoupon, ISigner signer)
    {
        // Serialize the citizen coupon message to protobuf binary
        byte[] citizenCouponProto = citizenCoupon.ToByteArray();

        // convert the serialized protobuf of citizen coupon to base64 string
        var base64EncodedProto = Convert.ToBase64String(citizenCouponProto);

        // convert the base64 string of the citizen coupon to byte array
        var base64EncodedBytes = Encoding.UTF8.GetBytes(base64EncodedProto);
        
        // digitally sign the bytes and return the signature
        string signature = signer.SignBytes(base64EncodedBytes);
        
        Console.WriteLine($"Coupon   : {base64EncodedProto}");
        Console.WriteLine($"signature: {signature}");
        
        // Combine the encoded data and signature to create QR Code string and return it
        string qrCodeString = $"{base64EncodedProto}|{signature}";
        Console.WriteLine($"qr code  : {qrCodeString}");
        return qrCodeString;
    }

    /// This method digitally signs the PosCoupon provided and returns the pos coupon
    /// and signature in encoded base64 string 
    public static (string, string) SignPosCoupon(PosCoupon posCoupon, ISigner signer)
    {
        // Serialize the pos coupon message to protobuf binary
        byte[] posCouponProto = posCoupon.ToByteArray();
        
        // convert the serialized protobuf of pos coupon to base64 string
        var base64EncodedProto = Convert.ToBase64String(posCouponProto);

        // convert the base64 string of the pos coupon to byte array
        var base64EncodedBytes = Encoding.UTF8.GetBytes(base64EncodedProto);
        
        // digitally sign the bytes
        string signature = signer.SignBytes(base64EncodedBytes);
 
        Console.WriteLine($"Coupon   : {base64EncodedProto}");
        Console.WriteLine($"signature: {signature}");
        
        // return the coupon and signature as base64 strings
        return (base64EncodedProto, signature);
    }

    public static async Task SendQrCode()
    {
        const string url = "https://fiskalizimi.atk-ks.org/citizen/coupon";

        try
        {
            // create the model builder
            var builder = new ModelBuilder();
            // create signer using the private key defined
            var signer = new Signer(PrivateKeyPem);

            // get citizen coupon from the builder
            var citizenCoupon = builder.GetCitizenCoupon();

            // digitally sign citizen coupon and get the qr code 
            var qrCode = SignCitizenCoupon(citizenCoupon, signer);
            
            // prepare the request
            var request = new
            {
                citizen_id = 1,
                qr_code = qrCode
            };
        
            // POST the request to fiscalisation service 
            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync(url, request);
            
            // ensure that the response is success (2xx)
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Qr code sent successfully");
        }
        catch (Exception e)
        {
            // if there is an error write it to console
            Console.WriteLine("Error sending the QR code");
            Console.WriteLine(e);
        }
    }
    
    public static async Task SendPosCoupon()
    {
        const string url = "https://fiskalizimi.atk-ks.org/pos/coupon";
        
        try
        {
            // create the model builder
            var builder = new ModelBuilder();
            // create signer using the private key defined
            var signer = new Signer(PrivateKeyPem);

            // get pos coupon from the builder
            var posCoupon = builder.GetPosCoupon();

            // digitally sign pos coupon and get the coupon and signature in base64 string 
            var (coupon, signature) = SignPosCoupon(posCoupon, signer);

            // prepare the request
            var request = new
            {
                details = coupon,
                signature = signature
            };
        
            // POST the request to fiscalisation service 
            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync(url, request);
            
            // ensure that the response is success (2xx)
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Qr code sent successfully");
        }
        catch (Exception e)
        {
            // if there is an error write it to console
            Console.WriteLine("Error sending the QR code");
            Console.WriteLine(e);
        }
    }
    
    public static async Task Main(string[] args)
    {
        await SendPosCoupon();
        await SendQrCode();
    }
}