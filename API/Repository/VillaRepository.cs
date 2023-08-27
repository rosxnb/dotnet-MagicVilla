using API.Data;
using API.Models;
using API.Repository.IRepository;

namespace API.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        private readonly AppDbContext _db;

        public VillaRepository(AppDbContext db)
            : base(db)
        {
            _db = db;
        }

        public async Task<Villa> UpdateAsync(Villa entity)
        {
            _ = _db.Villas.Update(entity);
            _ = await _db.SaveChangesAsync();
            return entity;
        }
    }
}
