using ChocolateFactory.Data;
using ChocolateFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateFactory.Repositories
{
    public interface IRawMaterialRepository
    {
        Task<RawMaterial> GetRawMaterialByNameAsync(string materialName);
        Task<RawMaterial> GetRawMaterialByBatchIdAsync(Guid materialBatchId);
        Task<List<RawMaterial>> GetAllRawMaterialsAsync();
        Task<IEnumerable<RawMaterial>> GetAllRawMaterialsByWarehouseNameAsync(string warehouseName);
        Task AddRawMaterialAsync(RawMaterial material);
        Task UpdateRawMaterialAsync(RawMaterial material);
        Task DeleteRawMaterialAsync(string name);
        Task DeleteRawMaterialByBatchAsync(Guid materialId);
    }

    public class RawMaterialRepository : IRawMaterialRepository
    {
        private readonly ApplicationDbContext _context;

        public RawMaterialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RawMaterial> GetRawMaterialByNameAsync(string materialName) =>
            await _context.RawMaterials.FindAsync(materialName);

        public async Task<RawMaterial> GetRawMaterialByBatchIdAsync(Guid id) => await _context.RawMaterials.FindAsync(id);

        public async Task<List<RawMaterial>> GetAllRawMaterialsAsync() =>
            await _context.RawMaterials.ToListAsync();

        public async Task<IEnumerable<RawMaterial>> GetAllRawMaterialsByWarehouseNameAsync(string warehouseName) =>
            await _context.RawMaterials.Where(x=>x.WarehouseName == warehouseName).ToListAsync();

        public async Task AddRawMaterialAsync(RawMaterial material)
        {
            await _context.RawMaterials.AddAsync(material);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRawMaterialAsync(RawMaterial material)
        {
            _context.RawMaterials.Update(material);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRawMaterialAsync(string name)
        {
            var maerial = await GetRawMaterialByNameAsync(name);
            if (maerial != null) {
                _context.RawMaterials.Remove(maerial);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRawMaterialByBatchAsync(Guid materialBatchId)
        {
            var material = await GetRawMaterialByBatchIdAsync(materialBatchId);
            if (material != null)
            {
                _context.RawMaterials.Remove(material);
                await _context.SaveChangesAsync();
            }
        }
    }

}
