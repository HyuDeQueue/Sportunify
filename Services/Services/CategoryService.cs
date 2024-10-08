﻿using Repositories.Models;
using Repositories.Repositories;

namespace Services.Services
{
    public class CategoryService
    {
        private CategoryRepository _categoryRepository = new();

        public Category GetCategoryById(int id)
        {
            return _categoryRepository.GetCategoryById(id);
        }
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
