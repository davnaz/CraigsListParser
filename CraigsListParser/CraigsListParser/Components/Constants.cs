using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraigsListParser.Components
{
    public class Constants
    {
        public class WebAttrsNames
        {
            public const string href = "href";
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
        }
    }
}
