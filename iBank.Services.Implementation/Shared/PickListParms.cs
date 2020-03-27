using CODE.Framework.Core.Utilities.Extensions;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Logging;

using ILogger = com.ciswired.libraries.CISLogger.ILogger;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.Shared
{
    // ToDo: cPickNameAcctTitle: how is it used?
    // ToDo: IsRouteCriteria: how is it used?
    // ToDo: gPListBreakout. Ref: lineno 946, ibBldWhere.prg
    /* gPListBreakout = ;
     *      Upper(Alltrim(crsIbUserExtras.fieldfunction)) = "PICKLIST_BREAKOUT" And ;
     *      Upper(Alltrim(crsIbUserExtras.fieldData)) = "YES" */

    /// <summary>
    /// PickListParms takes a comma delimited list of times, tidies up each item, and expands embedded lists, if any.
    /// Embedded lists are of the form "U-nnn" or "[U-nnn]" where "nnn" is a RecordNum in the client database table
    /// PickList. 
    /// </summary>
    public class PickListParms
    {
        private readonly ErrorLogger _errorLogger = new ErrorLogger();
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CommaDelimitedStringCollection Items;
        public CommaDelimitedStringCollection Names;

        public ReportGlobals Globals { get; set; }
        private static readonly string[] RouteCriteria = { "COUNTRY", "REGION", "AIRLINES", "AIRPORTS" };
        private string _pickType;
        
       public PickListParms(ReportGlobals globals)
       {
            Items = new CommaDelimitedStringCollection();
            Names = new CommaDelimitedStringCollection();
            Globals = globals;
       }

        public bool IsListBreakOut { get; set; }

        public bool IsRouteCriteria
        {
            get { return RouteCriteria.Contains(_pickType); }
        }

        public List<string> PickList
        {
            get
            {
                if (Items == null) return new List<string>();
                string itemString = Items.ToString();
                return (itemString == null) ? new List<string>() : itemString.Split(',').ToList();
            }
        }

        public string PickListString { get { return string.Join(",", PickList); } }

        public string PickName {
            get { return Names.ToString();}
        }
        public List<string> PickNameList
        {
            get
            {
                if (Names == null) return new List<string>();
                string nameString = Names.ToString();
                return (nameString == null) ? new List<string>() : nameString.Split(',').ToList();
            }
        }

        public string PickNameAcctTitle { get; set; }

        // I return the value replacement from the get instead of setting the field in the set because
        // it feels wrong to lose the original value that was sent. Once it's clearer how it's being
        // used whatever is most efficient or effective.
        public string PickType
        {
            get
            {
                return (String.Compare(_pickType, "VALCARR",
                    StringComparison.OrdinalIgnoreCase) == 0) ? "AIRLINES" : _pickType;
            }
            set { _pickType = value; }
        }

        public void ProcessList(string pickList, string pickName, string pickType)
        {
            Items.Clear();
            PickType = pickType;
            ParseList(pickList, pickName);
        }

        protected void ExpandList(string pickItem)
        {
            string s = pickItem.Replace("U-", String.Empty).Replace(@"[", string.Empty).Replace(@"]", string.Empty);
            int i = s.TryIntParse(-1);
            if (i < 0) return;
            
            // Question for CIS: The column picklists.recordnum is an identity column, so, why
            // do we select for PickType, too?
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var result = new GetPicklistByRecordNumberAndListTypeQuery(new iBankClientQueryable(server, db), i, PickType).ExecuteQuery();
            // Something is wrong...the parent level pick list refers to a list that's expired?
            PickListRow firstOrDefault = result.FirstOrDefault();
            if (firstOrDefault == null) return;
            ParseList(firstOrDefault.ListData, firstOrDefault.ListName);
        }

        protected void ParseList(string pickList, string pickName)
        {
            if (pickName != string.Empty) Names.Add(pickName.TrimCommaDelimitedItem());
            SplitList(pickList, pickName);
        }

        protected void SplitList(string pickList, string pickName)
        {
            string[] pl =
                pickList.ToUpperInvariant().TrimCommaDelimitedItem().RemoveUnintentionalSpaces().Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries);

            if (pl.Count() == 1 && PickListContainsList(pl[0]) == false) // We're done.
            {
                Items.Add(pl[0].TrimCommaDelimitedItem());
                return;
            }

            foreach (string s in pl)
            {
                if (PickListContainsList(s))
                {
                    ExpandList(s);
                    continue;
                }
                Items.Add(s.TrimCommaDelimitedItem());
            }
        }

        protected static bool PickListContainsList(string val)
        {
            return val.StartsWith("U-") || val.StartsWith("(U-");
            // the below line was commented to fix US: https://ciswired.atlassian.net/browse/ST2-313 the above code was added 
            // per the comments inside the ibBldWhere.prg file and after having some discussion with Sofia about how pickList work
            //return val.Contains("U-");
        }
    }
}