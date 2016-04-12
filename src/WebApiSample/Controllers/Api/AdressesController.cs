using System.Collections.Generic;
using System.Web.Http;

namespace WebApiSample.Controllers.Api
{
    public class AddressController : ApiController
    {
        [RoutePrefix("Api/Adresses")]
        public class AdressesController : ApiController
        {
            [HttpGet]
            [Route("Address")]
            public IList<Address> Adresses()
            {
                return new List<Address>() { new Address() };
            }

            [HttpGet]
            [Route("Address")]
            public Address Address(int id)
            {
                return new Address();
            }

            [HttpPost]
            [Route("Address")]
            public void CreateAddress([FromBody]Address model)
            {
            }

            [HttpPut]
            [Route("Address")]
            public void EditAddress([FromBody]Address model)
            {
            }

            [HttpDelete]
            [Route("Address")]
            public void DeleteAddress(int id)
            {
            }
        }

        public class Address
        {
            public int Id { get; set; }

            public AddressType Type { get; set; }
        }

        public enum AddressType
        {
            Residential = 1,
            Commercial = 2
        }
    }
}
