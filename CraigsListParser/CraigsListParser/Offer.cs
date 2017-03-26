using System;
using System.Collections.Generic;

namespace CraigsListParser
{
    internal class Offer
    {
        private long OfferID;
        private string OfferName;
        private double OfferPrice;
        private string OfferPlaceName;
        private string OfferPlaceMapsLink;
        private string OfferDescription;
        private string OfferBedRooms;
        private string OfferBathRooms;
        private double OfferSquare;
        private string OfferAvailability;
        private string OfferImages;
        private string OfferAdditional;
        private System.DateTime OfferPosted;
        private System.DateTime OfferUpdated;

        public Offer(long Id, string Name)
        {
            OfferID = Id;
            OfferName = Name;
            OfferPrice = 0;
            OfferPlaceName = null;
            OfferPlaceMapsLink = null;
            OfferDescription = null;
            OfferBedRooms = null;
            OfferBathRooms = null;
            OfferSquare = 0;
            OfferAvailability = null;
            OfferImages = null;
            OfferAdditional = null;
            OfferPosted = DateTime.MinValue;
            OfferUpdated = DateTime.MinValue;
        }

        public Offer(Offer a)
        {
            OfferID = a.OfferID;
            OfferName = a.OfferName;
            OfferPrice = a.OfferPrice;
            OfferPlaceName = a.OfferPlaceName;
            OfferPlaceMapsLink = a.OfferPlaceName;
            OfferDescription = a.OfferDescription;
            OfferBedRooms = a.OfferBedRooms;
            OfferBathRooms = a.OfferBathRooms;
            OfferSquare = a.OfferSquare;
            OfferAvailability = a.OfferAvailability;
            OfferImages = a.OfferImages;
            OfferAdditional = a.OfferAdditional;
            OfferPosted = a.OfferPosted;
            OfferUpdated = a.OfferUpdated;
        }

        public Offer()
        {
            OfferID = 0;
            OfferName = "";
            OfferPrice = 0;
            OfferPlaceName = null;
            OfferPlaceMapsLink = null;
            OfferDescription = null;
            OfferBedRooms = null;
            OfferBathRooms = null;
            OfferSquare = 0;
            OfferAvailability = null;
            OfferImages = null;
            OfferAdditional = null;
            OfferPosted = DateTime.MinValue;
            OfferUpdated = DateTime.MinValue;
        }

        public long ID
        {
            get
            {
                return OfferID;
            }

            set
            {
                OfferID = value;
            }
        }

        public string Name
        {
            get
            {
                return OfferName;
            }

            set
            {
                OfferName = value;
            }
        }

        public DateTime Posted
        {
            get
            {
                return OfferPosted;
            }

            set
            {
                OfferPosted = value;
            }
        }

        public DateTime Updated
        {
            get
            {
                return OfferUpdated;
            }

            set
            {
                OfferUpdated = value;
            }
        }

        public double Price
        {
            get
            {
                return OfferPrice;
            }

            set
            {
                OfferPrice = value;
            }
        }

        public string PlaceName
        {
            get
            {
                return OfferPlaceName;
            }

            set
            {
                OfferPlaceName = value;
            }
        }

        public string PlaceMapsLink
        {
            get
            {
                return OfferPlaceMapsLink;
            }

            set
            {
                OfferPlaceMapsLink = value;
            }
        }

        public string Description
        {
            get
            {
                return OfferDescription;
            }

            set
            {
                OfferDescription = value;
            }
        }

        public string BedRooms
        {
            get
            {
                return OfferBedRooms;
            }

            set
            {
                OfferBedRooms = value;
            }
        }

        public string BathRooms
        {
            get
            {
                return OfferBathRooms;
            }

            set
            {
                OfferBathRooms = value;
            }
        }

        public double Square
        {
            get
            {
                return OfferSquare;
            }

            set
            {
                OfferSquare = value;
            }
        }

        public string Availability
        {
            get
            {
                return OfferAvailability;
            }

            set
            {
                OfferAvailability = value;
            }
        }

        public string Images
        {
            get
            {
                return OfferImages;
            }

            set
            {
                OfferImages = value;
            }
        }


        public bool SetImages(List<string> imagesList)
        {
            try
            {
                string imageStringContainer = "";
                for (int i = 0; i < imagesList.Count; i++)
                {
                    imageStringContainer += imagesList[i] + ";"; // формирование строки с ссылками, которые разделены точкой с запятой
                }
                imageStringContainer.TrimEnd(';'); //убираем последнюю точку с запятой
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
           
        }


    }
}