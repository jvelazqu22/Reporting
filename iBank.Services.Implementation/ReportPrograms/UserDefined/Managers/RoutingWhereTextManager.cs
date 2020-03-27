using iBank.Server.Utilities;
using iBank.Services.Implementation.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class RoutingWhereTextManager
    {
        public string GetRoutingText(string whereText, RoutingCriteria routingCriteria, bool isExclHomeOn)
        {
            string text = "";

            //Add the details to the where text
            if (!string.IsNullOrEmpty(routingCriteria.Origins))
            {
                text += " Airport: " + routingCriteria.Origins;
            }

            if (!string.IsNullOrEmpty(routingCriteria.Destinations))
            {
                text += " Airport: " + routingCriteria.Destinations;
            }

            if (!string.IsNullOrEmpty(routingCriteria.OriginCountries))
            {
                text += " Country: " + routingCriteria.OriginCountries;
            }

            if (!string.IsNullOrEmpty(routingCriteria.DestinationCountries))
            {
                text += " Country: " + routingCriteria.DestinationCountries;
            }

            if (!string.IsNullOrEmpty(routingCriteria.OriginRegions))
            {
                text += " Region: " + routingCriteria.OriginRegions;
            }

            if (!string.IsNullOrEmpty(routingCriteria.DestinationRegions))
            {
                text += " Region: " + routingCriteria.DestinationRegions;
            }

            if (!string.IsNullOrEmpty(routingCriteria.OriginMetros))
            {
                text += " Metro: " + routingCriteria.OriginMetros;
            }

            if (!string.IsNullOrEmpty(routingCriteria.DestinationMetros))
            {
                text += " Metro: " + routingCriteria.DestinationMetros;
            }
            
            if (whereText.Right(10).Equals("Location -"))
            {
                text += " ANYWHERE";
            }
            else
            {
                if (isExclHomeOn)
                {
                    text += "; Exclude travelers arriving home";
                }
            }

            return !string.IsNullOrEmpty(text)
                ? !string.IsNullOrEmpty(whereText)
                    ? whereText + "; Traveler Location - " + text
                    : "Traveler Location - " + text
                : whereText + "; Traveler Location - ANYWHERE";
        }
    }
}
