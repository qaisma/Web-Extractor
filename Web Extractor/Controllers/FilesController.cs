using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Web_Extractor.Controllers
{
    public class FilesController : Controller
    {
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
                //string numberOfReviewsPattern = "<span.*?id=\"hp_hotel_name\">\s*(.+?)\s*</span>";
                //string descriptionPattern = "<span.*?id=\"hp_hotel_name\">\s*(.+?)\s*</span>";
                //string roomCategoriesPattern = "<span.*?id=\"hp_hotel_name\">\s*(.+?)\s*</span>"; 
                //string alternativesPattern = "<span.*?id=\"hp_hotel_name\">\s*(.+?)\s*</span>";

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
                extractedObj.Classification = val;

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
            return Json(new { IsExtracted = false, StatusCode = 500, ErrorMessage = "No posted file found!" });
        }
    }
}