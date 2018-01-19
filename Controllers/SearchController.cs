using Newtonsoft.Json;
using SPservices.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SPservices.Controllers
{
    public class test {
        public int id { get; set; }
        public string country { get; set; }
        public string city { get; set; }
    }
    public class documentTypes {
        public int clsId { get; set; }
        public string clsCode { get; set; }
        public int orgId { get; set; }
        public string titleEng { get; set; }
        public string titleSpa { get; set; }
    }


    public class SearchController : ApiController
    {
        // GET api/search
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        
        

        // GET api/search/5
        public List<Document> Get(int inst, int lang, string serie = "%", string query = "%")
        {

            var origin = HttpContext.Current.Request.Headers["Origin"];
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST");


            var lista = new List<Document>(); 
 
            var response = new HttpResponseMessage();
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            string appID = System.Web.Hosting.HostingEnvironment.ApplicationID;

            //var fromDate = "11/09/2017";
            //var toDate =  "18/09/2017"; 
            var fromDate = "20170101";
            var toDate = "20171231"; 

            var cs = "Data Source=DPSSRV02;Initial Catalog=Agenda;User ID=webuser;Password=colbra2K";
            //var sp = ConfigurationManager.AppSettings["SP_PIC_FORM_GET_TOPIC_TIP_BY_LANGUAGE"];
            var _serie = ""; 
            if (serie != "all"){
                _serie = "and (C.CLS_CODE like '" + serie + "')";  
            }
            var _inst = "";
            if (inst > 0)
            {
                _inst = "and (C.ORG_ROWID = "+ inst + ")";
            } 

            var sql_ = "SELECT D.DOV_VERSION_ID, " +
            "C.CLS_CODE," +
            "C.ORG_ROWID," +
            "C.CLS_TYPE," +
            "D.DOV_ACTUAL_DIST_DATE, " +
            "D.DOV_VERSION_NAME_ENG, " +
            "D.DOV_VERSION_NAME_SPA, " +
            "D.DOV_E_DIST_IND, " +
            "D.DOV_ACCESS_IND " +
            "FROM  DOC_VERSION D, " +
            "CLASS C " +
            "WHERE	(D.DOV_VERSION_REG_DATE IS NOT NULL) " +
            "AND (SUBSTRING(D.DOV_VERSION_ID, 1, 3) <> 'MH-') " +
            "AND (D.DOV_ACTUAL_DIST_DATE >= '"+fromDate+"') " +
            "AND (D.DOV_ACTUAL_DIST_DATE <= '"+toDate +"') " +
            "AND C.ORG_ROWID = case SUBSTRING(SUBSTRING ( DOV_VERSION_ID , 0 , CHARINDEX('-', DOV_VERSION_ID)), 0, CHARINDEX('/', SUBSTRING ( DOV_VERSION_ID , 0 , CHARINDEX('-', DOV_VERSION_ID)) ) ) when '' then 1 when 'CII' then 2 else 3 end " +
            "AND C.CLS_CODE = SUBSTRING ( DOV_VERSION_ID , CHARINDEX('/', DOV_VERSION_ID)+1 , CHARINDEX('-', DOV_VERSION_ID) - CHARINDEX('/', DOV_VERSION_ID)-1) " +
            "and (d.DOV_VERSION_NAME_" + (lang==1 ? "SPA":"ENG") + " like '%" + query + "%') " +
             _serie + 
             _inst +
            "ORDER BY D.DOV_ACTUAL_DIST_DATE DESC, D.DOV_ROWID DESC ";  
   


            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(cs);
               
                conn.Open();


                var cmd = new SqlCommand(sql_, conn);

                //cmd.CommandType = CommandType.StoredProcedure;
                
                //cmd.Parameters.AddWithValue("@ipAddress", userIp);
                //cmd.Parameters.AddWithValue("@countryName", " ");
                //cmd.Parameters.AddWithValue("@docName", obj.documentName);

                //cmd.Parameters.Add("@Lang", SqlDbType.VarChar, 2).Value = language;
                //cmd.Parameters.Add("@Value", SqlDbType.VarChar, 255).Value = option;

                var dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    var test_ = new Document();
                    test_.secRegNumber = (dr["DOV_VERSION_ID"].ToString().Trim());
                    test_.code = dr["CLS_CODE"].ToString();
                    test_.orgId = Int32.Parse(dr["ORG_ROWID"].ToString());
                    test_.type = dr["CLS_TYPE"].ToString();
                    var _date = Convert.ToDateTime(dr["DOV_ACTUAL_DIST_DATE"].ToString()); 
                    test_.dateDistributed = _date.Month.ToString() + "/" + _date.Day.ToString() + "/"+ _date.Year.ToString()  ; 
                    
                    test_.nameEng = dr["DOV_VERSION_NAME_ENG"].ToString();
                    test_.nameSpa = dr["DOV_VERSION_NAME_SPA"].ToString();
                    test_.onlyEDoc = (dr["DOV_E_DIST_IND"].ToString() == "N" ? false : true);
                    test_.restricted = (dr["DOV_ACCESS_IND"].ToString() == "R" ? true:false) ;

                    lista.Add(test_); 
                }            

                //if (dr.hasrows)
                //{
                //    dr.read();
                //    var tojson = jsonconvert.serializexmlnode(nodo, newtonsoft.json.formatting.none, true);
                //    response.content = new stringcontent(tojson, system.text.encoding.utf8, "application/json");

                //}

                return lista;
             }
             catch (SqlException e)
            {
                //Console.WriteLine(e.Message);
                throw;
            }
            //catch (Exception e)
            //{
            //    //Console.WriteLine(e.Message);
            //    throw;
            //}
            //finally
            //{
            //    if (conn != null)
            //        ((IDisposable)conn).Dispose();
            //}

           

            //return "value " + id;
        }
      
        // private string test () {

        // string filters = "" ; 
        // string class_title  = "a.CLS_TITLE_ENG";
        // string org_name  = "b.ORG_NAME_ENG";
        // string org_condition = "";

        // If ListOfClassTypes IsNot Nothing Then
        //    For i As Integer = 0 To ListOfClassTypes.Length - 1
        //        If i = 0 Then
        //            filters += "and ( "
        //        Else
        //            filters += " or "
        //        End If
        //        filters += "a.CLS_TYPE ='" + ListOfClassTypes(i) + "' "
        //        If i = ListOfClassTypes.Length - 1 Then
        //            filters += ") "
        //        End If
        //    Next
        //End If

        //If OrgRowId IsNot Nothing Then
        //    org_condition = "          and b.ORG_ROWID = @org_rowid "
        //End If

        //If (Lang == Enums.Languages.Spanish) {  
        //    class_title = "a.CLS_TITLE_SPA"; 
        //    org_name = "b.ORG_NAME_SPA"; 
        //} 


        //var string QueryStr  = "SELECT	a.CLS_ROWID, " +
        //                        "          a.ORG_ROWID," +
        //                         "          a.CLS_CODE," +
        //                         "          $class_title as CLS_TITLE, " +
        //                         "          a.CLS_SEC_IND," +
        //                         "          b.ORG_CODE," +
        //                         "          $org_name as ORG_NAME," +
        //                         "          a.CLS_TYPE " +
        //                         "FROM	    CLASS a, " +
        //                         "          ORGAN b " +
        //                         "WHERE     a.ORG_ROWID = b.ORG_ROWID " +
        //                         "          and CLS_ROWID in (select distinct CLS_ROWID from DOC_ROOT) " +
        //                         "          and a.CLS_CODE <> 'MH' " +
        //                         org_condition;

        //QueryStr += filters + " ORDER BY a.CLS_CODE ";
        //QueryStr = QueryStr.Replace("$class_title", class_title);
        //QueryStr = QueryStr.Replace("$org_name", org_name);



        //    var cs = ConfigurationManager.ConnectionStrings["AgendaConnectionString"].ConnectionString; 
        
        //    var Conn = new SqlConnection(cs);
        //    var Cmd =  new SqlCommand(QueryStr, Conn);

        //    Cmd.Parameters.Add(new SqlParameter("@org_rowid", OrgRowId));

        //    var Adpt = new SqlDataAdapter(Cmd); 

        

           
        //  return ""; 
        //}


        // POST api/search
        public void Post([FromBody]string value)
        {
        }

        // PUT api/search/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/search/5
        public void Delete(int id)
        {
        }
    }
}
