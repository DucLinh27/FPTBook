﻿using Microsoft.AspNetCore.Mvc;
using WebApplication123.Models;
using WebApplication123.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication123.ModelsCRUD.Category;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using WebApplication123.Utils;

namespace WebApplication123.Controllers
{
    [Area(SD.AuthenticatedArea)]
    [Authorize(Roles = SD.StoreOwnerRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;

        public CategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> CategoryIndex()
        {
            var category = await context.Categories.ToListAsync();
            return View(category);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(AddCategoryViewModel CategoryModel)
        {
            var category = new Category()
            {
                Name = CategoryModel.Name,
                Description = CategoryModel.Description


            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return RedirectToAction("CategoryIndex");
        }
        [HttpGet]
        public async Task<IActionResult> ViewCategory(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);

            if (category != null)
            {
                var viewmodel = new UpdateCategoryView()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description
                };

                return await Task.Run(() => View("ViewCategory", viewmodel));
            }

            return RedirectToAction("CategoryIndex");
        }
        [HttpPost]
        public async Task<IActionResult> ViewCategory(UpdateCategoryView model)
        {
            var category = await context.Categories.FindAsync(model.CategoryId);
            if (category != null)
            {
                category.Name = model.Name;
                category.Description = model.Description;

                await context.SaveChangesAsync();

                return RedirectToAction("CategoryIndex");
            }

            return RedirectToAction("CategoryIndex");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(UpdateCategoryView model)
        {
            var category = await context.Categories.FindAsync(model.CategoryId);

            if (category != null)
            {
                context.Categories.Remove(category);


                await context.SaveChangesAsync();

                return RedirectToAction("CategoryIndex");
            }

            return RedirectToAction("CategoryIndex");
        }
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.IsApproved = true;
            context.SaveChanges();

            // code to notify store owner of category approval here

            return RedirectToAction(nameof(CategoryIndex));
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            // code to notify store owner of category rejection here

            return RedirectToAction(nameof(CategoryIndex));
        }
    }
}