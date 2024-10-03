# Fiscalization Integration with C# using Protobuf

This repository provides a C# implementation for integrating with a fiscalization system using classes generated by Protobuf. The process includes constructing fiscal receipts (Citizen and POS Coupons), digitally signing them, and submitting them to the fiscalization service. This guide walks you through the steps necessary to integrate and execute the solution.

## Table of Contents

- [Project Overview](#project-overview)
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
- [Generating PROTOBUF models](#generating-protobuf-models)
  - [Manually generating Models](#manually-generating-models)
  - [Let .NET generate models automatically](#let-net-generate-models-automatically)
- [Model Explanation](#model-explanation)
    - [Citizen Coupon](#citizen-coupon)
    - [POS Coupon](#pos-coupon)
- [PKI Key Generation](#key-generation)
- [Digital Signing](#digital-signing)
    - [QR Code generation](#qr-code) 
- [Sending Data to Fiscalization Service](#sending-data-to-fiscalization-service)
    - [Sending Citizen Coupons](#sending-citizen-coupons)
    - [Sending POS Coupons](#sending-pos-coupons)
- [Running the Application](#running-the-application)

## Project Overview

This project provides a set of C# classes to interact with a fiscalization system. The key components include:

1. **Models**: This is the models generated by Protobuf
2. **ModelBuilder**: Constructs the Citizen and POS coupons (receipts) using predefined tax groups, items, and payment methods.
3. **Signer**: Signs the receipts using a digital signature created with an ECDSA private key.
4. **Fiskalizimi**: Contains methods for constructing, signing, and sending fiscal coupons to the fiscalization service.

### Key Technologies:
- **Protobuf**: Used for serializing the data models (CitizenCoupon, PosCoupon) to binary.
- **ECDSA**: Elliptic curve algorithm used for digital signatures.
- **HttpClient**: For sending data to the fiscalization service.

## Getting Started

### Prerequisites

Before integrating the system, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Protobuf Compiler](https://developers.google.com/protocol-buffers)
- A valid [ECDSA private key](#key-generation) for signing the data.

### Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/your-repo/fiskalizimi-integration.git
   cd fiskalizimi-integration

## Generating PROTOBUF models ##

### Manually generating Models ###

### Let .NET generate models automatically

## Model Explanation ##

### Citizen Coupon ###

The ```CitizenCoupon``` represents a simplified receipt that will be the part of QR Code. Below is the example structure created by the ```ModelBuilder``` class:

```
public CitizenCoupon GetCitizenCoupon()
{
    var citizenCoupon = new CitizenCoupon
    {
        BusinessId = 1,
        FiscalId = 1,
        CouponId = 2,
        PosId = 1,
        Type = CouponType.Sale,
        Time = new DateTimeOffset(2024, 10, 1, 15,30, 20, TimeSpan.Zero).ToUnixTimeSeconds(),
        Total = 1820,
        TaxGroups =
        {
            new TaxGroup { TaxRate = "C", TotalForTax = 450, TotalTax = 0 },
            new TaxGroup { TaxRate = "D", TotalForTax = 320, TotalTax = 26 },
            new TaxGroup { TaxRate = "E", TotalForTax = 1850, TotalTax = 189 }
        },
        TotalTax = 215
    };
    
    return citizenCoupon;
}
```

The Citizen Coupon includes:

* **BusinessID** which is NUI of the business (received from ATK)
* **FiscalID** also defined by ATK
* **CouponID** is the unique identifier of the fiscal coupon generated by POS system
* **PosID** is the unique id of the POS. POS is the computer/till that has the POS system installed. Each POS unit must have a unique ID.
* **Type** this is the type of the coupon. It is an enum value and can be ```SALE```, ```RETURN``` or ```CANCEL```
* **Time** the time fiscal coupon is issued. The value is Unix timestamp
* **Total** that represents the total value to be paid by customer
* **TaxGroups** is an array of ```TaxGroup``` objects. Each ```TaxGroup``` object represents the details about tax category 
* **TotalTax** is the amount of the tax in total that customer will have to pay

**NOTE:** These details must match the [POS Coupon](#pos-coupon) details, otherwise the coupon will be market as ```FAILED VERIFICATION``` !


### POS Coupon ###

The PosCoupon includes all details of the POS Coupon that will be printed and given to the customer.

```
public PosCoupon GetPosCoupon()
{
    var posCoupon = new PosCoupon
    {
        BusinessId = 1,
        FiscalId = 1,
        CouponId = 2,
        Location = "Prishtine",
        OperatorId = "Kushtrimi",
        PosId = 1,
        Type = CouponType.Sale,
        Time = new DateTimeOffset(2024, 10, 1, 15,30, 20, TimeSpan.Zero).ToUnixTimeSeconds(),
        Items =
        {
            new CouponItem { Name = "uje rugove", Price = 150, Quantity = 3, Total = 450, TaxRate = "C", Type = "TT" },
            new CouponItem { Name = "sendviq", Price = 300, Quantity = 2, Total = 600, TaxRate = "E", Type = "TT" },
            new CouponItem { Name = "buke", Price = 80, Quantity = 4, Total = 320, TaxRate = "D", Type = "TT" },
            new CouponItem { Name = "machiato e madhe", Price = 150, Quantity = 3, Total = 450, TaxRate = "E", Type = "TT" }
        },
        Payments =
        {
            new Payment { Type = PaymentType.Cash, Amount = 500 },
            new Payment { Type = PaymentType.CreditCard, Amount = 1000 },
            new Payment { Type = PaymentType.Voucher, Amount = 320 }
        },
        Total = 1820,
        TaxGroups =
        {
            new TaxGroup { TaxRate = "C", TotalForTax = 450, TotalTax = 0 },
            new TaxGroup { TaxRate = "D", TotalForTax = 320, TotalTax = 26 },
            new TaxGroup { TaxRate = "E", TotalForTax = 1850, TotalTax = 189 }
        },
        TotalTax = 215,
        TotalNoTax = 1605
    };

    return posCoupon;
}
```

The POS Coupon includes:

* **BusinessID** which is NUI of the business (received from ATK)
* **FiscalID** also defined by ATK
* **CouponID** is the unique identifier of the fiscal coupon generated by POS system
* **Location** is the location/city of the Sale Point
* **OperatorID** is the ID/Name of the operator/server
* **PosID** is the unique id of the POS. POS is the computer/till that has the POS system installed. Each POS unit must have a unique ID.
* **Type** this is the type of the coupon. It is an enum value and can be ```SALE```, ```RETURN``` or ```CANCEL```
* **Time** the time fiscal coupon is issued. The value is Unix timestamp
* **Items** is an array of ```CouponItem``` objects. Each ```CouponItem``` represents an item sold to the customer.
* **Payments** is an array of ```Payment``` that represent the types of the payment methods and the amoun used by customer to pay for the goods. The valid types are: ```Cash```, ```CreditCard```, ```Voucher```, ```Cheque```, ```CryptoCurrency```, and ```Other```. 
* **Total** that represents the total value to be paid by customer
* **TaxGroups** is an array of ```TaxGroup``` objects. Each ```TaxGroup``` object represents the details about tax category
* **TotalTax** is the amount of the tax in total that customer will have to pay
* **TotalNoTax** is the total amount without tax that customer will have to pay

**NOTE:** These details must match the [Citizen Coupon](#citizen-coupon) details, otherwise the coupon will be market as ```FAILED VERIFICATION``` !

## Key Generation ##

There are different ways to generate a PKI key pair, depending on the operating system. 

**WARNING!** Each POS system (PC/till) needs to have its own PKI key pair. The private key should never leave the machine that it is generated on !!!

We have provided a tool that simplifies the process a lot by creating the key pair, generating a CSR and sending the CSR to ATK Certificate Authority to be digitally signed and verified. 

To download the tool on you machine, click on one of the links below (depending on the operating system you are using):

* [atkcli for windows](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-cli-windows.zip)
* [atkcli for MacOS Intel](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-cli-macos-intel.zip)
* [atkcli for MacOS M1/M2](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-cli-macos-apple-silicon.zip)
* [atkcli for Linux](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-linux.zip)

Once you have downloaded the atkcli tool, and extracted/unzipped it to a folder, then you need to use the following command by chaning the values in curly braces and providing valid data:

```
./atkcli onboard -b "{businesID}" -f "{fiscalID}" -p "{PosID} -n "{Business name}" -u "http://a94422f45ed154fe59456dd9678d460f-556849162.us-east-1.elb.amazonaws.com/ca/signcsr"
```

For example, if you BusinssID (NUI) is 888234, FiscalID is 3543234 and the business name is "Joe Bloggs Caffee", then the command for POS with id 1 would look like:
```
 ./atkcli onboard -b "888234" -f "3543234" -p "1"  -n "Joe Bloggs Caffee" -u "http://a94422f45ed154fe59456dd9678d460f-556849162.us-east-1.elb.amazonaws.com/ca/signcsr"
```

if everything went Ok, you should get back a 200 response and a valid certificate, something like:

```
Signed Certificate saved successfully
-----BEGIN CERTIFICATE-----
MIICDzCCAbWgAwIBAgIRAKWGerCepv0WWsWbfqLL2WIwCgYIKoZIzj0EAwIwYzEM
MAoGA1UEBhMDUktTMSgwJgYDVQQKEx9BZG1pbmlzdHJhdGEgVGF0aW1vcmUgZSBL
b3NvdmVzMRAwDgYDVQQLEwdlRmF0dXJhMRcwFQYDVQQDEw53d3cuYXRrLWtzLm9y
ZzAeFw0yNDEwMDIxMzQyNDNaFw0yOTEwMDExMzQyNDNaME0xDDAKBgNVBAYTA1JL
UzEPMA0GA1UEChMGODg4MjM0MRAwDgYDVQQLEwczNTQzMjM0MRowGAYDVQQDExFK
b2UgQmxvZ2dzIENhZmZlZTBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABDx8VXYZ
3jH7F0oWwJZEgWj/XDPN8qcXJos4gX1LnyhKjg9dwj5m5fYbVT3n42sORBDKMDQ0
BUv9b7HmlQSnDnyjYDBeMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEF
BQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/BAIwADAfBgNVHSMEGDAWgBSZJp0OTWII
XbHAKFoCTvpLPTevcDAKBggqhkjOPQQDAgNIADBFAiBd0JfUeggtY18pfDYgXcAM
w/nn2Z9qk5n0hXet9OrdVwIhAP6AyGnyFhLVL5X5i/zEXSYqDV0Tatjx6ie55wVF
Zxm4
-----END CERTIFICATE-----
```
and in the folder you will have another three files:

* ```privatekey.bin``` - the private key generated by atkcli (encrypted)
* ```csr.pem``` - CSR (Certificate Signing Request) generated by atkcli and sent to CA
* ```certificate.bin``` - signed certificate by ATC CA (encrypted)

To view certificate and private key in PEM format, using the command below:

```
./atkcli cert show -p
```

To extract certificate and private key in PEM format, using the command below:
```
./atkcli cert export -p
```

the last command will create another two files in the folder ```private-key.pem``` and ```signed-certificate.pem```

**WARNING !** Makes sure to keep private key safe.

## Digital Signing ##

Before the data is sent to the Fiscalization System, the POS Coupon details need to be digitally signed using the private key to ensure the authenticity and integrity of the data transmitted to the fiscalization system.

#### Why Digital Signing? ####

The fiscalization system requires each coupon to be signed digitally before submission to ensure:

1. **Data Integrity:** Ensures that the data sent to the fiscalization service has not been tampered with during transmission.
3. **Authentication:** Confirms that the coupon is issued by a legitimate entity (in this case, your business), preventing fraudulent submissions.
3. **Non-Repudiation:** Guarantees that the sender cannot deny sending the data once it has been signed and submitted.

The digital signature is generated using a private key, and the fiscalization service verifies the signature using a corresponding public key. If the signature is valid, the coupon is considered authentic.

The steps that are needed to provide a valid signature are:

1. **Serialization:** First, the coupon (either a Citizen or POS coupon) is serialized into a Protobuf binary format. This format ensures that the data can be transmitted efficiently and consistently.
   ```
   byte[] posCouponProto = posCoupon.ToByteArray();
   ```
2. **Base64 Encoding:** The serialized Protobuf binary data is then encoded into a Base64 string. Base64 is a binary-to-text encoding scheme that makes it easy to transfer data as a string format.
   ```
   var base64EncodedProto = Convert.ToBase64String(posCouponProto);
   ```
3. **Hashing:** Before signing, the data is hashed using a SHA-256 cryptographic hash function. Hashing converts the coupon data into a fixed-length string of bytes, ensuring that even a small change in the original data will produce a completely different hash value.
   ```
   var sha256 = SHA256.Create();
   var hash = sha256.ComputeHash(base64EncodedProto);
   ```
4. **Signature Creation:** The hash is then signed using the ECDSA private key. This generates a digital signature, which is unique to the data and the private key. The fiscalization system can later verify this signature using the corresponding public key.
   ```
   var ecdsa = ECDsa.Create();
   ecdsa.ImportFromPem(_key);  // Load the private key
   var signature = ecdsa.SignHash(hash);
   ```
5. **Base64 Signature:** The generated signature is then encoded into a Base64 string, which makes it easy to include in the final request to the fiscalization service.
   ```
   var encodedSignature = Convert.ToBase64String(signature);
   ```

The Signer class digitally signs both Citizen and POS coupons using the **ECDSA** algorithm. A private key is loaded and used to create a signature over the serialized coupon data.

```
public string SignBytes(byte[] data)
{
    var ecdsa = ECDsa.Create();
    ecdsa.ImportFromPem(_key);
    var hash = SHA256.Create().ComputeHash(data);
    var signature = ecdsa.SignHash(hash);
    return Convert.ToBase64String(signature);
}
```

The return value is a **base64-encoded** signature.

### QR Code ###

Printed fiscal coupon needs to also have a QR Code that can be scanned by citizens to verify the authenticity of the receipt.

In the Fiscalization System, QR codes are generated based on the serialized and signed data of a Citizen Coupon. The data, once encoded into a QR code, is typically printed on the customer receipt.

#### QR Code Data Structure ####

In this implementation, the QR code contains:

1. The **Base64-encoded** serialized data of the [Citizen Coupon](#citizen-coupon).
2. The **Base64-encoded** digital signature of that data.

These two parts are combined into a single string, separated by a pipe | symbol, which forms the data to be encoded in the QR code.

#### QR Code Generation in Code ####

The following steps show how the QR code data is generated in the ```Fiskalizimi``` class using the ```CitizenCoupon``` model.

1. **Serialize the CitizenCoupon to Protobuf binary:** This ensures that the receipt data is in a compact binary format.
   ```
   byte[] citizenCouponProto = citizenCoupon.ToByteArray();
   ```
2. **Base64 encode the Protobuf data:** This converts the binary data into a Base64-encoded string, making it suitable for use in the QR code.
   ```
   var base64EncodedProto = Convert.ToBase64String(citizenCouponProto);
   ```
3. **Generate a digital signature:** Using the ECDSA private key, sign the Base64-encoded Protobuf data to ensure its authenticity and integrity.
   ```
   var base64EncodedBytes = Encoding.UTF8.GetBytes(base64EncodedProto);
   string signature = signer.SignBytes(base64EncodedBytes);
   ```
4. **Combine the data and signature:** The Base64-encoded coupon data and the Base64-encoded signature are concatenated with a pipe | symbol to form the final string, which will be encoded into a QR code.
   ```
   string qrCodeString = $"{base64EncodedProto}|{signature}";
   ```
5. **Print or display the QR code:** The resulting qrCodeString can now be encoded into a QR code and printed on the receipt or displayed on a screen.

![QR Code](qr.png)

Below is the method in the Fiskalizimi class that generates the QR code string for a Coupon:

```
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
```


## Sending Data to Fiscalization Service ##

### Sending Citizen Coupons ###

QR Code will be scanned by the Citizen Mobile App, which in turn will send the data to the Fiscalization System for verification. This method mimics the Citizen Mobile App, and it is used for testing purposes. The SendQrCode method sends the serialized and signed citizen coupon to the fiscalization service.    

This is how you prepare and submit the request:

```
public static async Task SendQrCode()
{
    var builder = new ModelBuilder();
    var signer = new Signer(PrivateKeyPem);
    var citizenCoupon = builder.GetCitizenCoupon();
    var qrCode = SignCitizenCoupon(citizenCoupon, signer);

    var request = new { citizen_id = 1, qr_code = qrCode };
    var response = await new HttpClient().PostAsJsonAsync(url, request);
    response.EnsureSuccessStatusCode();
}
```

### Sending POS Coupons ###

Similar to citizen coupons, you can send POS coupons with the SendPosCoupon method:

```
public static async Task SendPosCoupon()
{
    var builder = new ModelBuilder();
    var signer = new Signer(PrivateKeyPem);
    var posCoupon = builder.GetPosCoupon();
    var (coupon, signature) = SignPosCoupon(posCoupon, signer);

    var request = new { details = coupon, signature = signature };
    var response = await new HttpClient().PostAsJsonAsync(url, request);
    response.EnsureSuccessStatusCode();
}
```

## Running the Application ##

To execute the program and send the coupons:
```
dotnet run
```

Make sure to configure the correct URL of the fiscalization service and have a valid ECDSA private key to sign the coupons.