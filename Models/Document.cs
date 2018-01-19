using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPservices.Models
{

    public class words {
        public int word { get; set; }
    } 

    public class Document
    {
        public string secRegNumber { get; set; }
        public string code { get; set; }
        public int orgId { get; set; }
        public string type { get; set; }
        public string dateDistributed { get; set; }
        public string nameEng { get; set; }
        public string nameSpa { get; set; }
        public Boolean onlyEDoc { get; set; }
        public Boolean restricted  { get; set; }

    }
}