using HtmlAgilityPack;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Web_Extractor.Utilities;

namespace Web_Extractor.Controllers
{
    public class FilesController : Controller
    {
        #region task 1
        [HttpPost]
        public JsonResult Extract(HttpPostedFileBase theHtmlFile)
        {
            if (theHtmlFile != null)
            {
                BinaryReader b = new BinaryReader(theHtmlFile.InputStream);
                byte[] binData = b.ReadBytes(theHtmlFile.ContentLength);

                string fileContent = System.Text.Encoding.UTF8.GetString(binData);

                //string hotelNamePattern = "<span.*?id=\"hp_hotel_name\">\s*(.+?)\s*</span>";
                //string addressPattern = "<span.*?id=\"hp_address_subtitle\">\s*(.+?)\s*</span>";
                //string classificationPattern = "<i class=\"b-sprite stars (.*?) star_track\"";
                //string reviewPointsPattern = "<span.*?id=\"hp_hotel_name\">\s*(.+?)\s*</span>";
                //...
                //..
                //.

                ////Utilities.HtmlTools.StripTags
                //Regex regex = new Regex(hotelNamePattern);
                //Match match = regex.Match(fileContent);

                //var v1 = match.Groups[1].ToString();


                /////////////////// we can use regex to extract the content, but why to invent the wheel?
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(fileContent);

                var extractedObj = new Models.ExtractedObject();
                extractedObj.HotelName = htmlDoc.GetElementbyId("hp_hotel_name").InnerText.Trim();
                extractedObj.Address = htmlDoc.GetElementbyId("hp_address_subtitle").InnerText.Trim();

                //Classification
                var domObj = htmlDoc.DocumentNode.Descendants().FirstOrDefault(o => o.HasClass("star_track"));
                var val = domObj == null ?
                    string.Empty :
                    domObj.GetClasses().FirstOrDefault(c => c.Contains("ratings_stars_")).Split('_')[2].Trim();
                extractedObj.Classification = int.Parse(val);

                //ReviewPoints
                domObj = htmlDoc.DocumentNode.Descendants().FirstOrDefault(o => o.HasClass("rating")
                                                                    && o.Descendants().Any(d => d.HasClass("average"))
                                                                  );
                val = domObj == null ?
                    string.Empty :
                    domObj.Descendants().First(d => d.HasClass("average")).InnerText.Trim();
                extractedObj.ReviewPoints = val;


                //NumberOfReviews
                domObj = htmlDoc.DocumentNode.Descendants().FirstOrDefault(o => o.GetClasses().Any(c => c.Contains("number_of_reviews"))
                                                                    && o.Descendants().Any(d => d.HasClass("count"))
                                                                  );
                val = domObj == null ?
                    string.Empty :
                    domObj.Descendants().First(d => d.HasClass("count")).InnerText.Trim();
                extractedObj.NumberOfReviews = int.Parse(val);

                //Description
                foreach (var node in htmlDoc.GetElementbyId("summary").SelectNodes("p"))
                {
                    extractedObj.Description += node.InnerText.Trim() + Environment.NewLine;
                }

                //RoomCategories
                foreach (var node in htmlDoc.GetElementbyId("maxotel_rooms").Descendants().Where(d => d.HasClass("ftd")))
                {
                    extractedObj.RoomCategories.Add(node.InnerText.Trim());
                }

                //AlternativeHotels
                foreach (var node in htmlDoc.GetElementbyId("althotelsRow").Descendants().Where(d => d.HasClass("althotel_link")))
                {
                    extractedObj.AlternativeHotels.Add(node.InnerText.Trim());
                }
                return Json(extractedObj, JsonRequestBehavior.AllowGet);
            }

            //internal server error
            return Json(new { IsExtracted = false, StatusCode = 500, ErrorMessage = "No posted file found!" }, JsonRequestBehavior.AllowGet);
        } 
        #endregion

        #region task2
        [HttpPost]
        public JsonResult GenerateExcelReport(HttpPostedFileBase thePostedFile)
        {
            if (thePostedFile != null)
            {
                BinaryReader binaryReader = new BinaryReader(thePostedFile.InputStream);
                byte[] binData = binaryReader.ReadBytes(thePostedFile.ContentLength);

                string fileContent = System.Text.Encoding.UTF8.GetString(binData);
                var data = JsonConvert.DeserializeObject<Models.HotelImport>(fileContent);

                var fileName = "ConvertedFile_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls";

                // create an empty spreadsheet
                using (var excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Hotels");
                    int row = 1;
                    worksheet.SetValue(row, 1, "ARRIVAL_DATE");
                    worksheet.SetValue(row, 2, "DEPARTURE_DATE");
                    worksheet.SetValue(row, 3, "PRICE");
                    worksheet.SetValue(row, 4, "CURRENCY");
                    worksheet.SetValue(row, 5, "RATENAME");
                    worksheet.SetValue(row, 6, "ADULTS");
                    worksheet.SetValue(row, 7, "BREAKFAST_INCLUDED");

                    foreach (var hotelRate in data.hotelRates)
                    {
                        row++;
                        worksheet.SetValue(row, 1, hotelRate.targetDay.ToShortDateString());
                        worksheet.SetValue(row, 2, hotelRate.targetDay.AddDays(hotelRate.los).ToShortDateString());
                        worksheet.SetValue(row, 3, hotelRate.price.numericFloat);
                        worksheet.SetValue(row, 4, hotelRate.price.currency);
                        worksheet.SetValue(row, 5, hotelRate.rateName);
                        worksheet.SetValue(row, 6, hotelRate.adults);
                        var isBreakfastIncluded = hotelRate.rateTags.Any(t => t.name == "breakfast" && t.shape);
                        worksheet.SetValue(row, 7, isBreakfastIncluded ? 1 : 0);

                        if (row % 2 != 0)
                        {
                            worksheet.Row(row).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Row(row).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }
                    }
                    using (var ms = new MemoryStream())
                    {
                        excelPackage.SaveAs(ms);
                        //ms.Seek(0, SeekOrigin.Begin);
                        //fileData = ms.ToArray();
                        //save the file to server temp folder
                        string fullPath = Path.Combine(Server.MapPath("~/FilesTemp"), fileName);
                        FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        ms.WriteTo(file);
                        file.Close();
                    }
                }
                return Json(new { FileName = fileName }, JsonRequestBehavior.AllowGet);
            }

            //internal server error
            //500 sould be from enum
            return Json(new { IsExtracted = false, StatusCode = 500, ErrorMessage = "File was not processed!" }, JsonRequestBehavior.AllowGet);
        }

        [DeleteFileAttribute]
        public ActionResult DownloadExcelReport(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                //get the temp folder and file path in server
                string fullPath = Path.Combine(Server.MapPath("~/FilesTemp"), fileName);

                //return the file for download, this is an Excel 
                //so I set the file content type to "application/vnd.ms-excel"
                return File(fullPath, "application/vnd.ms-excel", fileName);
            }

            //internal server error
            //500 sould be from enum
            return Json(new { IsExtracted = false, StatusCode = 500, ErrorMessage = "File name was not provided!" }, JsonRequestBehavior.AllowGet);
        } 
        #endregion
    }
}