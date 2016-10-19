using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApiSample.Models;

namespace WebApiSample.Controllers.Api
{
    [RoutePrefix("Api/Products")]
    public class ProductsController : ApiController
    {
        [HttpGet]
        [Route("PagedProducts")]
        public PagedResult<Product> PagedProducts(PagingOptions options)
        {
            var items = Enumerable.Range(1, 10000)
                .Select(a => new Product
                {
                    Id = a,
                    Price = a * 5
                })
                .ToList();

            return new PagedResult<Product>
            {
                Items = items.OrderBy(i => i.Price).Skip((options.Page - 1) * options.PageSize).Take(options.PageSize).ToList(),
                Count = items.ToList().Count
            };
        }

        [HttpGet]
        [Route("Product")]
        public IList<Product> Products()
        {
            return new List<Product>() { new Product() };
        }

        [HttpGet]
        [Route("Product")]
        public Product Product(int id)
        {
            return new Product();
        }

        [HttpPost]
        [Route("Product")]
        public void CreateProduct([FromBody]Product model)
        {
        }

        [HttpPut]
        [Route("Product")]
        public void EditProduct([FromBody]Product model)
        {
        }

        [HttpDelete]
        [Route("Product")]
        public void DeleteProduct(int id)
        {
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public decimal Price { get; set; }
    }
}
