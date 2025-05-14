using AutoMapper;
using ECommercePlateform.Application.DTOs;
using ECommercePlateform.Application.Interfaces;
using ECommercePlateform.Application.Interfaces.IProduct;
using ECommercePlateform.Domain.Entities;

namespace ECommercePlateform.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.IsAvailable = true;
            product.CreatedOn = DateTime.Now;
            product.IsActive = true;

            var result = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(result);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            // Soft delete
            product.IsDeleted = true;
            product.IsActive = false;
            product.ModifiedOn = DateTime.Now;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.FindAsync(p => p.IsActive && !p.IsDeleted);
            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IReadOnlyList<ProductDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductWithDetailsAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id);
            if (product == null || product.IsDeleted)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(Guid id, UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            _mapper.Map(updateProductDto, product);
            product.ModifiedOn = DateTime.Now;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
        }
    }
}