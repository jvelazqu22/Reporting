using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Server.Utilities.Classes;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class EProfileFileMaskHandler
    {
        private readonly string _daty = "$DATY";

        private readonly string _enty = "$ENTY";

        private readonly string _user = "$USER";

        private readonly string _eprf = "$EPRF";

        private readonly string _trdg = "$TRDG";

        private readonly string _tmpl = "$TMPL";

        private readonly string _style = "$STYLE";

        private readonly string _date = "$DATE";

        private readonly string _time = "$TIME";

        private readonly string _fixed = "FIXED:";
        public bool NeedToApplyMask(string fileNameMask)
        {
            return fileNameMask.Contains(_daty)
                || fileNameMask.Contains(_enty)
                || fileNameMask.Contains(_user)
                || fileNameMask.Contains(_eprf)
                || fileNameMask.Contains(_trdg)
                || fileNameMask.Contains(_tmpl)
                || fileNameMask.Contains(_style)
                || fileNameMask.Contains(_date)
                || fileNameMask.Contains(_time)
                || (fileNameMask.Length >= 6 && fileNameMask.Substring(0, 6).ToUpper().Equals(_fixed));
        }

        public string ApplyMask(ReportGlobals globals, EffectsOutputInformation eProfileInfo,
            XmlReport xmlReport, int styleGroupNumber, TimeStrings timeStrings, IQuery<StyleGroup> getStyleGroupsQuery)
        {
            var fileNameMask = eProfileInfo.FileNameMask;
            //if the fileNameMask begins with "FIXED:" then use the text after "FIXED:" as file name, else construct file name
            if (fileNameMask.Length >= 6 && fileNameMask.Substring(0, 6).ToUpper().Equals("FIXED:"))
            {
                return eProfileInfo.Outbox.AddBS() + fileNameMask.SubStr(7, fileNameMask.Length - 1);
            }

            var outputFile = eProfileInfo.Outbox.AddBS() + eProfileInfo.FileNameMask;

            if (fileNameMask.Contains(_daty))
            {
                var daty = "";
                var reportType = globals.GetParmValue(WhereCriteria.PREPOST);
                if (reportType == "1") daty = "ib";
                else if (reportType == "2") daty = "hib";

                if (!string.IsNullOrEmpty(daty))
                {
                    outputFile = outputFile.Replace(_daty, daty);
                }
            }

            if (fileNameMask.Contains(_enty))
            {
                outputFile = outputFile.Replace(_enty, globals.Agency);
            }

            if (fileNameMask.Contains(_user))
            {
                outputFile = outputFile.Replace(_user, globals.User.UserId);
            }

            if (fileNameMask.Contains(_eprf))
            {
                outputFile = outputFile.Replace(_eprf, eProfileInfo.ProfileName);
            }

            if (fileNameMask.Contains(_trdg))
            {
                outputFile = outputFile.Replace(_trdg, eProfileInfo.TradingPartnerName);
            }

            if (fileNameMask.Contains(_tmpl))
            {
                outputFile = outputFile.Replace(_tmpl, xmlReport != null ? xmlReport.CrName : "");
            }

            if (fileNameMask.Contains(_style))
            {
                var styleGroupName = "";
                if (styleGroupNumber > 0)
                {
                    var styleRecord = getStyleGroupsQuery.ExecuteQuery();

                    if (styleRecord != null)
                    {
                        styleGroupName = styleRecord.SGroupName.Trim();
                    }
                }

                outputFile = outputFile.Replace(_style, styleGroupName);
            }

            if (fileNameMask.Contains(_date))
            {
                var date = timeStrings.Year + timeStrings.Month + timeStrings.Day;
                outputFile = outputFile.Replace(_date, date);
            }

            if (fileNameMask.Contains(_time))
            {
                var time = timeStrings.Hour + timeStrings.Min + timeStrings.Sec + timeStrings.Ms;
                outputFile = outputFile.Replace(_time, time);
            }

            return outputFile;
        }
    }
}
