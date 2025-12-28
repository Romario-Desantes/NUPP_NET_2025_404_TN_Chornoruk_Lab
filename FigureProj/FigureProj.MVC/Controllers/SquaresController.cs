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
    public class SquaresController : Controller
    {
        private readonly FigureContext _context;

        public SquaresController(FigureContext context)
        {
            _context = context;
        }

        // GET: Squares
        public async Task<IActionResult> Index()
        {
            var figureContext = _context.Squares.Include(s => s.Collection);
            return View(await figureContext.ToListAsync());
        }

        // GET: Squares/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var squareModel = await _context.Squares
                .Include(s => s.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (squareModel == null)
            {
                return NotFound();
            }

            return View(squareModel);
        }

        // GET: Squares/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description");
            return View();
        }

        // POST: Squares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Side,Id,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] SquareModel squareModel)
        {
            // Автоматично генеруємо DomainId, якщо він не вказаний
            if (squareModel.DomainId == Guid.Empty)
            {
                squareModel.DomainId = Guid.NewGuid();
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(squareModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", squareModel.CollectionId);
            return View(squareModel);
        }

        // GET: Squares/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var squareModel = await _context.Squares.FindAsync(id);
            if (squareModel == null)
            {
                return NotFound();
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", squareModel.CollectionId);
            return View(squareModel);
        }

        // POST: Squares/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Side,Id,DomainId,Name,Color,Area,Perimeter,CreatedAt,CollectionId")] SquareModel squareModel)
        {
            if (id != squareModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(squareModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SquareModelExists(squareModel.Id))
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
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Description", squareModel.CollectionId);
            return View(squareModel);
        }

        // GET: Squares/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var squareModel = await _context.Squares
                .Include(s => s.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (squareModel == null)
            {
                return NotFound();
            }

            return View(squareModel);
        }

        // POST: Squares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var squareModel = await _context.Squares.FindAsync(id);
            if (squareModel != null)
            {
                _context.Squares.Remove(squareModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SquareModelExists(int id)
        {
            return _context.Squares.Any(e => e.Id == id);
        }
    }
}
