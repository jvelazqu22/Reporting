using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.Utilities
{
    public class XmlExporter
    {
        public ReportGlobals Globals { get; set; }
        public List<XmlExtract.XmlTag> Tags { get; set; }
        public void CreateXml(iXML ixml)
        {
            //TODO: This had been retrieved in the calling program, but it's only needed here, and I assume we'll need other globals. 
            var multiPaxOption = Globals.GetParmValue(WhereCriteria.DDPASSENGERXMLRECORD);

            var ixmlOverrides = new XmlAttributeOverrides();

            #region Overrides for First Destination and Origin
            if (ixml.iBankReportCriteria.Origination.IsFirst)
            {
                AddNameOverride(ixmlOverrides, "Origination", "FirstOrigination", typeof(iXMLIBankReportCriteria));
            }
            if (ixml.iBankReportCriteria.Destination.IsFirst)
            {
                AddNameOverride(ixmlOverrides, "Destination", "FirstDestination", typeof(iXMLIBankReportCriteria));
            }
            if (ixml.iBankReportCriteria.OriginCountry.IsFirst)
            {
                AddNameOverride(ixmlOverrides, "OriginCountry", "FirstOriginCountry", typeof(iXMLIBankReportCriteria));
            }
            if (ixml.iBankReportCriteria.DestinationCountry.IsFirst)
            {
                AddNameOverride(ixmlOverrides, "DestinationCountry", "FirstDestinationCountry", typeof(iXMLIBankReportCriteria));
            }
            if (ixml.iBankReportCriteria.OriginRegion.IsFirst)
            {
                AddNameOverride(ixmlOverrides, "OriginRegion", "FirstOriginRegion", typeof(iXMLIBankReportCriteria));
            }
            if (ixml.iBankReportCriteria.DestinationRegion.IsFirst)
            {
                AddNameOverride(ixmlOverrides, "DestinationRegion", "FirstDestinationRegion", typeof(iXMLIBankReportCriteria));
            }
            #endregion

            //if there are at least two criteria, add the "and/or" property
            if (ixml.iBankReportCriteria.AdvancedCriteria.Criteria01 != null && ixml.iBankReportCriteria.AdvancedCriteria.Criteria02 != null)
            {
                OverrideIgnore(ixmlOverrides, "CondtionsJoinedBy", typeof(iXMLIBankReportCriteriaAdvancedCriteria));
            }
            //check for items that are "on" in the Order tag
            foreach (var tag in Tags.Where(s => s.TagType.EqualsIgnoreCase("TRP")))
            {
                if (!tag.IsOn)
                    IgnoreAttribute(ixmlOverrides, tag.TagName, typeof(iXMLIBank_TravelItineraryTravelItineraryOrder));
                //if (tag.IsRenamed)
                    AddNameOverride(ixmlOverrides, tag.TagName, tag.TagName + "2", typeof(iXMLIBank_TravelItineraryTravelItineraryOrder));
            }



            var xs = new XmlSerializer(typeof(iXML), ixmlOverrides);

            using (var sw = new StreamWriter("C:\\iBank.Reports\\XML\\temp.xml"))
            {
                xs.Serialize(sw, ixml);
            }
        }

        private static void AddNameOverride(XmlAttributeOverrides myOverrides, string originalName, string newName, System.Type type)
        {
            var myElementAttribute = new XmlElementAttribute();
            myElementAttribute.ElementName = newName;
            var myAttributes = new XmlAttributes();
            myAttributes.XmlElements.Add(myElementAttribute);

            myOverrides.Add(type, originalName, myAttributes);
        }

        private static void OverrideIgnore(XmlAttributeOverrides myOverrides, string propertyName, System.Type type)
        {
            var myAttributes = new XmlAttributes();
            myAttributes.XmlIgnore = false;
            myOverrides.Add(type, propertyName, myAttributes);
        }

        private static void IgnoreAttribute(XmlAttributeOverrides myOverrides, string propertyName, System.Type type)
        {
            var myAttributes = new XmlAttributes();
            myAttributes.XmlIgnore = true;
            myOverrides.Add(type, propertyName, myAttributes);
        }
    }



    //public class iXml
    //{
    //    public iXml()
    //    {
    //        iBankReportCriteria = string.Empty;
    //        ReportName = string.Empty;
    //        DataType = string.Empty;
    //        OutputTo = string.Empty;
    //        Format = string.Empty;
    //        DateRangeType = string.Empty;
    //        DateFrom = string.Empty;
    //        DateTo = string.Empty;
    //        ParentAccounts = new ParentAccounts();
    //        CustomerNumbers = new CustomerNumbers();
    //        Break1 = new Break();
    //        Break2 = new Break();
    //        Break3 = new Break();
    //        ValidatingCarrier = new ValidatingCarrier();
    //        Origination = new Origination();
    //        Destination = new Destination();
    //        OriginCountry = new RoutingCountry();
    //        DestinationCountry = new RoutingCountry();
    //        OriginRegion = new RoutingRegion();
    //        DestinationRegion = new RoutingRegion();
    //        TicketNum = string.Empty;
    //        RecordLocator = string.Empty;
    //        InvoiceNum = string.Empty;
    //        PassLastName = string.Empty;
    //        PassFirstName = string.Empty;
    //        CreditCardNum = string.Empty;
    //        Pseudocity = string.Empty;
    //        Branch = string.Empty;
    //        AgentID = string.Empty;
    //        UDIDNum = string.Empty;
    //        UDIDText = string.Empty;
    //        DomesticInternationalTransborder = string.Empty;
    //        AdvancedCriteria = new AdvancedCriteria();
    //    }
    //    public string iBankReportCriteria { get; set; }
    //    public string ReportName { get; set; }
    //    public string DataType { get; set; }
    //    public string OutputTo { get; set; }
    //    public string Format { get; set; }
    //    public string DateRangeType { get; set; }
    //    public string DateFrom { get; set; }
    //    public string DateTo { get; set; }
    //    public ParentAccounts ParentAccounts { get; set; }
    //    public CustomerNumbers CustomerNumbers { get; set; }
    //    public Break Break1 { get; set; }
    //    public Break Break2 { get; set; }
    //    public Break Break3 { get; set; }
    //    public ValidatingCarrier ValidatingCarrier { get; set; }

    //    public Origination Origination { get; set; }
    //    public Destination Destination { get; set; }
    //    public RoutingCountry OriginCountry { get; set; }
    //    public RoutingCountry DestinationCountry { get; set; }
    //    public RoutingRegion OriginRegion { get; set; }
    //    public RoutingRegion DestinationRegion { get; set; }
    //    public string TicketNum { get; set; }
    //    public string RecordLocator { get; set; }
    //    public string InvoiceNum { get; set; }
    //    public string PassLastName { get; set; }
    //    public string PassFirstName { get; set; }
    //    public string CreditCardNum { get; set; }
    //    public string Pseudocity { get; set; }
    //    public string Branch { get; set; }
    //    public string AgentID { get; set; }
    //    public string UDIDNum { get; set; }
    //    public string UDIDText{ get; set; }
    //    public string DomesticInternationalTransborder { get; set; }
    //    public AdvancedCriteria AdvancedCriteria { get; set; }

    //}

    //public class ParentAccounts
    //{
    //    public ParentAccounts()
    //    {
    //        Accounts = string.Empty;
    //        NotInParentAccount = false;
    //    }
    //    [XmlAttribute]
    //    public string Accounts { get; set; }
    //    [XmlAttribute]
    //    public bool NotInParentAccount { get; set; }
    //}

    //public class CustomerNumbers
    //{
    //    public CustomerNumbers()
    //    {
    //        Numbers = string.Empty;
    //        NotInCustomerNumbers = false;
    //    }
    //    [XmlAttribute]
    //    public string Numbers { get; set; }
    //    [XmlAttribute]
    //    public bool NotInCustomerNumbers { get; set; }
    //}

    //public class Break
    //{
    //    public Break()
    //    {
    //        In = string.Empty;
    //        NotIn = false;
    //        UserName = string.Empty;
    //    }
    //    [XmlAttribute]
    //    public string In { get; set; }
    //    [XmlAttribute]
    //    public bool NotIn { get; set; }
    //    [XmlAttribute]
    //    public string UserName { get; set; }
    //}

    //public class ValidatingCarrier
    //{
    //    public ValidatingCarrier()
    //    {
    //        Carrier = string.Empty;
    //        NotInCarrier = false;
    //    }
    //    [XmlAttribute]
    //    public string Carrier { get; set; }
    //    [XmlAttribute]
    //    public bool NotInCarrier { get; set; }
    //}

    
    //public class Origination
    //{
    //    public Origination()
    //    {
    //        Origin = string.Empty;
    //        NotInOrigin = false;
    //        IsFirst = false;
    //    }

    //    [XmlAttribute]
    //    public string Origin { get; set; }
    //    [XmlAttribute]
    //    public bool NotInOrigin { get; set; }
    //    [XmlIgnore]
    //    public bool IsFirst { get; set; }
    //}

    //public class Destination
    //{
    //    public Destination()
    //    {
    //        Destin = string.Empty;
    //        NotInDestin = false;
    //        IsFirst = false;
    //    }

    //    [XmlAttribute]
    //    public string Destin { get; set; }
    //    [XmlAttribute]
    //    public bool NotInDestin { get; set; }
    //    [XmlIgnore]
    //    public bool IsFirst { get; set; }
    //}

    //public class RoutingCountry
    //{
    //    public RoutingCountry()
    //    {
    //        Country = string.Empty;
    //        NotInCountry = false;
    //        IsFirst = false;
    //    }

    //    [XmlAttribute]
    //    public string Country { get; set; }
    //    [XmlAttribute]
    //    public bool NotInCountry { get; set; }
    //    [XmlIgnore]
    //    public bool IsFirst { get; set; }
    //}

    //public class RoutingRegion
    //{
    //    public RoutingRegion()
    //    {
    //        Region = string.Empty;
    //        NotInRegion = false;
    //        IsFirst = false;
    //    }

    //    [XmlAttribute]
    //    public string Region { get; set; }
    //    [XmlAttribute]
    //    public bool NotInRegion { get; set; }
    //    [XmlIgnore]
    //    public bool IsFirst { get; set; }
    //}

    //public class AdvancedCriteria
    //{
    //    public AdvancedCriteria()
    //    {
    //        Criteria01 = new AdvancedCriterion();
    //        Criteria02 = new AdvancedCriterion();
    //        Criteria03 = new AdvancedCriterion();
    //        Criteria04 = new AdvancedCriterion();
    //        Criteria05 = new AdvancedCriterion();
    //        Criteria06 = new AdvancedCriterion();
    //        Criteria07 = new AdvancedCriterion();
    //        Criteria08 = new AdvancedCriterion();
    //        Criteria09 = new AdvancedCriterion();
    //        Criteria10 = new AdvancedCriterion();
    //        CondtionsJoinedBy = "AND";
    //    }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria01 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria02 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria03 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria04 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria05 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria06 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria07 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria08 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria09 { get; set; }
    //    [XmlIgnore]
    //    public AdvancedCriterion Criteria10 { get; set; }
    //    [XmlIgnore]
    //    public string CondtionsJoinedBy { get; set; }//Misspelled on purpose...should we fix this and risk breaking customer imports?  
       
    //}

    //public class AdvancedCriterion
    //{
    //    public AdvancedCriterion()
    //    {
    //        Criteria = string.Empty;
    //        FieldName = string.Empty;
    //        Value = string.Empty;
    //    }
    //    [XmlElement]
    //    public bool Exists { get { return FieldName.Trim().Length > 0; } }
    //    [XmlAttribute]
    //    public string FieldName { get; set; }
    //    [XmlAttribute]
    //    public string Criteria { get; set; }
    //    [XmlAttribute]
    //    public string Value { get; set; }
    //}
    
}
