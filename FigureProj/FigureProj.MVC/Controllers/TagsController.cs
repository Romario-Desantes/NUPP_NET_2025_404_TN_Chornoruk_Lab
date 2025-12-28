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
    public class TagsController : Controller
    {
        private readonly FigureContext _context;

        public TagsController(FigureContext context)
        {
            _context = context;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tags.ToListAsync());
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagModel = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tagModel == null)
            {
                return NotFound();
            }

            return View(tagModel);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Color,CreatedAt")] TagModel tagModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tagModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tagModel);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagModel = await _context.Tags.FindAsync(id);
            if (tagModel == null)
            {
                return NotFound();
            }
            return View(tagModel);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Color,CreatedAt")] TagModel tagModel)
        {
            if (id != tagModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tagModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagModelExists(tagModel.Id))
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
            return View(tagModel);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagModel = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tagModel == null)
            {
                return NotFound();
            }

            return View(tagModel);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tagModel = await _context.Tags.FindAsync(id);
            if (tagModel != null)
            {
                _context.Tags.Remove(tagModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagModelExists(int id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}
