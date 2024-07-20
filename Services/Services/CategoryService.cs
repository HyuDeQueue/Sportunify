﻿using Repositories.Models;
using Repositories.Repositories;
using System.Collections.Generic;

namespace Services.Services
{
    public class CategoryService
    {
        private CategoryRepository _categoryRepository = new();


        public List<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public void CreateCategory(Category category)
        {
            _categoryRepository.CreateCategory(category);
        }

        public void EditCategory(Category category)
        {
            _categoryRepository.EditCategory(category);
        }
    }
}
