using API.Data;
using API.Models;
using API.Repository.IRepository;

namespace API.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly AppDbContext _db;

        public VillaNumberRepository(AppDbContext db)
            : base(db)
        {
            _db = db;
        }

        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            _ = _db.VillaNumbers.Update(entity);
            _ = await _db.SaveChangesAsync();
            return entity;
        }
    }
}
