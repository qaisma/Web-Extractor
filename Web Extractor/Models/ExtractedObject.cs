using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Extractor.Models
{
    public class ExtractedObject
    {
        public ExtractedObject()
        {
            RoomCategories = new List<string>();
            AlternativeHotels = new List<string>();
        }
        public string HotelName { get; set; }
        public string Address { get; set; }
        public string Classification { get; set; }
        public string ReviewPoints { get; set; }
        public int NumberOfReviews { get; set; }
        public string Description { get; set; }
        public List<string> RoomCategories { get; set; }
        public List<string> AlternativeHotels { get; set; }
    }
}