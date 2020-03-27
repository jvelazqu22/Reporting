using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class RawDataHelper
    {
        private BuildWhere _buildWhere;
        private IClientQueryable _clientQueryDb;
        private ReportGlobals _globals;
        private bool _isPreview;

        public RawDataHelper(BuildWhere buildWhere, IClientQueryable clientQueryDb, ReportGlobals globals, bool isPreview)
        {
            _globals = globals;
            _clientQueryDb = clientQueryDb;
            _buildWhere = buildWhere;
            _isPreview = isPreview;
        }


        public List<RawData> GetRawData(string whereClause, bool isPreview, string UdidNumber)
        {
            var orderClause = "order by T1.valcarr, T1.SourceAbbr, valcarMode";
            string fromClause;
            string fieldList;

            int udid;
            var goodUdid = int.TryParse(UdidNumber, out udid);
            if (goodUdid && udid != 0)
            {
                fromClause = isPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
                whereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$','XD') and valcarr is not null and " + whereClause;
                fieldList = "valcarr, SourceAbbr, 'A' as valcarMode, airchg ";
            }
            else
            {
                fromClause = isPreview ? "ibtrips T1" : "hibtrips T1";
                whereClause = "valcarr not in ('ZZ','$$','XD') and valcarr is not null and " + whereClause;
                fieldList = "valcarr, SourceAbbr, case when valcarMode != 'R' then 'A' else valcarMode end as valcarMode, airchg ";
            }
            fieldList += ", depdate, arrdate, invdate, bookdate, ";
            fieldList += (isPreview) ? "T1.reckey, convert(int, 1) as plusmin " : " T1.reckey, convert(int, plusmin) as plusmin";

            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClause, orderClause, _globals);
            return ClientDataRetrieval.GetUdidFilteredOpenQueryData<RawData>(fullSql, _globals, _buildWhere.Parameters, _isPreview).ToList();
        }


        public List<RawData> ApplyHomeCountryFilterAndMode(List<RawData> list, string inHomeCtry, string notCtry, int mode)
        {
            var rawDataList = ApplyHomeCountryFilter(list, inHomeCtry, notCtry);
            return ModeFilter(rawDataList, mode);
        }

        public List<RawData> ApplyHomeCountryFilter(List<RawData> list, string inHomeCtry, string notCtry)
        {
            if (inHomeCtry == "" && inHomeCtry == "") return list;
            var rowsToRemove = new List<RawData>();

            string[] arrHomeCtry = inHomeCtry.Split(',');

            for (int i = 0; i < arrHomeCtry.Length; i++)
            {
                string ctry = arrHomeCtry[i].Trim();
                foreach (var row in list)
                {
                    string cctry = LookupFunctions.LookupHomeCountryCode(row.SourceAbbr, _globals, new MasterDataStore()).Trim();
                    //need to remove the countries
                    if (cctry == notCtry || cctry != ctry) rowsToRemove.Add(row);
                }
            }

            foreach (var row in rowsToRemove)
            {
                list.Remove(row);
            }

            return list;
        }

        public List<RawData> ModeFilter(List<RawData> list, int mode)
        {
            if (mode == 0) return list;
            var modeCode = (mode == 1) ? "A" : "R";
            var rowsToRemove = new List<RawData>();

            foreach (var row in list)
            {
                string curMode = row.ValCarMode;
                if (curMode != modeCode) rowsToRemove.Add(row);
            }

            foreach (var row in rowsToRemove)
            {
                list.Remove(row);
            }

            return list;
        }
    }
}
