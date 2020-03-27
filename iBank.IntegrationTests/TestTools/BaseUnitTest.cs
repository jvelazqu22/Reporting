using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Context;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes.Interfaces;
using iBank.Services.Implementation.ReportPrograms;

namespace iBank.IntegrationTests.TestTools
{
    public class BaseUnitTest
    {
        public const int UserNumber = 1602;
        public const string CfBox = "keystonecf1";
        public const string Agency = "DEMO";
        

        public ReportGlobals ReportGlobals { get; set; }
        public List<ReportHandoffInformation> ReportHandoff { get; set; }
        public string ReportId { get; set; }
        public BaseUnitTest()
        {
            ReportId = Guid.NewGuid() + "." + CfBox;
            ReportHandoff = new List<ReportHandoffInformation>();
            ReportGlobals = new ReportGlobals();
        }

        /// <summary>
        /// Creates the report handoff table using the records created in the derived unit test 
        /// </summary>
        public void InsertReportHandoff()
        {
            using (var context = new MasterDbContext())
            {
                var recordNumber = context.reporthandoff.Max(s => s.recordno) + 1;
                foreach (var record in ReportHandoff)
                {
                    //we don't want to generate files
                    if (record.ParmName.Equals("OUTPUTTYPE", StringComparison.OrdinalIgnoreCase))
                        record.ParmValue = "-1";
                    var newrec = new reporthandoff
                    {
                        //recordno = recordNumber++,
                        reportid = ReportId,
                        agency = Agency,
                        usernumber = UserNumber,
                        cfbox = CfBox,
                        svrnumber = 0,
                        parmname = record.ParmName,
                        parminout = record.ParmInOut,
                        parmvalue = record.ParmValue,
                        langCode = record.LangCode,
                        datecreated = DateTime.Now
                    };

                    context.reporthandoff.Add(newrec);
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes all records with the current report id
        /// </summary>
        public void ClearReportHandoff()
        {
            
            using (var context = new MasterDbContext())
            {
                var recsToRemove = context.reporthandoff.Where(s => s.reportid.Equals(ReportId));
                context.reporthandoff.RemoveRange(recsToRemove);
                context.SaveChanges();
            }
        }

        public IReportRunner RunReport()
        {
            var pri = new PendingReportInformation
                          {
                              ReportId = ReportId,
                              Agency = Agency,
                              ColdFusionBox = CfBox,
                              UserNumber = UserNumber,
                              ServerNumber = 0
                          };

            var rptSwitch = new ReportSwitch { IsOfflineServer = false, DoneEvent = new System.Threading.ManualResetEvent(false), DevMode = true};
            rptSwitch.RunReport(pri);

            return rptSwitch.ReportRunner;
        }

        public void ManipulateReportHandoffRecords(string newVal, string parmName)
        {
            try
            {
                var handoff = ReportHandoff.First(x => x.ParmName == parmName);
                if (handoff != null)
                {
                    ReportHandoff.RemoveAll(x => x.ParmName == parmName);
                    handoff.ParmValue = newVal;
                    ReportHandoff.Add(handoff);
                }
                else
                {
                    var rpi = new ReportHandoffInformation
                    {
                        ParmName = parmName,
                        ParmValue = newVal,
                        ParmInOut = "IN",
                        LangCode = ""
                    };

                    ReportHandoff.Add(rpi);
                }
            }
            catch (Exception)
            {
                var rpi = new ReportHandoffInformation
                {
                    ParmName = parmName,
                    ParmValue = newVal,
                    ParmInOut = "IN",
                    LangCode = ""
                };

                ReportHandoff.Add(rpi);
            }
        }


    }

    
}
