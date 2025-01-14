﻿using AliExpress.Application.Contract;
using AliExpress.Context;
using AliExpress.Models;
using AliExpress.Models.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace None.Infrastructure
{
    public class OrderRepository:IOrderRepository
    {
        private readonly AliExpressContext _context;

        public OrderRepository(AliExpressContext context)
        {
            _context = context;
        }


        public async Task<Order> CreateOrderAsync(int cartId, int deleveryMethodId, AppUser appUser)
        {
            //var cart = await _context.Carts.FirstOrDefaultAsync(c =>c.CartId == cartId);
            var cart =  _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.CartId == cartId);
            var orderItems = new List<OrderItem>();
            if (cart?.CartItems?.Count > 0)
            {
                foreach (var item in cart.CartItems)
                {
                    var product =  _context.Products.Find(item.ProductId);
                    var orderItem = new OrderItem(product, item.Quantity, item.Product.Price);
                    orderItems.Add(orderItem);
                    product.quantity--;
                     _context.SaveChanges();
                }

            }
            decimal total = 0.0m;

            //calc subtotal
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);
            if (cart.CartItems?.Count > 0)
              
            {
               
                foreach (var item in cart.CartItems)
                {
                    var product =  _context.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        total += (item.Quantity * product.Price);
                    }
                  
                }
            }
           
            var deliveryMethod =  _context.DeliveryMethods.FirstOrDefault(d => d.Id == deleveryMethodId);
            //create order
            var order = new Order(deliveryMethod, appUser, orderItems, total);
            _context.Orders.Add(order);
             _context.SaveChanges();
            return order;
        }



        public async Task DeleteAsync(Order order)
        {
            var products=order.OrderItems.Select(item => item.Product).ToList();
            foreach(var product in products)
            {
                product.quantity++;
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
           
            var orders = await _context.Orders
        .Include(o => o.DeliveryMethod)
        .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
        .Include(o => o.AppUser)
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();
            return orders;

        }

        public async Task<AppUser> GetAppUserAsync(string userId)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return appUser;
        }

        //public async Task<Order> GetByUserIdAsync(string userId)
        //{
        //    var order = await _context.Orders
        //.Include(o => o.OrderItems)
        //    .ThenInclude(oi => oi.Product)
        //    .ThenInclude(i => i.Images)// Include the related Product entity
        //.Include(o => o.DeliveryMethod)
        //.Where(o => o.AppUserId == userId)
        //.OrderByDescending(o => o.CreatedAt)
        //.FirstOrDefaultAsync();
        //    return order;
        //}
        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
          var order = await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ThenInclude(i => i.Images)// Include the related Product entity
        .Include(o => o.DeliveryMethod)
        .Include(o => o.AppUser)
        .Where(o => o.AppUserId == userId)
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();
          return order;
        }

        public async Task<IEnumerable<DeliveryMethod>> GetDeliveryMethods()
        {
            var delveryMethods = await _context.DeliveryMethods.ToListAsync();
            return delveryMethods;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            //var order = await _context.Orders.Include(o => o.DeliveryMethod).FirstOrDefaultAsync(o => o.Id == orderId);
            //return order;
            //var order = await _context.Orders
            //                .Include(o => o.DeliveryMethod)
            //                .Include(o => o.OrderItems)
            //                .Include(o => o.AppUser)
            //                .FirstOrDefaultAsync(o => o.Id == orderId);
            //return order;
            var order = await _context.Orders
                              .Include(o => o.DeliveryMethod)
                              .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                              .Include(o => o.AppUser)
                              .FirstOrDefaultAsync(o => o.Id == orderId);
            return order;
        }

        public async Task<Order> GetOrderLastAsync(string userId)
        {
            var order = await _context.Orders
         .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Product)
         .ThenInclude(i => i.Images)// Include the related Product entity
         .Include(o => o.DeliveryMethod)
         .Include(o => o.AppUser)
         .Where(o => o.AppUserId == userId)
         .OrderByDescending(o => o.CreatedAt)
         .FirstOrDefaultAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
        }
        //public async Task UpdateAsync(Order order)
        //{
        //    var oldOrder = await _context.Orders.FindAsync(order.Id); 
        //    if (oldOrder != null)
        //    {
        //        _context.Entry(oldOrder).Property(o => o.AppUserId).IsModified = false; 
        //        _context.Entry(oldOrder).CurrentValues.SetValues(order);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
