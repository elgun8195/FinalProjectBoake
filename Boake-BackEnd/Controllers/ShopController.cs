﻿using Boake_BackEnd.DAL;
using Boake_BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boake_BackEnd.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int sortId, int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Books.Where(a => !a.IsDeleted && a.IsSale).Count() / 3);
            ViewBag.Types = _context.ProductTypes.Include(a=>a.BookTypes).ThenInclude(a=>a.Book).Where(a => !a.IsDeleted).ToList();
            ViewBag.Authors = _context.Authors.Include(a=>a.AuthorBooks).ThenInclude(a=>a.Book).Where(a => !a.IsDeleted).ToList();
            List<Book> model = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookCategories).ThenInclude(a => a.Category).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(a => !a.IsDeleted && a.IsSale).Skip((page - 1) * 3).Take(3).ToList(); 
            ViewBag.Count=_context.Books.Where(a => !a.IsDeleted && a.IsSale).ToList().Count();
            ViewBag.id = sortId;

            switch (sortId)
            {
                case 1:
                    model = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(a => !a.IsDeleted && a.IsSale).Skip((page - 1) * 3).Take(3).ToList();
                    break;
                case 2:
                    model = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(a => !a.IsDeleted && a.IsSale).Skip((page - 1) * 3).Take(3).OrderByDescending(s => s.Name).ToList();
                    break;
                case 3:
                    model = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(a => !a.IsDeleted && a.IsSale).Skip((page - 1) * 3).Take(3).OrderBy(s => s.Name).ToList();
                    break;
                case 4:
                    model = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(a => !a.IsDeleted && a.IsSale).Skip((page - 1) * 3).Take(3).OrderByDescending(s => s.Price).ToList();
                    break;
                case 5:
                    model = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(a => !a.IsDeleted && a.IsSale).Skip((page - 1) * 3).Take(3).OrderBy(s => s.Price).ToList();
                    break;
                default:

                    break;
            }
            return View(model);
        }
        public IActionResult Search(string search)
        {
            List<Book> products = _context.Books.Where(p =>!p.IsDeleted && p.IsSale && p.Name.ToLower().Contains(search.ToLower())).ToList();
            return PartialView("_SearchPartial", products);
        }
        public IActionResult FromTo(decimal from,decimal to)
        {
            List<Book> books = _context.Books.Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookCategories).ThenInclude(a => a.Category).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(p => !p.IsDeleted && p.IsSale==true && p.Price>=from &&p.Price<=to).ToList();
            ViewBag.Types = _context.ProductTypes.Include(a => a.BookTypes).ThenInclude(a => a.Book).Where(p => !p.IsDeleted).ToList();
            ViewBag.Authors = _context.Authors.Include(a => a.AuthorBooks).ThenInclude(a => a.Book).Where(p => !p.IsDeleted).ToList();
            ViewBag.Count = _context.Books.Where(a => !a.IsDeleted && a.IsSale).ToList().Count();
            return  View(  books);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Book book =await _context.Books.Include(a => a.BookTags).ThenInclude(a => a.Tag).Include(a => a.BookCategories).ThenInclude(a => a.Category).Include(a => a.AuthorBooks).ThenInclude(a => a.Author).Include(a => a.BookTypes).ThenInclude(a => a.ProductType).Where(b => !b.IsDeleted && b.IsSale && b.Id == id).FirstOrDefaultAsync();
            if (book==null)
            {
                return RedirectToAction(nameof(Index));
            }
            List<ProductType> types = _context.ProductTypes.Include(c => c.BookTypes).ThenInclude(bc => bc.Book).Where(b => b.BookTypes.Any(bc => bc.BookId == id)).ToList();

            List<Book> relatedBooks = new List<Book>();
            foreach (var item in types)
            {
                relatedBooks = _context.Books.Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author).Include(b => b.BookTags).ThenInclude(bt => bt.Tag).Include(b => b.BookTypes).ThenInclude(bc => bc.ProductType).Where(b => b.IsSale&&!b.IsDeleted&& b.BookTypes.Any(bc => bc.ProductTypeId == item.Id)).ToList();
            }
            ViewBag.RelatedBooks = relatedBooks;
            return View(book);
        }

        public IActionResult Modalim(int id)
        {
            Book book = _context.Books .Include(b => b.BookCategories).ThenInclude(b => b.Category).FirstOrDefault(b =>!b.IsDeleted&&b.IsSale&& b.Id == id);

            return PartialView("_ModalBook", book);
        }
    }
}
