using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class ClauseParser
    {
        private FieldPrefixer _fieldPrefixer = new FieldPrefixer();
        // we want to avoid calling the db hundrends of times inside one of the loops within this class

        public IEnumerable<string> GetTablesThatExistInClause(string clause, List<string> tablesToCheckFor)
        {
            // That is just matching on the table name. \b…\b is Regex for a whole word
            return tablesToCheckFor.Where(table => Regex.IsMatch(clause, $@"\b{table}\b", RegexOptions.IgnoreCase));
        }

        public string GetExistingReckeyPrefix(string clause)
        {
            //find matches where the clause string contains reckey
            var match = Regex.Match(clause, $@"\breckey\b", RegexOptions.IgnoreCase);
            if (!match.Success || match.Index <= 2) return "";

            var lengthOfPrefix = DetermineLengthOfPrefix(clause, match.Index);

            var startingIndex = match.Index - lengthOfPrefix;
            var prefix = clause.Substring(startingIndex, lengthOfPrefix - 1); //we subtract 1 to account for the period that separates the prefix from the table

            return prefix;
        }

        private int DetermineLengthOfPrefix(string clause, int startIndexOfTable)
        {
            var lengthOfPrefix = 0;
            for (var i = startIndexOfTable - 1; i >= 0; i--)
            {
                //walk the clause backwards until you get to a space or comma. That will signify that you have reached the start of the prefix
                if (clause[i] != ' ' && clause[i] != ',')
                {
                    lengthOfPrefix++;
                }
                else
                {
                    break;
                }
            }

            return lengthOfPrefix; 
        }

        public Dictionary<string, string> BuildTablePrefixPairsUseEntityFieldsLookup(string fromClause, bool isReservation)
        {
            var pairs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            //first we split them on the comma -> comes in this form 'ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock)'
            foreach (var table in fromClause.SplitAndRemoveEmptyStrings(','))
            {
                //then we want to split them on the space -> end up with { "ibtrips", "T1", "WITH", "(nolock)", ... }
                var splitClause = table.SplitAndRemoveEmptyStrings(' ').ToList();

                if (!splitClause.Any()) continue;

                var lookup = new EntityFieldsLookup();
                var prefix = lookup.GetTablePrefix(splitClause[0], isReservation);
                pairs.Add(splitClause[0], prefix);
            }

            return pairs;
        }

        public string PrefixWhereClause(string whereClause, bool isReservationReport, Dictionary<string, string> tablePrefixPairs)
        {
            return _fieldPrefixer.PerformWherePrefix(whereClause, isReservationReport, tablePrefixPairs, ' ', ' '.ToString());
        }

        public string PrefixSelectClause(string selectClause, bool isReservationReport, Dictionary<string, string> tablePrefixPairs)
        {
            return _fieldPrefixer.PerformSelectPrefix(selectClause, isReservationReport, tablePrefixPairs, ',', ", ");
        }
    }
}
