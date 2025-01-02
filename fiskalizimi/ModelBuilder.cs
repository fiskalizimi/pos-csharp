using Atk;

namespace fiskalizimi;



public class ModelBuilder
{
    public CitizenCoupon GetCitizenCoupon()
    {
        var citizenCoupon = new CitizenCoupon
        {
            BusinessId = 60100,
            PosId = 1,
            CouponId = 10,
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


    public PosCoupon GetPosCoupon()
    {
        var posCoupon = new PosCoupon
        {
            BusinessId = 60100,
            CouponId = 10,
            BranchId = 1,
            Location = "Prishtine",
            OperatorId = "Kushtrimi",
            PosId = 1,
            ApplicationId = 1234,
            VerificationNo = "1234567890123456",
            Type = CouponType.Sale,
            Time = new DateTimeOffset(2024, 10, 1, 15,30, 20, TimeSpan.Zero).ToUnixTimeSeconds(),
            Items =
            {
                new CouponItem { Name = "uje rugove", Price = 150, Unit = "cope", Quantity = 3, Total = 450, TaxRate = "C", Type = "TT" },
                new CouponItem { Name = "sendviq", Price = 300, Unit = "cope", Quantity = 2, Total = 600, TaxRate = "E", Type = "TT" },
                new CouponItem { Name = "buke", Price = 80, Unit = "cope", Quantity = 4, Total = 320, TaxRate = "D", Type = "TT" },
                new CouponItem { Name = "machiato e madhe", Unit = "cope", Price = 150, Quantity = 3, Total = 450, TaxRate = "E", Type = "TT" }
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
}