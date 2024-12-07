using ChocolateFactory.Models;
using ChocolateFactory.Repositories;

namespace ChocolateFactory.Services
{
    public class QualityControlService
    {
        private readonly QualityCheckRepository _repository;

        public QualityControlService(QualityCheckRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<QualityCheck>> GetAllChecksAsync()
        {
            return await _repository.GetAllChecksAsync();
        }

        public async Task AddQualityCheckAsync(QualityCheck check)
        {
            await _repository.AddCheckAsync(check);
        }
    }
}
