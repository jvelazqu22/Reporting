using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class ChangesWhere : AbstractWhere
    {
        public bool AddBuildWhereChanges(ReportGlobals globals, BuildWhere where)
        {
            var item = globals.GetParmValue(WhereCriteria.CHANGECODE);
            var list = globals.GetParmValue(WhereCriteria.INCHANGECODE);
            var notIn = globals.IsParmValueOn(WhereCriteria.NOTINCHANGECODE);

            if (string.IsNullOrEmpty(item) && string.IsNullOrEmpty(list)) return true;

            if (string.IsNullOrEmpty(list)) list = item;

            var pickList = new PickListParms(globals);
            pickList.ProcessList(list, string.Empty, "CHNGCODES");

            if (!AllPicklistItemsAreNumeric(pickList.PickList)) return false;

            where.WhereClauseChanges = AddListWhere(globals, where.WhereClauseChanges, WhereCriteria.CHANGECODE, WhereCriteria.INCHANGECODE,
                WhereCriteria.NOTINCHANGECODE, "CHNGCODES", "changecode", "Change Code", where.NotInText);

            where.IncludeCancelled = true;

            if (pickList.PickList.Any(s => s.Contains("101")) || (item != null && item.Contains("101") && string.IsNullOrEmpty(list)))
            {
                where.IncludeCancelled = !notIn;
            }
            else
            {
                where.IncludeCancelled = notIn;
            }
            
            return true;
        }

        private bool AllPicklistItemsAreNumeric(List<string> picklist)
        {
            foreach (var x in picklist)
            {
                int y;
                if (!int.TryParse(x, out y)) return false;
            }

            return true;
        }
    }
}
