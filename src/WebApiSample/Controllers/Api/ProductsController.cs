using System.Collections.Generic;
using System.Web.Http;

namespace WebApiSample.Controllers.Api
{
    [RoutePrefix("Api/Products")]
    public class ProductsController : ApiController
    {
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
