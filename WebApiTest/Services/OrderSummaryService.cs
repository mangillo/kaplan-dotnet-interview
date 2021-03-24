using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApiTest.Data;
using WebApiTest.Models;

namespace WebApiTest.Services
{
    public class OrderSummaryService : IOrderSummaryService
    {
        public OrderSummaryService(TestDbContext testDB)
        {
            _testDB = testDB;
        }

        public async Task<OrderSummaryModel> GetAsync(int orderID)
        {
            Order order = await _testDB.Orders
                                       .Where(o => o.OrderID == orderID)
                                       .Include(o => o.OrderItems).FirstOrDefaultAsync();

            OrderSummaryModel summary = null;

            if (order != null)
            {
                summary = new OrderSummaryModel()
                {
                    OrderID = orderID,
                    Totals = new OrderTotalsModel()
                    {
                        Shipping = Shipping
                    }
                };

                List<int> personIDs = new List<int>();
                decimal subTotal = 0;
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    personIDs.Add(orderItem.StudentPersonID);
                    subTotal += orderItem.Price;
                }

                summary.Totals.SubTotal = subTotal;
                summary.Totals.Tax = subTotal * TaxRate;
                summary.Totals.Shipping = Shipping;

                summary.OrderingPersonID = order.OrderingPersonID; // we cannot use single() here as there are duplicate values and single() works only when there is exactly one element in sequence
                                                                   // but why we are taking here personIds.First()?? since we want ordering person id , we are getting this from order only

                summary.OrderPersonIDs = personIDs;
            }

            return summary;
        }

        private readonly TestDbContext _testDB;

        private const decimal TaxRate = 0.055m;
        private const decimal Shipping = 15.0m;
    }
}
