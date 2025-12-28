using MongoDB.Driver;
using FigureProj.NoSql.Models;

namespace FigureProj.NoSql.Repositories
{
    public class MongoFigureRepository : IMongoRepository<FigureDocument>
    {
        private readonly IMongoCollection<FigureDocument> _collection;

        public MongoFigureRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<FigureDocument>("figures");
        }

        public async Task<FigureDocument?> GetByIdAsync(string id)
        {
            var filter = Builders<FigureDocument>.Filter.Eq(f => f.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FigureDocument>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(FigureDocument entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(string id, FigureDocument entity)
        {
            var filter = Builders<FigureDocument>.Filter.Eq(f => f.Id, id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<FigureDocument>.Filter.Eq(f => f.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<long> CountAsync()
        {
            return await _collection.CountDocumentsAsync(_ => true);
        }

        public async Task<IEnumerable<FigureDocument>> GetByTypeAsync(string type)
        {
            var filter = Builders<FigureDocument>.Filter.Eq(f => f.Type, type);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<FigureDocument>> GetByColorAsync(string color)
        {
            var filter = Builders<FigureDocument>.Filter.Eq(f => f.Color, color);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<FigureDocument>> GetByTagAsync(string tag)
        {
            var filter = Builders<FigureDocument>.Filter.AnyEq(f => f.Tags, tag);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}


