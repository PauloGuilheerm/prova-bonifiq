using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Controllers
{
	
	[ApiController]
	[Route("[controller]")]
	public class Parte2Controller :  ControllerBase
	{
		/// <summary>
		/// Precisamos fazer algumas alterações:
		/// 1 - Não importa qual page é informada, sempre são retornados os mesmos resultados. Faça a correção.
		/// 2 - Altere os códigos abaixo para evitar o uso de "new", como em "new ProductService()". Utilize a Injeção de Dependência para resolver esse problema
		/// 3 - Dê uma olhada nos arquivos /Models/CustomerList e /Models/ProductList. Veja que há uma estrutura que se repete. 
		/// Como você faria pra criar uma estrutura melhor, com menos repetição de código? E quanto ao CustomerService/ProductService. Você acha que seria possível evitar a repetição de código?
		/// 
		/// </summary>
		private readonly TestDbContext _ctx;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;

        public Parte2Controller(TestDbContext ctx, IProductService productService, ICustomerService customerService)
        {
			_ctx = ctx;
            _productService = productService;
            _customerService = customerService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<PageList<Product>>> GetProducts([FromQuery] int page = 1, CancellationToken ct = default)
        {
            var result = await _productService.GetPagedDataAsync(page, 10, ct);
            return Ok(result);
        }

        [HttpGet("customers")]
        public async Task<ActionResult<PageList<Customer>>> GetCustomers([FromQuery] int page = 1, CancellationToken ct = default)
        {
            var result = await _customerService.GetPagedDataAsync(page, 10, ct);
            return Ok(result);
        }
    }
}
