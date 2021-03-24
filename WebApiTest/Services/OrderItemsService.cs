using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApiTest.Data;
using WebApiTest.Models;

namespace WebApiTest.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        public OrderItemsService(TestDbContext dbContext)
        {
            testDbContext = dbContext;
        }

        public OrderItemsModel Get(int orderID)
        {

            var order = testDbContext.Orders
                        .Include(o => o.OrderItems)
                        .SingleOrDefault(o => o.OrderID == orderID);

            OrderItemsModel orderItems = null;

            if (order != null)
            {
                orderItems = new OrderItemsModel
                {
                    OrderID = orderID,
                    Items = order.OrderItems.Select(oi => new OrderItemModel
                    {
                        LineNumber = oi.LineNumber,
                        ProductID = oi.ProductID,
                        StudentPersonID = oi.StudentPersonID,
                        Price = oi.Price
                    })
                };
            }
            return orderItems;
        }

        public async Task<short> AddAsync(int orderID, OrderItemModel item)
        {
            if (item.LineNumber != 0)
            {
                throw new ValidationException("LineNumber is generated and cannot be specified");
            }

            /*here the error is happenning as for orderitem , key is {oderId, lineNo} and this key already exists
             that's why we need to take the max existing line no of order item for order and increment it by 1 so that we can get the new key */
            var lineNumber = (short)(testDbContext.OrderItems.Where(oi => oi.OrderID == orderID).Max(oi => oi.LineNumber) + 1);

            await testDbContext.OrderItems.AddAsync(new OrderItem
            {
                OrderID = orderID,
                LineNumber = lineNumber,
                Price = item.Price,
                ProductID = item.ProductID,
                StudentPersonID = item.StudentPersonID
            });

            await testDbContext.SaveChangesAsync();

            return lineNumber;
        }

        public async Task Delete(int orderID, OrderItemModel item)
        {

            var orderItem = testDbContext.OrderItems.SingleOrDefault(oi => oi.OrderID == orderID && oi.LineNumber == item.LineNumber);
            if (orderItem != null)
            {
                testDbContext.OrderItems.Remove(orderItem);
            }
            await testDbContext.SaveChangesAsync();
        }

        private readonly TestDbContext testDbContext;
    }
}
