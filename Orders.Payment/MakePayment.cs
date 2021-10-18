using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orders.Application.Features.Orders.Commands.PlaceOrder;
using Stripe;

namespace Orders.Payment
{
    public class MakePayment
    {
        public static async Task<dynamic> PayAsync(PaymentDetails payment)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51EyOWpKu7vMLlELF5j90sCvYI1YBL3XBuEgFecC5B0J9zrmU2CVeePhKwcL4nTCkrrdNd4i5sHscmY79gw5khYcd00j9sjPAPo";

                var optionsToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = payment.CardNumber,
                        ExpMonth = payment.Month,
                        ExpYear = payment.Year,
                        Cvc = payment.CVV
                    }
                };

                var serviceToken = new TokenService();
                Token stripeToken = await serviceToken.CreateAsync(optionsToken);

                var options = new ChargeCreateOptions
                {
                    Amount = payment.Value,
                    Currency = "eur",
                    Description = "Test Description of the test payment.",
                    Source = stripeToken.Id
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                if (charge.Paid) return "success";
                else return "fail";

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
