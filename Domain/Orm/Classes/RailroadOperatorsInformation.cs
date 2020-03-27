namespace Domain.Orm.Classes
{
    public class RailroadOperatorsInformation
    {
        public RailroadOperatorsInformation()
        {
            OperatorCode = string.Empty;
            OperatorName = string.Empty;
        }
        public int OperatorNumber { get; set; }
        public string OperatorCode { get; set; }
        public string OperatorName { get; set; }
    }
}
