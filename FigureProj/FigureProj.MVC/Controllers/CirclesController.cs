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
    public class CirclesController : Controller
    {
        private readonly FigureContext _context;

        public CirclesController(FigureContext context)
        {
            _context = context;
        }

        // GET: Circles
        public async Task<IActionResult> Index()
        {
            var figureContext = _context.Circles.Include(c => c.Collection);
            return View(await figureContext.ToListAsync());
        }

        // GET: Circles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var circleModel = await _context.Circles
                .Include(c => c.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (circleModel == null)
            {
                return NotFound();
            }

            return View(circleModel);
        }

        // GET: Circles/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description");
            return View();
        }

        // POST: Circles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Radius,Id,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] CircleModel circleModel)
        {
            // Автоматично генеруємо DomainId, якщо він не вказаний
            if (circleModel.DomainId == Guid.Empty)
            {
                circleModel.DomainId = Guid.NewGuid();
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(circleModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", circleModel.CollectionId);
            return View(circleModel);
        }

        // GET: Circles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var circleModel = await _context.Circles.FindAsync(id);
            if (circleModel == null)
            {
                return NotFound();
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", circleModel.CollectionId);
            return View(circleModel);
        }

        // POST: Circles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Radius,Id,DomainId,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] CircleModel circleModel)
        {
            if (id != circleModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(circleModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CircleModelExists(circleModel.Id))
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
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", circleModel.CollectionId);
            return View(circleModel);
        }

        // GET: Circles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var circleModel = await _context.Circles
                .Include(c => c.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (circleModel == null)
            {
                return NotFound();
            }

            return View(circleModel);
        }

        // POST: Circles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var circleModel = await _context.Circles.FindAsync(id);
            if (circleModel != null)
            {
                _context.Circles.Remove(circleModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CircleModelExists(int id)
        {
            return _context.Circles.Any(e => e.Id == id);
        }
    }
}
