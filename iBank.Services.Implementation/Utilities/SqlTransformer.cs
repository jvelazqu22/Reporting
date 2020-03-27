using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iBank.Services.Implementation.Utilities
{
    public class SqlTransformer
    {
        private static readonly Dictionary<string, string> _legsToMktSegsColumnNames = new Dictionary<string, string>
        {
            //legs table / mktsegs table
            { "rdepdate", "sdepdate"},
            { "deptime",  "sdeptime" },
            { "rarrdate", "sarrdate" },
            { "arrtime", "sarrtime" },
            { "(rdepdate", "(sdepdate"},
            { "(deptime",  "(sdeptime" },
            { "(rarrdate", "(sarrdate" },
            { "(arrtime", "(sarrtime" }

        };

        public string TranslateLegsAndMktSegsColumnNames(string clause, bool translateToMktSegsNames)
        {
            if (string.IsNullOrEmpty(clause)) return "";

            clause = clause.ToLower();
            var newClause = new StringBuilder();
            
            foreach (var s in clause.Split(' '))
            {
                var newValue = "";

                if (translateToMktSegsNames)
                {
                    newClause.Append(_legsToMktSegsColumnNames.TryGetValue(s, out newValue) ? newValue : s);
                }
                else
                {
                    //reverses the dictionary so that we can translate from segs to legs columns
                    var mktSegsToLegsColumnNames = _legsToMktSegsColumnNames.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
                    newClause.Append(mktSegsToLegsColumnNames.TryGetValue(s, out newValue) ? newValue : s);
                }
                
                newClause.Append(" ");
            }

            return newClause.ToString().Trim();
        }
    }
}
