using FootballStore.Data.Ado;
using FootballStore.Data.Ado.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        // Впровадження залежностей (UoW)
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/Product (Асинхронність + CancellationToken - 1.00 балів)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts(CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            return Ok(products); // 200 OK
        }

        // GET api/Product/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            
            if (product == null)
            {
                // Повертаємо 404 Not Found
                return NotFound(); 
            }
            
            // Повертаємо 200 OK
            return Ok(product);
        }

        // POST api/Product
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad Request
            }

            try
            {
                // Додавання до БД
                var id = await _unitOfWork.Products.AddAsync(product, cancellationToken);
                product.Id = id;
                
                // Фіксація транзакції
                await _unitOfWork.CommitAsync(); 

                // Повертаємо 201 Created
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                // У разі помилки, UoW відкатить транзакцію під час Dispose
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); 
            }
        }
        
        // PUT api/Product/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product product, CancellationToken cancellationToken)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                var updated = await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
                
                if (!updated)
                {
                    await _unitOfWork.RollbackAsync();
                    return NotFound(); // 404, якщо товар не знайдено
                }

                await _unitOfWork.CommitAsync();
                
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}