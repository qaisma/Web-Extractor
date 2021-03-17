using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Extractor.Models
{
    public class HotelImport
    {
        public Hotel hotel { get; set; }
        public List<HotelRates> hotelRates { get; set; }
    }
    public class Hotel
    {
        public int hotelID { get; set; }
        public int classification { get; set; }
        public string name { get; set; }
        public float reviewscore { get; set; }
    }
    public class HotelRates
    {
        public int adults { get; set; }
        public int los { get; set; }
        public Price price { get; set; }
        public string rateDescription { get; set; }
        public string rateID { get; set; }
        public string rateName { get; set; }
        public List<RateTag> rateTags { get; set; }
        public DateTime targetDay { get; set; }

    }
    public class Price
    {
        public string currency { get; set; }
        public float numericFloat { get; set; }
        public int numericInteger { get; set; }
    }
    public class RateTag
    {
        public string name { get; set; }
        public bool shape { get; set; }
    }
}