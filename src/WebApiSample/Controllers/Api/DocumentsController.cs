using System;
using System.Collections.Generic;
using System.Web.Http;

namespace WebApiSample.Controllers.Api
{
    [RoutePrefix("Api/Documents")]
    public class DocumentsController : ApiController
    {
        [HttpGet]
        [Route("Document")]
        public IList<Document> Documents()
        {
            return new List<Document>() { new Document() };
        }

        [HttpGet]
        [Route("Document")]
        public Document Document(int id)
        {
            return new Document();
        }

        [HttpPost]
        [Route("Document")]
        public void CreateDocument([FromBody]Document model)
        {
        }

        [HttpPut]
        [Route("Document")]
        public void EditDocument([FromBody]Document model)
        {
        }

        [HttpDelete]
        [Route("Document")]
        public void DeleteDocument(int id)
        {
        }
    }

    public class Document
    {
        public int Id { get; set; }

        public DateTime EmissionDate { get; set; }

        public DateTimeOffset CreationDate { get; set; }
    }
}
