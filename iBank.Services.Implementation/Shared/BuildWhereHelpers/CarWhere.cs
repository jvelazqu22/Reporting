using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class CarWhere : AbstractWhere
    {
        public void GetCarWhere(ReportGlobals globals, BuildWhere where, string notInText)
        {
            where.WhereClauseCar = AddListWhere(globals, where.WhereClauseCar, WhereCriteria.CARCOMP, WhereCriteria.INCARCOMPS, WhereCriteria.NOTINCARCOMP, "CARCOMP", "company", "Car Company", notInText);
            where.WhereClauseCar = AddListWhere(globals, where.WhereClauseCar, WhereCriteria.CARTYPE, WhereCriteria.INCARTYPES, WhereCriteria.NOTINCARTYPE, "CARTYPES", "cartype", "Car Type", notInText);

            where.WhereClauseCar = AddSimpleWhere(globals, where, where.WhereClauseCar, WhereCriteria.AUTOCITY, "autocity", "car city");
            where.WhereClauseCar = AddSimpleWhere(globals, where, where.WhereClauseCar, WhereCriteria.AUTOSTAT, "autostat", "car state");
            where.WhereClauseCar = AddSimpleWhere(globals, where, where.WhereClauseCar, WhereCriteria.RATETYPE, "ratetype", "rate type");
        }
    }
}
