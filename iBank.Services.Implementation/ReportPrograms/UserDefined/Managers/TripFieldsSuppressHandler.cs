using Domain.Helper;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class TripFieldsSuppressHandler
    {
        private SuppressTripDuplicateType _tripSuppressType { get; set; }

        public TripFieldsSuppressHandler(ReportGlobals globals)
        {
            switch (globals.GetParmValue(WhereCriteria.DDSUPPDUPETRIPFLDS))
            {
                case "1":
                    _tripSuppressType = SuppressTripDuplicateType.TextDateAndNumberFields;
                    break;
                case "2":
                    _tripSuppressType = SuppressTripDuplicateType.TextAndDateFieldsOnly;
                    break;
                case "3":
                    _tripSuppressType = SuppressTripDuplicateType.NumberFieldsOnly;
                    break;
                case "4":
                    _tripSuppressType = SuppressTripDuplicateType.NoSuppress;
                    break;
            }
        }

        public bool IsSpecialField(string fieldName)
        {
            return fieldName.ToUpper() == "RECKEY" || fieldName.ToUpper() == "PLUSMIN" 
                || fieldName.ToUpper() == "LEGPLUSMIN" ||fieldName.ToUpper() == "LEGCNTR";
        }

        public bool IsTripField(string table)
        {
            return table == "HIBTRIPS" || table == "IBTRIPS" || table == "TRIPS";
        }
        
        public bool IsOneOfTextDateNumberFields(string dataType)
        {
            return IsTextField(dataType) || IsDateField(dataType) || IsNumberField(dataType);
        }

        public bool IsOneOfTextDateFields(string dataType)
        {
            return IsTextField(dataType) || IsDateField(dataType);
        }        

        public bool IsDateField(string dataType)
        {
            return dataType == "DATE";
        }
    
        public bool IsNumericField(string dataType)
        {
            return dataType == "NUMERIC";
        }

        public bool IsNumberField(string dataType)
        {
            return dataType == "NUMERIC" || dataType == "CURRENCY";
        }

        public bool IsTextField(string dataType)
        {
            return dataType == "TEXT";
        }
        
        public bool IsCurrencyField(string dataType)
        {
            return dataType == "CURRENCY";
        }

        public bool IsSuppressNeeded(string fieldName,string dataType, string tableName, bool firstLeg)
        {
            if (firstLeg) return false;

            if (IsSpecialField(fieldName)) return false;

            if (!IsTripField(tableName)) return false;

            switch (_tripSuppressType)
            {
                case SuppressTripDuplicateType.TextDateAndNumberFields:
                    return IsOneOfTextDateNumberFields(dataType);
                case SuppressTripDuplicateType.TextAndDateFieldsOnly:
                    return IsOneOfTextDateFields(dataType);
                case SuppressTripDuplicateType.NumberFieldsOnly:
                    return IsNumberField(dataType);
                case SuppressTripDuplicateType.NoSuppress:
                    return false;
            }           

            return true;
        }

        
        public string GetDataAccordingToTheSuppressType(string value, string fieldName, string dataType, string tableName, bool isFirstLeg)
        {
            if (!IsSuppressNeeded(fieldName, dataType, tableName, isFirstLeg))
            {
                return value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}


