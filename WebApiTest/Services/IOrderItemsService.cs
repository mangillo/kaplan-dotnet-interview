﻿
using System.Threading.Tasks;

using WebApiTest.Models;

namespace WebApiTest.Services
{
    public interface IOrderItemsService
    {
        Task<short> AddAsync(int orderID, OrderItemModel item);
        OrderItemsModel Get(int orderID);
        Task Delete(int orderID, OrderItemModel item);
    }
}
