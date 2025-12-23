using System.Collections;
using FigureProj.Common.Models.Abstract;
using FigureProj.Common.Services;
using FigureProj.Infrastructure.Mapping;
using FigureProj.Infrastructure.Models;
using FigureProj.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FigureProj.Infrastructure.Services
{
    /// <summary>
    /// CRUD сервіс з підтримкою бази даних через Repository pattern
    /// </summary>
    public class DbCrudServiceAsync<T> : ICrudServiceAsync<T> where T : Figure
    {
        private readonly FigureContext _context;
        private readonly IRepository<FigureModel> _repository;

        public DbCrudServiceAsync(FigureContext context, IRepository<FigureModel> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<bool> CreateAsync(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            // Обчислюємо площу та периметр
            element.CalculateArea();
            element.CalculatePerimetr();

            // Конвертуємо доменну модель у модель БД
            var dbModel = element.ToDbModel();
            
            await _repository.AddAsync(dbModel);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<T> ReadAsync(Guid id)
        {
            // Знаходимо фігуру в БД за DomainId (Guid)
            var dbModel = await _context.Figures
                .FirstOrDefaultAsync(f => f.DomainId == id);

            if (dbModel == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

            var domainModel = dbModel.ToDomainModel();
            return (T)domainModel;
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            var dbModels = await _repository.GetAllAsync();
            return dbModels.Select(m => (T)m.ToDomainModel()).ToList();
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Номер сторінки повинен бути більше 0.");
            
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Кількість елементів повинна бути більше 0.");

            var dbModels = await _context.Figures
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToListAsync();

            return dbModels.Select(m => (T)m.ToDomainModel()).ToList();
        }

        public async Task<bool> UpdateAsync(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            // Обчислюємо площу та периметр
            element.CalculateArea();
            element.CalculatePerimetr();

            // Знаходимо фігуру в БД за DomainId
            var dbModel = await _context.Figures
                .FirstOrDefaultAsync(f => f.DomainId == element.Id);

            if (dbModel == null)
                throw new KeyNotFoundException($"Елемент з ID {element.Id} не знайдено для оновлення.");

            // Оновлюємо модель БД
            dbModel.UpdateFromDomain(element);

            await _repository.Update(dbModel);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveAsync(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            // Знаходимо фігуру в БД за DomainId
            var dbModel = await _context.Figures
                .FirstOrDefaultAsync(f => f.DomainId == element.Id);

            if (dbModel == null)
                throw new KeyNotFoundException($"Елемент з ID {element.Id} не знайдено для видалення.");

            await _repository.Delete(dbModel);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var dbModels = _context.Figures.AsEnumerable();
            return dbModels.Select(m => (T)m.ToDomainModel()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

