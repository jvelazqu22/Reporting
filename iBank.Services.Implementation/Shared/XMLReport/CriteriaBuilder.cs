using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using System;
using System.Xml.Linq;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public class CriteriaBuilder
    {
        private readonly ReportGlobals _globals;
        private readonly XNamespace _xmlns;
        private ElementBuilder _elementBuilder;
        public CriteriaBuilder(ReportGlobals globals, XNamespace xmlns)
        {
            _globals = globals;
            _xmlns = xmlns;
            _elementBuilder = new ElementBuilder(xmlns);
        }

        public XElement Build(XmlTag mainTag, String title, string format)
        {
            var crit = new XElement(_xmlns + mainTag.ActiveName);

            crit.Add(new XElement(_xmlns + "ReportName", ValueConverter.ConvertValue(title, false)));
            crit.Add(new XElement(_xmlns + "DataType", XMLReportValueLooksup.GetValueFromList(XMLReportValueLooksup.GetIndex(_globals.GetParmValue(WhereCriteria.PREPOST)), XMLReportConstants.DataTypeList)));
            crit.Add(new XElement(_xmlns + "OutputTo", XMLReportValueLooksup.GetValueFromList(XMLReportValueLooksup.GetIndex(_globals.GetParmValue(WhereCriteria.OUTPUTDEST)), XMLReportConstants.OutputToList)));

            crit.Add(new XElement(_xmlns + "Format", format));

            var dateRange = _globals.GetParmValue(WhereCriteria.DATERANGE);
            var index = XMLReportValueLooksup.GetIndex(dateRange);
            var xrmReportValue = XMLReportValueLooksup.GetValueFromList(index, XMLReportConstants.DateRangeTypeList);
            crit.Add(new XElement(_xmlns + "DateRangeType", xrmReportValue));

            crit.Add(new XElement(_xmlns + "DateFrom", _globals.BeginDate.Value));
            crit.Add(new XElement(_xmlns + "DateTo", _globals.EndDate.Value));

            //Parent Accounts
            var parentAccounts = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INPARENTACCT), false);
            var notInParentAccounts = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINPARENTACCT), false);
            crit.Add(new XElement(_xmlns + "ParentAccounts", new XAttribute("Accounts", parentAccounts),
                new XAttribute("NotInParentAccount", notInParentAccounts)));

            //Customer Numbers
            var accts = _globals.GetParmValue(WhereCriteria.INACCT);
            if (string.IsNullOrEmpty(accts))
            {
                accts = _globals.GetParmValue(WhereCriteria.ACCT);
            }
            var pl = new PickListParms(_globals);
            pl.ProcessList(accts, "ACCTS", "ACCTS");

            var list = ValueConverter.ConvertValue(pl.PickListString, false);
            var notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINACCT), false);
            crit.Add(new XElement(_xmlns + "CustomerNumbers", new XAttribute("Numbers", list),
                new XAttribute("NotInCustomerNumbers", notInlist)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INBREAK1), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINBRK1), false);
            var userNameBreak = ValueConverter.ConvertValue(_globals.User.Break1Name, false).Replace(":", string.Empty);
            //THE BREAK NAME VARIABLE VALUES TYPICALLY END WITH A COLON FOR RPT DISPLAY PURPORSES - REMOVE IT.
            crit.Add(new XElement(_xmlns + "Break1", new XAttribute("In", list), new XAttribute("NotIn", notInlist),
                new XAttribute("UserName", userNameBreak)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INBREAK2), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINBRK2), false);
            userNameBreak = ValueConverter.ConvertValue(_globals.User.Break2Name, false).Replace(":", string.Empty);
            //THE BREAK NAME VARIABLE VALUES TYPICALLY END WITH A COLON FOR RPT DISPLAY PURPORSES - REMOVE IT.
            crit.Add(new XElement(_xmlns + "Break2", new XAttribute("In", list), new XAttribute("NotIn", notInlist),
                new XAttribute("UserName", userNameBreak)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INBREAK3), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINBRK3), false);
            userNameBreak = ValueConverter.ConvertValue(_globals.User.Break3Name, false).Replace(":", string.Empty);
            //THE BREAK NAME VARIABLE VALUES TYPICALLY END WITH A COLON FOR RPT DISPLAY PURPORSES - REMOVE IT.
            crit.Add(new XElement(_xmlns + "Break3", new XAttribute("In", list), new XAttribute("NotIn", notInlist),
                new XAttribute("UserName", userNameBreak)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INVALCARR), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINVALCARR), false);
            crit.Add(new XElement(_xmlns + "ValidatingCarrier", new XAttribute("Carrier", list),
                new XAttribute("NotInCarrier", notInlist)));

            var firstOrigin = _globals.IsParmValueOn(WhereCriteria.FIRSTORIGIN);
            var firstDest = _globals.IsParmValueOn(WhereCriteria.FIRSTDEST);

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INORGS), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINORIGIN), false);
            var fieldName = firstOrigin ? "FirstOrigination" : "Origination";
            crit.Add(new XElement(_xmlns + fieldName, new XAttribute("Origin", list), new XAttribute("NotInOrigin", notInlist)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INDESTS), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINDESTS), false);
            fieldName = firstDest ? "FirstDestination" : "Destination";
            crit.Add(new XElement(_xmlns + fieldName, new XAttribute("Destin", list), new XAttribute("NotInDestin", notInlist)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INORIGCOUNTRY), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINORIGCOUNTRY), false);
            fieldName = firstOrigin ? "FirstOriginCountry" : "OriginCountry";
            crit.Add(new XElement(_xmlns + fieldName, new XAttribute("Country", list), new XAttribute("NotInCountry", notInlist)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INDESTCOUNTRY), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINDESTCOUNTRY), false);
            fieldName = firstDest ? "FirstDestinationCountry" : "DestinationCountry";
            crit.Add(new XElement(_xmlns + fieldName, new XAttribute("Country", list), new XAttribute("NotInCountry", notInlist)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INORIGREGION), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINORIGREGION), false);
            fieldName = firstOrigin ? "FirstOriginRegion" : "OriginRegion";
            crit.Add(new XElement(_xmlns + fieldName, new XAttribute("Region", list), new XAttribute("NotInRegion", notInlist)));

            list = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INDESTREGION), false);
            notInlist = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.NOTINDESTREGION), false);
            fieldName = firstDest ? "FirstDestinationRegion" : "DestinationRegion";
            crit.Add(new XElement(_xmlns + fieldName, new XAttribute("Region", list), new XAttribute("NotInRegion", notInlist)));

            var item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.TICKET), false);
            crit.Add(_elementBuilder.GetSimpleXElement("TicketNum", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.RECLOC), false);
            crit.Add(_elementBuilder.GetSimpleXElement("RecordLocator", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.INVOICE), false);
            crit.Add(_elementBuilder.GetSimpleXElement("InvoiceNum", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.PASSLAST), false);
            crit.Add(_elementBuilder.GetSimpleXElement("PassLastName", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.PASSFIRST), false);
            crit.Add(_elementBuilder.GetSimpleXElement("PassFirstName", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.CREDCARD), false);
            crit.Add(_elementBuilder.GetSimpleXElement("CreditCardNum", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.PSEUDOCITY), false);
            crit.Add(_elementBuilder.GetSimpleXElement("Pseudocity", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.BRANCH), false);
            crit.Add(_elementBuilder.GetSimpleXElement("Branch", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.AGENTID), false);
            crit.Add(_elementBuilder.GetSimpleXElement("AgentID", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.UDIDNBR), false);
            crit.Add(_elementBuilder.GetSimpleXElement("UDIDNum", item));
            item = ValueConverter.ConvertValue(_globals.GetParmValue(WhereCriteria.UDIDTEXT), false);
            crit.Add(_elementBuilder.GetSimpleXElement("UDIDText", item));

            var ditCode = _globals.GetParmValue(WhereCriteria.DOMINTL);
            item = XMLReportValueLooksup.GetValueFromList(XMLReportValueLooksup.GetIndex(ditCode), XMLReportConstants.DomesticIntList);
            crit.Add(_elementBuilder.GetSimpleXElement("DomesticInternationalTransborder", item));

            var advCrit = new XElement(_xmlns + "AdvancedCriteria");
            crit.Add(advCrit);
            var advCounter = 1;
            foreach (var parm in _globals.AdvancedParameters.Parameters)
            {
                var critName = "Criteria" + advCounter.ToString().PadLeft(2, '0');
                var op = parm.Operator.ToString();
                switch (parm.Operator)
                {
                    case Operator.Equal:
                        op = "EQ";
                        break;
                    case Operator.GreaterThan:
                        op = "GT";
                        break;
                    case Operator.GreaterOrEqual:
                        op = "GE";
                        break;
                    case Operator.Lessthan:
                        op = "LT";
                        break;
                    case Operator.LessThanOrEqual:
                        op = "LE";
                        break;
                    case Operator.NotEqual:
                        op = "NE";
                        break;
                }
                var val = ValueConverter.ConvertValue(parm.Value1, false);
                if (!string.IsNullOrEmpty(parm.Value2))
                    val = val + " and " + ValueConverter.ConvertValue(parm.Value2, false);

                advCrit.Add(new XElement(_xmlns + critName, new XAttribute("FieldName", parm.FieldName),
                    new XAttribute("Criteria", op), new XAttribute("Value", val)));

                advCounter++;
            }
            if (_globals.AdvancedParameters.Parameters.Count > 1)
            {
                advCrit.Add(new XElement(_xmlns + "CondtionsJoinedBy", _globals.AdvancedParameters.AndOr.ToString().ToUpper()));
            }
            return crit;
        }

    }
}
