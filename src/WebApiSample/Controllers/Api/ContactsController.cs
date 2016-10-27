using System.Collections.Generic;
using System.Web.Http;

namespace WebApiSample.Controllers.Api
{
    [RoutePrefix("Api/Contacts")]
    public class ContactsController : ApiController
    {
        [HttpGet]
        [Route("Contact")]
        public IList<Contact> Contacts()
        {
            return new List<Contact>() { new Contact() };
        }

        [HttpGet]
        [Route("Contact")]
        public Contact Contact(int id)
        {
            return new Contact();
        }

        [HttpPost]
        [Route("Contact")]
        public void CreateContact([FromBody]Contact model)
        {
        }

        [HttpPut]
        [Route("Contact")]
        public void EditContact([FromBody]Contact model)
        {
        }

        [HttpDelete]
        [Route("Contact")]
        public void DeleteContact(int id)
        {
        }

        [HttpPost]
        [Route("ContactDescription")]
        public void ContactDescription(int id, [FromBody]string description)
        {
        }
    }
    
    public class Contact
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public ContactType Type { get; set; }
    }

    public enum ContactType
    {
        Phone = 1,
        Email = 2,
        Site = 3
    }
}
