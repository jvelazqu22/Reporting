namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class RoutingCriteria
    {
        public string Origin { get; set; }
        public string Destination { get; set; }

        public bool IsOnlyOneWayCriteriaSpecified { get { return !string.IsNullOrEmpty(Origin) && string.IsNullOrEmpty(Destination)
                                                                 || string.IsNullOrEmpty(Origin) && !string.IsNullOrEmpty(Destination);
        } }

        public bool AreBothWaysCriteriaSpecified { get
        {
            return !string.IsNullOrEmpty(Origin) && !string.IsNullOrEmpty(Destination);
        } }

        public RoutingCriteria(string origin, string destination)
        {
            Origin = origin;
            Destination = destination;
        }
    }
}
