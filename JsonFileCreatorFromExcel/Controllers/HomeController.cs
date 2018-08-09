using JsonFileCreatorFromExcel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace JsonFileCreatorFromExcel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CreateJsonArray(HttpPostedFileBase file)
        {
            string filePath = string.Empty;
            if (file != null)
            {
                string Filename = file.FileName;
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                file.SaveAs(filePath);

            }
            string filename = filePath;
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                          "Data Source=" + filename + ";" +
                                          "Extended Properties=Excel 8.0;";

            OleDbDataAdapter dataAdapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
            DataTable dtExcelSchema = new DataTable();
            List<phy> phyList = new List<phy>();
            using (OleDbConnection connExcel = new OleDbConnection(connectionString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        connExcel.Open();

                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dtExcelSchema.Rows.Count > 0)
                        {
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            string row1 = dtExcelSchema.Rows[1].ToString();
                            string row2 = dtExcelSchema.Rows[2].ToString();
                            connExcel.Close();
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dtExcelSchema);
                            foreach (DataRow row in dtExcelSchema.Rows)
                            {
                                phy obj = new phy();
                                if (row["CLASSID"].ToString() != "")
                                {
                                    obj.CLASSID = row["CLASSID"].ToString();
                                    obj.CATEGORY = row["CATEGORY"].ToString();
                                    obj.DRUG = row["DRUG"].ToString();
                                    obj.ARV = row["ARV"].ToString();
                                    obj.PK = row["PK"].ToString();
                                    phyList.Add(obj);
                                }

                            }
                            connExcel.Close();

                        }
                    }
                }

            }
            CreateTextFile(phyList);
            return RedirectToAction("Index");
        }

        public void CreateTextFile(IEnumerable<phy> phyArray)
        {
            using (raviookRemoteDBEntities db = new raviookRemoteDBEntities())
            {
                foreach (var x in phyArray)
                {
                    db.phies.Add(x);
                    db.SaveChanges();
                }
            }


        }

        public ActionResult JsonCreator()
        {
            raviookRemoteDBEntities db = new raviookRemoteDBEntities();
            var classId = db.phies.Select(m => m.CLASSID).Distinct();
            
            using (StreamWriter file = new StreamWriter(@"C:\Users\Ravi kumar\Desktop\Myjson.txt")) 
            {
                file.WriteLine("\"Others\" :[");
                foreach (var tempClassId in classId)
                {
                    var others = db.phies.Where(m => m.CLASSID == tempClassId).ToList();
                    file.WriteLine("[");
                    foreach (var temp in others)
                    {
                        file.WriteLine("{\"Category\":\"" + temp.CATEGORY + "\",\"Drug\":\"" + temp.DRUG + "\",\"ARV\":\"" + temp.ARV + "\",");
                        file.WriteLine("\"PK\":\"" + temp.PK + "\"},");
                    }
                    file.WriteLine("]ravi,");
                }
                file.WriteLine("]");
            }
            return RedirectToAction("Index");
        }
    }
}