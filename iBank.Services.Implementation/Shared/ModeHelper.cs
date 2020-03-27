using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.Shared { 
    public class ModeHelper
    {
        private readonly ReportGlobals _globals;
        private readonly string _whereClause;
        private readonly bool _isPreview;
        private readonly object[] _parameters;
        private Mode _modeType { get; set; }
        private string _modeWhere { get; set; }
        private string _modeFrom { get; set; }
        public string ModeText { get; set; }

        public ModeHelper(ReportGlobals globals, object[] parameters, string whereClause, bool isPreview)
        {
            _globals = globals;
            _whereClause = whereClause;
            _isPreview = isPreview;
            _parameters = parameters;

            SetProperties();
        }

        public ModeHelper(ReportGlobals globals, object[] parameters, string whereClause, bool isPreview, string fromClause)
        {
            _globals = globals;
            _whereClause = whereClause;
            _isPreview = isPreview;
            _parameters = parameters;
            _modeFrom = fromClause;

            SetProperties();
        }
        
        public List<T> ApplyFilter<T>(List<T> list) where T : IRecKey
        {
            var modeReckey = GetRecKey();
            if (modeReckey != null)
            {
                list = list.Where(x => modeReckey.Contains(x.RecKey)).ToList();
            }
            return list;
        }

        private List<int> GetRecKey()
        {
            var recSql = SqlProcessor.ProcessSql("DISTINCT T1.reckey", false, _modeFrom, _modeWhere, string.Empty, _globals);
            return ClientDataRetrieval.GetOpenQueryData<int>(recSql, _globals, _parameters).ToList();
        }

        private void SetProperties()
        {
            _modeType = (Mode)Convert.ToInt32(_globals.GetParmValue(WhereCriteria.MODE));
            _modeWhere = GetWhereClause();

            if (string.IsNullOrEmpty(_modeFrom))
            {
                _modeFrom = _isPreview ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
            }

            ModeText = (_modeType == Mode.RAIL) ? "Rail Only" : "Air Only";
        }

        private string GetWhereClause()
        {
            var recWhere = "T1.reckey = T2.reckey";
            if (_modeType == Mode.RAIL)
            {
                recWhere += " and T2.Mode = 'R' and " + _whereClause;
            }
            else if (_modeType == Mode.BOTH)
            {
                recWhere += $" and T2.Mode IN ('A', 'R') and {_whereClause}";
            }
            else
            {
                recWhere += " and T2.Mode = 'A' and " + _whereClause;
            }
            return recWhere;
        }
    }
}
