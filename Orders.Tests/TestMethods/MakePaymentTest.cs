using System.Threading.Tasks;
using NUnit.Framework;
using Orders.Application.Features.Orders.Commands.PlaceOrder;
using Orders.Payment;

namespace Orders.Tests.TestMethods
{
    public class MakePaymentTest
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public async Task TestMakePayment()
        {
            var paymentDetails = new PaymentDetails()
            {
                CardNumber = "4242424242424242",
                Month = 10,
                Year = 2022,
                CVV = "123",
                Value = 500
            };

            var result = await MakePayment.PayAsync(paymentDetails);

            Assert.AreEqual("success", result);
        }
    }
}
