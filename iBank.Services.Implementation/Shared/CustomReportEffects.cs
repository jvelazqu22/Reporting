using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities.eFFECTS;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.Shared
{
    public static class CustomReportEffects
    {

        public static void ApplyEffects(string filename, ReportGlobals globals)
        {
            if (globals.IsEffectsDelivery)
            {
                var handler = new EffectsOuputHandler(globals.EProfileNumber,
                    new iBankClientQueryable(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName), new iBankMastersQueryable());
                var returnInfo = handler.Process(globals.ProcessKey, globals, globals.User.SGroupNumber, filename);

                globals.ReportInformation.ReturnCode = returnInfo.ReturnCode;
                globals.ReportInformation.ReturnText = returnInfo.ReturnMessage;
            }
        }
    }
}
