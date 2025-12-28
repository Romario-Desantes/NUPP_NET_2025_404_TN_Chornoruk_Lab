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
    public class TrianglesController : Controller
    {
        private readonly FigureContext _context;

        public TrianglesController(FigureContext context)
        {
            _context = context;
        }

        // GET: Triangles
        public async Task<IActionResult> Index()
        {
            var figureContext = _context.Triangles.Include(t => t.Collection);
            return View(await figureContext.ToListAsync());
        }

        // GET: Triangles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var triangleModel = await _context.Triangles
                .Include(t => t.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (triangleModel == null)
            {
                return NotFound();
            }

            return View(triangleModel);
        }

        // GET: Triangles/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description");
            return View();
        }

        // POST: Triangles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("A,B,C,Id,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] TriangleModel triangleModel)
        {
            // Автоматично генеруємо DomainId, якщо він не вказаний
            if (triangleModel.DomainId == Guid.Empty)
            {
                triangleModel.DomainId = Guid.NewGuid();
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(triangleModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", triangleModel.CollectionId);
            return View(triangleModel);
        }

        // GET: Triangles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var triangleModel = await _context.Triangles.FindAsync(id);
            if (triangleModel == null)
            {
                return NotFound();
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", triangleModel.CollectionId);
            return View(triangleModel);
        }

        // POST: Triangles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("A,B,C,Id,DomainId,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] TriangleModel triangleModel)
        {
            if (id != triangleModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(triangleModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TriangleModelExists(triangleModel.Id))
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
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", triangleModel.CollectionId);
            return View(triangleModel);
        }

        // GET: Triangles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var triangleModel = await _context.Triangles
                .Include(t => t.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (triangleModel == null)
            {
                return NotFound();
            }

            return View(triangleModel);
        }

        // POST: Triangles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var triangleModel = await _context.Triangles.FindAsync(id);
            if (triangleModel != null)
            {
                _context.Triangles.Remove(triangleModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TriangleModelExists(int id)
        {
            return _context.Triangles.Any(e => e.Id == id);
        }
    }
}
