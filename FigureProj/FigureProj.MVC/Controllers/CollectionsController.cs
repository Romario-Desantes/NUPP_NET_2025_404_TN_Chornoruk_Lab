using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FigureProj.Infrastructure;
using FigureProj.Infrastructure.Models;

namespace FigureProj.MVC.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly FigureContext _context;

        public CollectionsController(FigureContext context)
        {
            _context = context;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collections.ToListAsync());
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionModel = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collectionModel == null)
            {
                return NotFound();
            }

            return View(collectionModel);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,CreatedAt")] CollectionModel collectionModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collectionModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collectionModel);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionModel = await _context.Collections.FindAsync(id);
            if (collectionModel == null)
            {
                return NotFound();
            }
            return View(collectionModel);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedAt")] CollectionModel collectionModel)
        {
            if (id != collectionModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collectionModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionModelExists(collectionModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(collectionModel);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionModel = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collectionModel == null)
            {
                return NotFound();
            }

            return View(collectionModel);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collectionModel = await _context.Collections.FindAsync(id);
            if (collectionModel != null)
            {
                _context.Collections.Remove(collectionModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionModelExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }
    }
}
