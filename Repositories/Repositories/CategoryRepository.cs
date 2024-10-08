﻿using Repositories.Models;

namespace Repositories.Repositories
{
    public class CategoryRepository
    {
        private ListentogetherContext _context;
        public Category GetCategoryById(int id)
        {
            _context = new();
            return _context.Categories.First(cate => cate.CategoryId == id);
        }

        public List<Category> GetAllCategories()
        {
            _context = new();
            return _context.Categories.ToList();
        }

        public void CreateCategory(Category category)
        {
            _context = new();
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void EditCategory(Category category)
        {
            _context = new();
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
    }
}
