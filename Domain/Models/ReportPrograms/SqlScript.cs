namespace Domain.Models
{
    public class SqlScript
    {
        public string FieldList { get; set; } = string.Empty;
        public string FromClause { get; set; } = string.Empty;
        public string WhereClause { get; set; } = string.Empty;
        public string KeyWhereClause { get; set; } = string.Empty;
        public string GroupBy { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
    }
}
