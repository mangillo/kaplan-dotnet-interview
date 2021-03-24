
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WebApiTest.Models;
using WebApiTest.Services;
using WebApiTest.Web.Controllers;

namespace WebApiTest.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private IOrderItemsService mockOrderItemService;
        private ILogger<OrdersController> mockLogger;
        private OrdersController controller;

        [SetUp]
        public void SetUp()
        {
            mockOrderItemService = Substitute.For<IOrderItemsService>();
            mockLogger = Substitute.For<ILogger<OrdersController>>();
            controller = new OrdersController(mockOrderItemService, mockLogger);
        }

        [Test]
        public void That_Get_ReturnsOrderItems_WhenOrderIdExists()
        {

            var orderId = 101;
            var expected = new OrderItemsModel
            {
                OrderID = orderId,
                Items = new List<OrderItemModel>
                {
                    new OrderItemModel
                    {
                        LineNumber = 0,
                        ProductID = 1,
                        StudentPersonID = 1,
                        Price = 150
                    }
                }
            };
            mockOrderItemService.Get(Arg.Any<int>()).Returns(expected);
            var result = (OrderItemsModel)(controller.Get(orderId) as OkObjectResult).Value;
            Assert.That(result.OrderID, Is.EqualTo(expected.OrderID));
            Assert.That(result.Items.Count, Is.EqualTo(expected.Items.Count()));
            foreach (var item in expected.Items)
            {
                Assert.True(result.Items.Contains(item));
            }

        }

        [Test]
        public void That_Get_ReturnsNotFoundError_WhenOrderIdDoesNotExists()
        {
            var orderId = 100;
            mockOrderItemService.Get(Arg.Any<int>()).Returns((OrderItemsModel)null);
            var result = controller.Get(orderId) as NotFoundResult;
            Assert.That(result.StatusCode, Is.EqualTo(404));

        }
    }
}
