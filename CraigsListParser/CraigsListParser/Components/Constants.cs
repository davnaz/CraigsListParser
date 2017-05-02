using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraigsListParser.Components
{
    public class Constants
    {
        public class PriceValues
        {
            public const int undefinedValue = -1;
            public const int maxValue = 2500;
        }

        public class WebAttrsNames
        {
            public const string href = "href";
            public const string NotFound = "no";
        }
        public class OfferSelectorNames
        {
            public const string ID = "link[rel=canonical]"; // trim 'href' 
            public const string Name = "title"; //textContent
            public const string Price = "span.price"; //textContent
            public const string PlaceName = "div.mapaddress"; //textContent
            public const string PlaceMapsLink = "p.mapaddress a"; //href
            public const string Description = "#postingbody"; //textContent
            //public const string BedRooms = ""; 
            //public const string BathRooms = "";
            //public const string Square = "";
            //public const string Availability = "";
            public const string Image = "";
            //public const string Additional = "p.attrgroup"; // get collection, and if Contains more then 1 child, return infocollection[infocollection.Length - 1].TextContent.Replace("           ", "");
            public const string Posted = "#display-date>time";  //Attr title
            public const string Updated = ".postinginfo.reveal"; //  get collection, and if Contains "updated", parse value of attr "title"
            public const string City = "li.crumb.area > p > a"; //textContent
        }

        public class DbCellNames
        {
            public const string City = "@City";
            public const string PostID = "@PostID"; 
            public const string Name = "@Name"; 
            public const string Price = "@Price"; 
            public const string PlaceName = "@Place";
            public const string PlaceMapsLink = "@Placemaplink";
            public const string Description = "@Description"; 
            public const string BedRooms = "@Bedrooms"; 
            public const string BathRooms = "@Bathrooms";
            public const string Square = "@Square";
            public const string Availability = "@Availability";
            public const string Images = "@Images";
            public const string Additional = "@Additional"; 
            public const string Posted = "@Posted"; 
            public const string Updated = "@Updated"; 
        }
    }
}
