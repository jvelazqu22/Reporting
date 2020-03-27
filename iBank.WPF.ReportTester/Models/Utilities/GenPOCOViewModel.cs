using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CODE.Framework.Wpf.Mvvm;

using iBank.Entities.ClientEntities;
using iBank.Server.Utilities;

namespace iBank.WPF.ReportTester.Models.Utilities
{
    public class GenPOCOViewModel : ViewModel
    {
        public string FieldList { get; set; }
        public ViewAction GenClass { get; set; }
        public string POCOClass { get; set; }

        public GenPOCOViewModel()
        {
            GenClass = new ViewAction(execute: (o, a) => GeneratePocoClass());
        }

        public void GeneratePocoClass()
        {

            //T1.reckey, T1.recloc, acct, break1, break2, break3, depdate, bookdate, passlast, passfrst, pseudocity, domintl, ticket, agentid , trantype 
            // For ibMarket, run for both (h)ibTrip and (h)ibLeg
            var fieldString = FieldList.Trim();
            
            if (string.IsNullOrEmpty(fieldString)) return;
                

            fieldString = fieldString.Replace("T1.", string.Empty)
                .Replace("T2.", string.Empty)
                .Replace("T3.", string.Empty);

            var fields = fieldString.Split(',').Select(s => s.Trim());

            //get rid of table identifiers

            //Given a comma-delimited list of strings, generate a set of properties for a poco class
            var props = new List<string>();
            var ctor = new List<string>();
            //props.Add("public class <CLASSNAME>");
            //props.Add("{");

            ctor.Add("\tpublic RawData()");
            ctor.Add("\t{");

            //get the properties of the hibTrips table
            var tType = typeof(hibtrips);
            var hibTripsProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(ibtrips);
            var ibTripsProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(hiblegs);
            var hibLegsProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(iblegs);
            var ibLegsProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(ibcar);
            var ibCarProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(hibcars);
            var hibCarProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(ibhotel);
            var ibHotelProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(hibhotel);
            var hibHotelProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(ibMiscSegs);
            var ibMiscSegProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(hibMiscSegs);
            var hibMiscSegProperties = new List<PropertyInfo>(tType.GetProperties());

            tType = typeof(ibtravauth);
            var ibTravAuthProperties = new List<PropertyInfo>(tType.GetProperties());

            //Check the properties of each table for a match, then get the type
            foreach (var field in fields.Where(s => !string.IsNullOrEmpty(s)))
            {
                AddAttributes(field, props);

                var prop = hibTripsProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field,prop.PropertyType));
                    continue;
                }
                prop = ibTripsProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = hibLegsProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = ibLegsProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = ibCarProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = hibCarProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = ibHotelProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = hibHotelProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = ibMiscSegProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = hibMiscSegProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                prop = ibTravAuthProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null)
                {
                    props.Add(BuildProperty(field, prop.PropertyType));
                    ctor.Add(BuildDeclaration(field, prop.PropertyType));
                    continue;
                }

                //see if we have an "X as Y" situation that we can handle
                // Check whole word "as" so that "base" for example isn't replaced
                // var checkAs = field.Replace("as", "|").Replace("AS", "|").Replace("As", "|").Replace("aS", "|");
                var checkAs = field.Replace(" as ", " | ").Replace(" AS ", " | ").Replace(" As ", " | ").Replace(" aS ", " | ");
                if (checkAs.Contains("|"))
                {
                    var ca = checkAs.Split('|').Select(s => s.Trim()).ToList();
                    //if we don't have exactly two values, we don't know what to do here, so skip it.
                    if (ca.Count() == 2)
                    {
                        //can we figure out the type? 
                        var fieldTypeName = GetTypeFromData(ca[0]);
                        var fieldName = ca[1];

                        //Create the property as best we can, adding the original "field" as a comment 
                        props.Add("\tpublic " + fieldTypeName + " " + fieldName + " " +
                                  "{ get; set; } //TODO: VERIFY TYPE: " + field);
                        continue;
                    }

                }


                //no match. Add the field,default to string, unless we see "date" in the name. 
                if (field.IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    props.Add(BuildProperty(field, typeof (DateTime)));
                    ctor.Add(BuildDeclaration(field, typeof (DateTime)));
                }
                else
                {
                    props.Add(BuildProperty(field, typeof (string)));
                    ctor.Add(BuildDeclaration(field, typeof(string)));

                }
            }
            
            ctor.Add("\t}");

            //POCOClass = "public class RawData" + Environment.NewLine + "{" + Environment.NewLine + string.Join(Environment.NewLine, ctor) + Environment.NewLine + string.Join(Environment.NewLine, props) + Environment.NewLine + "}";
            POCOClass = "public class RawData" + Environment.NewLine + "{"  + Environment.NewLine + string.Join(Environment.NewLine, props) + Environment.NewLine + "}";
            NotifyChanged("POCOClass");
            //var timeStamp = string.Format("{0}{1}{2}_{3}{4}", DateTime.Now.Day.ToString().PadLeft(2, '0'),
            //    DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Year.ToString().PadLeft(4, '0'),
            //    DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'));
            //var filename = string.Format("c:\\cis reports\\POCO Classes\\class_{0}.txt", timeStamp);
            //File.WriteAllLines(filename, props);
            //Controller.Message("Complete!");
        }

        private static void AddAttributes(string field, List<string> props)
        {
            if (field == "HotCurrTyp")
            {
                props.Add("\t[HotelCurrency]");
            }
            if (field == "CarCurrTyp")
            {
                props.Add("\t[CarCurrency]");
            }
            if (field == "AirCurrTyp")
            {
                props.Add("\t[AirCurrency]");
            }

            if (field == "HotelExchangeDate" || field == "CarExchangeDate")
            {
                props.Add("\t[ExchangeDate1]");
            }
          
        }

        private object GetTypeFromData(string val)
        {
            decimal testDec;
            if (decimal.TryParse(val, out testDec)) return "decimal";

            int testInt;
            if (Int32.TryParse(val, out testInt)) return "int";

            //dates and strings will have single quotes around them
            val = val.Replace("'", string.Empty);

            DateTime testDate;
            if (DateTime.TryParse(val, out testDate)) return "DateTime";

            return "string";
        }

        public string BuildDeclaration(string fieldName, Type fieldType)
        {
            fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
            //Known properties
            switch (fieldName.ToUpper())
            {
                case "PLUSMIN":
                    return "\t\tPlusmin = 0;"; // "\tpublic int PlusMin { get; set; }";
                case "DITCODE":
                    return "\t\tDitCode = string.Empty;"; //"\tpublic string DitCode { get; set; }";
                case "ORIGIN":
                    return "\t\tOrigin = string.Empty;"; //"\tpublic string Origin { get; set; }";
                case "DESTINAT":
                    return "\t\tDestinat = string.Empty;"; //"\tpublic string Destinat { get; set; }";
                case "MODE":
                    return "\t\tMode = string.Empty;"; //"\tpublic string Mode { get; set; }";
                case "AIRLINE":
                    return "\t\tAirline = string.Empty;"; //"\tpublic string Airline { get; set; }";
                case "RECKEY":
                    return "\t\tRecKey = 0;"; //"\tpublic int RecKey { get; set; }";
                case "SEQNO":
                    return "\t\tSeqNo = 0;"; //"\tpublic int SeqNo { get; set; }";
                case "HOTELEXCHANGEDATE":
                    return "\t\tHotelExchangeDate = DateTime.MinValue;";
                case "CCBEGDATE":
                    return "\t\tCcBegDate = DateTime.MinValue;";
                case "CCDAYS":
                    return "\t\tCcDays = 0;";
            }
            var defaultValue = string.Empty;

            var checkForNullables = false;

            switch (fieldType.Name)
            {
                case "String":
                    defaultValue = "string.Empty";
                    break;
                case "Int32":
                    defaultValue = "0";
                    break;
                default:
                    defaultValue = "DEFAULT VALUE";
                    checkForNullables = true;
                    break;

            }

            if (checkForNullables)
            {
                if (fieldType == typeof(DateTime?))
                {
                    defaultValue = "DateTime.MinValue";
                }
                if (fieldType == typeof(decimal?))
                {
                    defaultValue = "0m";
                }
                if (fieldType == typeof(bool?))
                {
                    defaultValue = "false";
                }
                if (fieldType == typeof(Int16?) || fieldType == typeof(Int32?))
                {
                    defaultValue = "0";
                }
            }


            return "\t\t" + fieldName + " = " + defaultValue + ";";
        }

        public string BuildProperty(string fieldName, Type fieldType)
        {
            
            fieldName = Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
            //Known properties
            switch (fieldName.ToUpper())
            {
                //case "PLUSMIN":
                //    return "\tpublic int PlusMin { get; set; }";
                case "DITCODE":
                    return "\tpublic string DitCode { get; set; } = string.Empty;";
                case "ORIGIN":
                    return "\tpublic string Origin { get; set; } = string.Empty;";
                case "DESTINAT":
                    return "\tpublic string Destinat { get; set; } = string.Empty;";
                case "MODE":
                    return "\tpublic string Mode { get; set; } = string.Empty;";
                case "AIRLINE":
                    return "\tpublic string Airline { get; set; } = string.Empty;";
                case "RECKEY":
                    return "\tpublic int RecKey { get; set; } = 0;";
                case "SEQNO":
                    return "\tpublic int SeqNo { get; set; } = 0;";
                case "HOTELEXCHANGEDATE":
                    return "\tpublic DateTime HotelExchangeDate { get; set; } = DateTime.MinValue;";
                case "CCBEGDATE":
                    return "\tpublic DateTime CcBegDate { get; set; } = DateTime.MinValue;";
                case "CCDAYS":
                    return "\tpublic int CcDays { get; set; } = 0;";

            }
            var fieldTypeName = string.Empty;
            var initializer = " = string.Empty;";
            var checkForNullables = false;

            switch (fieldType.Name)
            {
                case "String":
                    fieldTypeName = "string";
                    break;
                case "Int32":
                    fieldTypeName = "int";
                    initializer = " = 0;";
                    break;
                default:
                    fieldTypeName = "TYPE";
                    checkForNullables = true;
                    break;

            }

            if (checkForNullables)
            {
                if (fieldType == typeof(DateTime?))
                {
                    fieldTypeName = "DateTime";
                    initializer = " = DateTime.MinValue;";
                }
                if (fieldType == typeof(Decimal?))
                {
                    initializer = " = 0m;";
                    fieldTypeName = "decimal";
                }
                if (fieldType == typeof(bool?))
                {
                    initializer = " = false;";
                    fieldTypeName = "bool";
                }
                if (fieldType == typeof(Int16?))
                {
                    initializer = " = 0;";
                    fieldTypeName = "int";
                }
                if (fieldType == typeof(Int32?))
                {
                    initializer = " = 0;";
                    fieldTypeName = "int";
                }
            }


            return "\tpublic " + fieldTypeName + " " + fieldName + " " + "{ get; set; }" + initializer;
        }
    }
}
