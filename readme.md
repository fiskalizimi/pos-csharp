# Fiscalization Integration with C# using Protobuf

This repository provides a C# implementation for integrating with a fiscalization system using classes generated by Protobuf. The process includes constructing fiscal receipts (Citizen and POS Coupons), digitally signing them, and submitting them to the fiscalization service. This guide walks you through the steps necessary to integrate and execute the solution.

## Table of Contents

- [Project Overview](#project-overview)
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
- [Model Explanation](#model-explanation)
    - [Citizen Coupon](#citizen-coupon)
    - [POS Coupon](#pos-coupon)
- [Digital Signing](#digital-signing)
- [Sending Data to Fiscalization Service](#sending-data-to-fiscalization-service)
    - [Sending Citizen Coupons](#sending-citizen-coupons)
    - [Sending POS Coupons](#sending-pos-coupons)
- [Running the Application](#running-the-application)

## Project Overview

This project provides a set of C# classes to interact with a fiscalization system. The key components include:
1. **ModelBuilder**: Constructs the Citizen and POS coupons (receipts) using predefined tax groups, items, and payment methods.
2. **Signer**: Signs the receipts using a digital signature created with an ECDSA private key.
3. **Fiskalizimi**: Contains methods for constructing, signing, and sending fiscal coupons to the fiscalization service.

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
* **TaxGroups** is an array of ```TaxGroup``` objects
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
* **TaxGroups** is an array of ```TaxGroup``` objects
* **TotalTax** is the amount of the tax in total that customer will have to pay
* **TotalNoTax** is the total amount without tax that customer will have to pay

**NOTE:** These details must match the [Citizen Coupon](#citizen-coupon) details, otherwise the coupon will be market as ```FAILED VERIFICATION``` !

## Key Generation ##

There are different ways to generate a PKI key pair, depending on the operating system. 

**WARNING!** Each POS system (PC/till) needs to have its own PKI key pair. The private key should never leave the machine that it is generated on !!!

We have provided a tool that simplifies the process a lot. To download the tool on you machine, click on one of the links below (depending on the operating system you are using):

* [atkcli for windows](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-cli-windows.zip)
* [atkcli for MacOS Intel](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-cli-macos-intel.zip)
* [atkcli for MacOS M1/M2](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-cli-macos-apple-silicon.zip)
* [atkcli for Linux](https://github.com/fiskalizimi/pos-csharp/raw/refs/heads/main/atkcli/atk-linux.zip)


## Digital Signing ##

### QR Code ###


## Sending Data to Fiscalization Service ##

### Sending Citizen Coupons ###

### Sending POS Coupons ###

## Running the Application ##
