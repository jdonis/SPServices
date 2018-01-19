using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Idb.CommonServices.Util.Office;
using System.Web;
using SPservices.Models; 
//using System.Web.Http.Cors;


namespace SPservices.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class countController : ApiController
    {

        private int getTotalWordCount(string path_ = "%"){
            int count = 0;
           
            //var info = OfficeHelper.GetWordDocumentStats(@"D:\Temp\!Downloads\5cd3_b_04_public_statement.docx");
            var info = OfficeHelper.GetWordDocumentStats(@path_);
            Console.WriteLine("Characters: " + info.Characters);
            Console.WriteLine("Words: " + info.Words);
            Console.WriteLine("Pages: " + info.Pages);
            Console.WriteLine("Paragraphs: " + info.Paragraphs);
            Console.WriteLine("Lines: " + info.Lines);
            count = info.Words; 
            return count;
        }

        // GET api/count
        public IEnumerable<string> Get()
        {
            //int count = getTotalWordCount(); 
            //return new string[] { count.ToString(), "value2" };
            var origin = HttpContext.Current.Request.Headers["Origin"]; 
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST");

            return new string[] { "dos", "value2" };
            
            //string callback = HttpContext.Current.Request.QueryString["callback"];
            //return callback + "(" + "{2,value2}"  + ")";
        }

        // GET api/count/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/count
        public HttpResponseMessage Post()
        {
            var httpRequest = HttpContext.Current.Request;
          
            //HttpRequestMessage request = this.Request;
            //if (request.Content.IsMimeMultipartContent()) {
            //    Console.WriteLine("yes"); 
            //}

            if (httpRequest.Files.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            int totalWords = 0;    
            foreach (string file in httpRequest.Files)
            {
                var postedFile = httpRequest.Files[file];
                var filePath = HttpContext.Current.Server.MapPath("~/TempStorage/" + postedFile.FileName);
                postedFile.SaveAs(filePath);
                totalWords = totalWords + getTotalWordCount(filePath);
                // NOTE: To store in memory use postedFile.InputStream
            }
             
           
              var origin = HttpContext.Current.Request.Headers["Origin"]; 
              HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin);
              HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
            //string callback = HttpContext.Current.Request.QueryString["callback"];
              var word = new words();
               word.word = totalWords; 
             return Request.CreateResponse(HttpStatusCode.OK, word);
            //return callback + "([{ words:" + totalWords.ToString() + "}]);"; 
        }

        // PUT api/count/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/count/5
        public void Delete(int id)
        {
        }
    }
}
