﻿using AliExpress.Dtos.Category;
using AliExpress.Dtos.Product;
using AliExpress.Dtos.ViewResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliExpress.Application.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategory();
        Task<ResultView<CategoryDto>> Create(CategoryDto categoryDto);
        Task<ResultView<CategoryDto>> Update(CategoryDto categoryDto);
        Task<ResultView<CategoryDto>> Delete(int id);
        Task<ResultView<CategoryDto>> GetOne(int Id);

        Task<IEnumerable<ProductViewDto>> GetAllProductsByCategory(int cateId);
        Task<IEnumerable<ProductViewDto>> GetAllProductsByNameCategory(string Name, int num);
    }
}
