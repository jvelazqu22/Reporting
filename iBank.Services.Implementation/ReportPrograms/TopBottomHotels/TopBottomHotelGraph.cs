using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using Domain.Models.ReportPrograms.TopBottomHotelsReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public class TopBottomHotelGraph
    {
        public bool GenerateGraph(string sortBy, List<FinalData> FinalDataList, ref string CrystalReportName, bool secondRange, ReportGlobals Globals, 
            ReportDocument ReportSource, string catDesc1, DateTime begDate, DateTime begDate2, DateTime endDate, DateTime endDate2)
        {
            var graphTitle = "Volume Booked";
            switch (sortBy)
            {
                case "2":
                    graphTitle = "# of Stays";
                    break;
                case "3":
                    graphTitle = "# of Nights";
                    break;
                case "4":
                    graphTitle = "Avg Booked Rate";
                    break;
            }

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.Category.Left(16),
                Data1 = new TopBottomHotelGraph().GetGraphData1(s, sortBy),
                Data2 = new TopBottomHotelGraph().GetGraphData2(s, sortBy)
            }).ToList();

            CrystalReportName = !secondRange ? "ibGraph1" : "ibGraph2";

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", "2,3".Contains(sortBy) ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", catDesc1);
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", begDate.ToShortDateString() + " - " + endDate.ToShortDateString());
            if (secondRange)
            {
                ReportSource.SetParameterValue("cGrColHdr2", begDate2.ToShortDateString() + " - " + endDate2.ToShortDateString());
            }
            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

        public decimal GetGraphData1(FinalData rec, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return rec.Stays;
                case "3":
                    return rec.Nights;
                case "4":
                    return rec.Avgbook;
                default:
                    return rec.Hotelcost;
            }
        }

        public decimal GetGraphData2(FinalData rec, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return rec.Stays2;
                case "3":
                    return rec.Nights2;
                case "4":
                    return rec.Avgbook2;
                default:
                    return rec.Hotelcost2;
            }
        }

    }
}
