using CrystalDecisions.CrystalReports.Engine;

using System.Collections.Generic;

using Domain.Helper;

namespace iBank.Services.Implementation.Utilities
{
    public class FontHandler
    {
        private static readonly List<string> _languageCodesNeedingUnicode = new List<string> { "JP" };
        public ReportDocument DrawPdfWithUserFont(ReportDocument reportSource, string outputLanguage, string userFontType)
        {
            var needsUnicodeFont = NeedsUnicodeFormat(outputLanguage);

            //if needs unicode has to be redrawn no matter what
            if (needsUnicodeFont)
            {
                var unicodeFont = "Arial Unicode MS";

                return DrawPdfWithFontType(reportSource, unicodeFont);
            }

            /*  user defined font is only valid for standard reports - for user defined reports we just stick with the font specified in the crystal
             *  - if it is a user defined report the font type has been set to Constants.UserDefinedReport in ReportGlobal.SetUserFontType()\
             *  - if the user font type is [DEFAULT] let stand the report's controls' font type.
             * */
            if (userFontType == Constants.UserDefinedReport || userFontType == Constants.DefaultFontPlaceholder)
            {
                return reportSource;
            }

            return DrawPdfWithFontType(reportSource, userFontType);
        }

        private static ReportDocument DrawPdfWithFontType(ReportDocument reportSource, string fontType)
        {
            foreach (var obj in reportSource.ReportDefinition.ReportObjects)
            {
                if (obj.GetType() == typeof(FieldHeadingObject))
                {
                    var field = (FieldHeadingObject)obj;
                    field.ApplyFont(new System.Drawing.Font(fontType, field.Font.Size, field.Font.Style, field.Font.Unit, field.Font.GdiCharSet,
                        field.Font.GdiVerticalFont));
                }
                else if (obj.GetType() == typeof(FieldObject))
                {
                    var field = (FieldObject)obj;
                    field.ApplyFont(new System.Drawing.Font(fontType, field.Font.Size, field.Font.Style, field.Font.Unit, field.Font.GdiCharSet,
                        field.Font.GdiVerticalFont));
                }
                else if (obj.GetType() == typeof(TextObject))
                {
                    var field = (TextObject)obj;
                    field.ApplyFont(new System.Drawing.Font(fontType, field.Font.Size, field.Font.Style, field.Font.Unit, field.Font.GdiCharSet,
                        field.Font.GdiVerticalFont));
                }
            }

            return reportSource;
        }

        private static bool NeedsUnicodeFormat(string outputLanguage)
        {
            return _languageCodesNeedingUnicode.Contains(outputLanguage.ToUpper().Trim());
        }
    }
}
