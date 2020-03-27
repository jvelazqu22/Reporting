using Domain.Orm.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers
{
    public class XmlDataStructure
    {

        private readonly ReportGlobals _globals;
        private int _reportKey { get; set; }
        private bool _isCustomExport { get; set; }

        public List<XmlTag> Tags;
        public bool HasError { get; set; }
        public bool NeedCancelledTrips { get; set; }
        public XmlExport XmlDetails { get; set; }
        public DataSwitches DataSwitches { get; set; }
        protected iBankClientQueryable ClientQueryableDb => new iBankClientQueryable(_globals.AgencyInformation.ServerName, _globals.AgencyInformation.DatabaseName);
        protected iBankMastersQueryable MasterQueryableDb => new iBankMastersQueryable();

        public XmlDataStructure(int reportKey, ReportGlobals globals)
        {
            _reportKey = reportKey;
            _globals = globals;
        }
        public void RetrieveStructure()
        {
            HasError = false;
            if (_reportKey >= 0)
            {
                //use client
                var getCustomXmlTagsQuery = new GetCustomXmlTagsByReportKeyQuery(ClientQueryableDb, _reportKey);
                Tags = getCustomXmlTagsQuery.ExecuteQuery().ToList();

                var exportQuery = new GetCustomXmlExportTypeQuery(ClientQueryableDb, _reportKey);

                var exp = exportQuery.ExecuteQuery();
                if (exp == null)
                {
                    _globals.ReportInformation.ReturnCode = 2;
                    _globals.ReportInformation.ErrorMessage = "Error: Main record not found for this report.";
                    HasError = true;
                }
                XmlDetails = new XmlExport { Name = exp.crname, Type = exp.exportType, Title = exp.crtitle };
            }
            else
            {
                //use master
                var key = Math.Abs(_reportKey);
                var getMasterXmlTagsQuery = new GetMasterXmlTagsByReportKeyQuery(MasterQueryableDb, key);
                Tags = getMasterXmlTagsQuery.ExecuteQuery().ToList();

                var exportQuery = new GetStandardXmlExportTypeQuery(MasterQueryableDb, key);
                var exp = exportQuery.ExecuteQuery();
                if (exp == null)
                {
                    _globals.ReportInformation.ReturnCode = 2;
                    _globals.ReportInformation.ErrorMessage = "Error: Main record not found for this report.";
                    HasError = true;
                }
                XmlDetails = new XmlExport { Name = exp.crname, Type = exp.exportType, Title = exp.crname };
            }

            if (!Tags.Any(s => s.IsOn))
            {
                _globals.ReportInformation.ReturnCode = 2;
                _globals.ReportInformation.ErrorMessage = "Error: No data elements are selected for this report.";
                HasError = true;

            }

            NeedCancelledTrips = Tags.Any(s => s.TagName.Trim().ToUpper().Equals("ORDERCHANGEREASON"));
            DataSwitches = new DataSwitches();
            DataSwitches.SetDataSwitches(Tags);

            if (DataSwitches.NoSwitches)
            {
                _globals.ReportInformation.ReturnCode = 2;
                _globals.ReportInformation.ErrorMessage = "Error: No data elements are selected for this report.";
                HasError = true;
            }
        }
        
    }
}
