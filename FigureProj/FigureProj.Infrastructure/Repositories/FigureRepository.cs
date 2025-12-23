using Microsoft.EntityFrameworkCore;
using FigureProj.Infrastructure.Models;

namespace FigureProj.Infrastructure.Repositories
{
    public class FigureRepository : Repository<FigureModel>
    {
        public FigureRepository(FigureContext context) : base(context)
        {
        }

        public override async Task<FigureModel?> GetByIdAsync(int id)
        {
            return await _context.Figures
                .Include(f => f.Metadata)
                .Include(f => f.Collection)
                .Include(f => f.FigureTags)
                    .ThenInclude(ft => ft.Tag)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public override async Task<IEnumerable<FigureModel>> GetAllAsync()
        {
            return await _context.Figures
                .Include(f => f.Metadata)
                .Include(f => f.Collection)
                .Include(f => f.FigureTags)
                    .ThenInclude(ft => ft.Tag)
                .ToListAsync();
        }

        public async Task<IEnumerable<FigureModel>> GetByCollectionIdAsync(int collectionId)
        {
            return await _context.Figures
                .Where(f => f.CollectionId == collectionId)
                .Include(f => f.Metadata)
                .Include(f => f.FigureTags)
                    .ThenInclude(ft => ft.Tag)
                .ToListAsync();
        }

        public async Task<IEnumerable<FigureModel>> GetByTagAsync(string tagName)
        {
            return await _context.Figures
                .Include(f => f.FigureTags)
                    .ThenInclude(ft => ft.Tag)
                .Where(f => f.FigureTags.Any(ft => ft.Tag.Name == tagName))
                .ToListAsync();
        }

        public async Task<IEnumerable<FigureModel>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Figures
                .Include(f => f.Metadata)
                .Include(f => f.Collection)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}

