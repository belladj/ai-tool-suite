using ai_tool_suite.Models;
using OpenAI_API;
using OpenAI_API.Completions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ai_tool_suite.Controllers
{
    public class PdfController : Controller
    {
        public string apiKey = "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";
        // GET: Pdf
        public ActionResult Index()
        {
            return View(GetFiles());
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            string constr = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "INSERT INTO Pdfs VALUES (@Name, @ContentType, @Data)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Name", Path.GetFileName(postedFile.FileName));
                    cmd.Parameters.AddWithValue("@ContentType", postedFile.ContentType);
                    cmd.Parameters.AddWithValue("@Data", bytes);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return View(GetFiles());
        }

        [HttpPost]
        public JsonResult GetPdf(int id)
        {
            byte[] bytes;
            string fileName, contentType;
            string constr = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT Name, Data, ContentType FROM Pdfs WHERE ID=@ID";
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        sdr.Read();
                        bytes = (byte[])sdr["Data"];
                        contentType = sdr["ContentType"].ToString();
                        fileName = sdr["Name"].ToString();
                    }
                    con.Close();
                }
            }
            JsonResult jsonResult = Json(new { FileName = fileName, ContentType = contentType, Data = bytes });
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        private static List<Pdfs> GetFiles()
        {
            List<Pdfs> files = new List<Pdfs>();
            string constr = ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ID, Name FROM Pdfs"))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Pdfs
                            {
                                ID = Convert.ToInt32(sdr["ID"]),
                                Name = sdr["Name"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return files;
        }

        [HttpPost]
        public ActionResult Run(string searchText)
        {
            string answer = string.Empty;
            var openai = new OpenAIAPI(apiKey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = searchText;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 4000;
            var result = openai.Completions.CreateCompletionAsync(completion);
            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    answer = item.Text;
                }
            }
            ViewBag.Answer = answer;
            ViewBag.Text = searchText;
            return View();
        }
    }
}