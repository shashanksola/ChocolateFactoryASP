using ChocolateFactory.Models;
using ChocolateFactory.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Any;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace ChocolateFactory.Services
{
    public class WarehouseService
    {
        private readonly WarehouseRepository _repository;

        public WarehouseService(WarehouseRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Warehouse>> GetAllWarehousesAsync()
        {
            return await _repository.GetAllWarehousesAsync();
        }

        public async Task AddWarehouseAsync(Warehouse warehouse)
        {
            await _repository.AddWarehouseAsync(warehouse);
        }

        public async Task DeleteWarehouseAsync(string warehouse)
        {
            await _repository.DeleteWarehouseAsync(warehouse);
        }

        public async Task<Warehouse> GetWarehouseByNameAsync(string name)
        {
            return await _repository.GetWarehouseByNameAsync(name);
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse) {
            await _repository.UpdateWarehouseAsync(warehouse);
        }
    }
}
