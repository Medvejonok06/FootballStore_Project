using MediatR;
using FootballStore.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

// ВИПРАВЛЕНО: Видаляємо using, які викликають CS0234:
// using FootballStore.Services.Features.Products.Queries;
// using FootballStore.Services.Features.Products.Commands;

// Тепер ми будемо використовувати повні імена класів або додавати коректні using,
// якщо ви не створили GetProductByIdQuery.

namespace FootballStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/Product (ЧИТАННЯ - Query)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(CancellationToken cancellationToken)
        {
            // Використовуємо повне ім'я класу, щоб компілятор його знайшов
            var query = new FootballStore.Services.Features.Products.Queries.GetAllProductsQuery(); 
            var products = await _mediator.Send(query, cancellationToken); 
            return Ok(products); 
        }

        // POST api/Product (ЗАПИС - Command)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto dto, CancellationToken cancellationToken)
        {
            // Використовуємо повне ім'я класу
            var command = new FootballStore.Services.Features.Products.Commands.CreateProductCommand { Dto = dto };
            
            var productDto = await _mediator.Send(command, cancellationToken); 
            
            return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
        }
        
        // GET api/Product/5 (Демонстрація Query для одного об'єкта)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProductById(int id, CancellationToken cancellationToken)
        {
            // Використовуємо повне ім'я класу
            var query = new FootballStore.Services.Features.Products.Queries.GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query, cancellationToken);
            
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}