using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.Classes
{
    public class RoutingCriteria
    {
        public RoutingCriteria()
        {
            Origins = string.Empty;
            Destinations = string.Empty;
            OriginMetros = string.Empty;
            DestinationMetros = string.Empty;
            OriginCountries = string.Empty;
            DestinationCountries = string.Empty;
            OriginRegions = string.Empty;
            DestinationRegions = string.Empty;

            OriginsList = new List<string>();
            DestinationsList = new List<string>();
            OriginCountriesList = new List<string>();
            DestinationCountriesList = new List<string>();
            OriginMetrosList = new List<string>();
            DestinationMetrosList = new List<string>();
            OriginRegionsList = new List<string>();
            DestinationRegionsList = new List<string>();


        }
        
        private string _origins;
        public string Origins
        {
            get { return _origins; }
            set
            {
                _origins = value;
                if (!string.IsNullOrEmpty(value))
                {
                    OriginsList = value.Split(',').ToList();
                }
            }
        }
        private List<string>  OriginsList { get;  set; }
        public bool OriginsContains(string key)
        {
            return OriginsList.Contains(key);
        }

        private string _destinations;
        public string Destinations
        {
            get { return _destinations; }
            set
            {
                _destinations = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DestinationsList = value.Split(',').ToList();
                }
            }
        }
        private List<string> DestinationsList { get; set; }
        public bool DestinationsContains(string key)
        {
            return DestinationsList.Contains(key);
        }

        private string _originMetros;
        public string OriginMetros
        {
            get { return _originMetros; }
            set
            {
                _originMetros = value;
                if (!string.IsNullOrEmpty(value))
                {
                    OriginMetrosList = value.Split(',').ToList();
                }
            }
        }
        private List<string> OriginMetrosList { get; set; }

        private string _destinationMetros;
        public string DestinationMetros
        {
            get { return _destinationMetros; }
            set
            {
                _destinationMetros = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DestinationMetrosList = value.Split(',').ToList();
                }
            }
        }
        private List<string> DestinationMetrosList { get; set; }

        private string _originCountries;
        public string OriginCountries
        {
            get { return _originCountries; }
            set
            {
                _originCountries = value;
                if (!string.IsNullOrEmpty(value))
                {
                    OriginCountriesList = value.Split(',').ToList();
                }
            }
        }
        private List<string> OriginCountriesList { get; set; }
        public bool OriginCountriesContains(string key)
        {
            return OriginCountriesList.Contains(key);
        }

        private string _destinationCountries;
        public string DestinationCountries
        {
            get { return _destinationCountries; }
            set
            {
                _destinationCountries = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DestinationCountriesList = value.Split(',').ToList();
                }
            }
        }
        private List<string> DestinationCountriesList { get; set; }
        public bool DestinationCountriesContains(string key)
        {
            return DestinationCountriesList.Contains(key);
        }

        private string _originRegions;
        public string OriginRegions
        {
            get { return _originRegions; }
            set
            {
                _originRegions = value;
                if (!string.IsNullOrEmpty(value))
                {
                    OriginRegionsList = value.Split(',').ToList();
                }
            }
        }
        private List<string> OriginRegionsList { get; set; }
        public bool OriginRegionsContains(string key)
        {
            return OriginRegionsList.Contains(key);
        }

        private string _destinationRegions;
        public string DestinationRegions
        {
            get { return _destinationRegions; }
            set
            {
                _destinationRegions = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DestinationRegionsList = value.Split(',').ToList();
                }
            }
        }
        private List<string> DestinationRegionsList { get; set; }
        public bool DestinationRegionsContains(string key)
        {
            return DestinationRegionsList.Contains(key);
        }

        public bool NoRoutingCriteria
        {
            get
            {
                return string.IsNullOrEmpty(Origins) &&
                    string.IsNullOrEmpty(Destinations) &&
                    string.IsNullOrEmpty(OriginMetros) &&
                    string.IsNullOrEmpty(DestinationMetros) &&
                    string.IsNullOrEmpty(OriginCountries) &&
                    string.IsNullOrEmpty(DestinationCountries) &&
                    string.IsNullOrEmpty(OriginRegions) &&
                    string.IsNullOrEmpty(DestinationRegions);
            }
        }

        public bool HasCountry
        {
            get
            {
                return !string.IsNullOrEmpty(OriginCountries) || !string.IsNullOrEmpty(DestinationCountries);
            }
        }

        public bool NotInOrigin { get; set; }
        public bool NotInDestination { get; set; }
        public bool NotInOriginMetros { get; set; }
        public bool NotInDestinationMetros { get; set; }
        public bool NotInOriginCountries { get; set; }
        public bool NotInDestinationCountries { get; set; }
        public bool NotInOriginRegions { get; set; }
        public bool NotInDestinationRegions { get; set; }
    }
}
