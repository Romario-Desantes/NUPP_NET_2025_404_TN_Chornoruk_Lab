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
    public class RectanglesController : Controller
    {
        private readonly FigureContext _context;

        public RectanglesController(FigureContext context)
        {
            _context = context;
        }

        // GET: Rectangles
        public async Task<IActionResult> Index()
        {
            var figureContext = _context.Rectangles.Include(r => r.Collection);
            return View(await figureContext.ToListAsync());
        }

        // GET: Rectangles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rectangleModel = await _context.Rectangles
                .Include(r => r.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rectangleModel == null)
            {
                return NotFound();
            }

            return View(rectangleModel);
        }

        // GET: Rectangles/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description");
            return View();
        }

        // POST: Rectangles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Height,Width,Id,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] RectangleModel rectangleModel)
        {
            // Автоматично генеруємо DomainId, якщо він не вказаний
            if (rectangleModel.DomainId == Guid.Empty)
            {
                rectangleModel.DomainId = Guid.NewGuid();
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(rectangleModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", rectangleModel.CollectionId);
            return View(rectangleModel);
        }

        // GET: Rectangles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rectangleModel = await _context.Rectangles.FindAsync(id);
            if (rectangleModel == null)
            {
                return NotFound();
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", rectangleModel.CollectionId);
            return View(rectangleModel);
        }

        // POST: Rectangles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Height,Width,Id,DomainId,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] RectangleModel rectangleModel)
        {
            if (id != rectangleModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rectangleModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RectangleModelExists(rectangleModel.Id))
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
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", rectangleModel.CollectionId);
            return View(rectangleModel);
        }

        // GET: Rectangles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rectangleModel = await _context.Rectangles
                .Include(r => r.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rectangleModel == null)
            {
                return NotFound();
            }

            return View(rectangleModel);
        }

        // POST: Rectangles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rectangleModel = await _context.Rectangles.FindAsync(id);
            if (rectangleModel != null)
            {
                _context.Rectangles.Remove(rectangleModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RectangleModelExists(int id)
        {
            return _context.Rectangles.Any(e => e.Id == id);
        }
    }
}
