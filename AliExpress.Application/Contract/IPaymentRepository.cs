﻿using AliExpress.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliExpress.Application.Contract
{
    public interface IPaymentRepository:IRepository<PaymentMethod,int>
    {
       // Task<IEnumerable<Product>> GetAllAsync(string searchValue, int page, int pageSize);
        Task<int> CoutPayemnt();
    }
}
