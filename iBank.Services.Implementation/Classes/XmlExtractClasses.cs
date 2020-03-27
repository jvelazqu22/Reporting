using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace iBank.Services.Implementation.Classes
{

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class iXML
    {

        private iXMLIBankReportCriteria iBankReportCriteriaField;

        private iXMLIBank_TravelItinerary[] iBank_TravelItineraryField;

        /// <remarks/>
        public iXMLIBankReportCriteria iBankReportCriteria
        {
            get
            {
                return iBankReportCriteriaField;
            }
            set
            {
                iBankReportCriteriaField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("iBank_TravelItinerary")]
        public iXMLIBank_TravelItinerary[] iBank_TravelItinerary
        {
            get
            {
                return iBank_TravelItineraryField;
            }
            set
            {
                iBank_TravelItineraryField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteria
    {

        private string reportNameField;

        private string dataTypeField;

        private string outputToField;

        private string formatField;

        private string dateRangeTypeField;

        private string dateFromField;

        private string dateToField;

        private iXMLIBankReportCriteriaParentAccounts parentAccountsField;

        private iXMLIBankReportCriteriaCustomerNumbers customerNumbersField;

        private iXMLIBankReportCriteriaBreak break1Field;

        private iXMLIBankReportCriteriaBreak break2Field;

        private iXMLIBankReportCriteriaBreak break3Field;

        private iXMLIBankReportCriteriaValidatingCarrier validatingCarrierField;

        private iXMLIBankReportCriteriaOrigination originationField;

        private iXMLIBankReportCriteriaDestination destinationField;

        private iXMLIBankReportCriteriaRoutingCountry originCountryField;

        private iXMLIBankReportCriteriaRoutingCountry destinationCountryField;

        private iXMLIBankReportCriteriaRoutingRegion originRegionField;

        private iXMLIBankReportCriteriaRoutingRegion destinationRegionField;

        private object ticketNumField;

        private object recordLocatorField;

        private object invoiceNumField;

        private object passLastNameField;

        private object passFirstNameField;

        private object creditCardNumField;

        private object pseudocityField;

        private object branchField;

        private object agentIDField;

        private object uDIDNumField;

        private object uDIDTextField;

        private object domesticInternationalTransborderField;

        private iXMLIBankReportCriteriaAdvancedCriteria advancedCriteriaField;

        /// <remarks/>
        public string ReportName
        {
            get
            {
                return reportNameField;
            }
            set
            {
                reportNameField = value;
            }
        }

        /// <remarks/>
        public string DataType
        {
            get
            {
                return dataTypeField;
            }
            set
            {
                dataTypeField = value;
            }
        }

        /// <remarks/>
        public string OutputTo
        {
            get
            {
                return outputToField;
            }
            set
            {
                outputToField = value;
            }
        }

        /// <remarks/>
        public string Format
        {
            get
            {
                return formatField;
            }
            set
            {
                formatField = value;
            }
        }

        /// <remarks/>
        public string DateRangeType
        {
            get
            {
                return dateRangeTypeField;
            }
            set
            {
                dateRangeTypeField = value;
            }
        }

        /// <remarks/>
        public string DateFrom
        {
            get
            {
                return dateFromField;
            }
            set
            {
                dateFromField = value;
            }
        }

        /// <remarks/>
        public string DateTo
        {
            get
            {
                return dateToField;
            }
            set
            {
                dateToField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaParentAccounts ParentAccounts
        {
            get
            {
                return parentAccountsField;
            }
            set
            {
                parentAccountsField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaCustomerNumbers CustomerNumbers
        {
            get
            {
                return customerNumbersField;
            }
            set
            {
                customerNumbersField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaBreak Break1
        {
            get
            {
                return break1Field;
            }
            set
            {
                break1Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaBreak Break2
        {
            get
            {
                return break2Field;
            }
            set
            {
                break2Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaBreak Break3
        {
            get
            {
                return break3Field;
            }
            set
            {
                break3Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaValidatingCarrier ValidatingCarrier
        {
            get
            {
                return validatingCarrierField;
            }
            set
            {
                validatingCarrierField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaOrigination Origination
        {
            get
            {
                return originationField;
            }
            set
            {
                originationField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaDestination Destination
        {
            get
            {
                return destinationField;
            }
            set
            {
                destinationField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaRoutingCountry OriginCountry
        {
            get
            {
                return originCountryField;
            }
            set
            {
                originCountryField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaRoutingCountry DestinationCountry
        {
            get
            {
                return destinationCountryField;
            }
            set
            {
                destinationCountryField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaRoutingRegion OriginRegion
        {
            get
            {
                return originRegionField;
            }
            set
            {
                originRegionField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaRoutingRegion DestinationRegion
        {
            get
            {
                return destinationRegionField;
            }
            set
            {
                destinationRegionField = value;
            }
        }

        /// <remarks/>
        public object TicketNum
        {
            get
            {
                return ticketNumField;
            }
            set
            {
                ticketNumField = value;
            }
        }

        /// <remarks/>
        public object RecordLocator
        {
            get
            {
                return recordLocatorField;
            }
            set
            {
                recordLocatorField = value;
            }
        }

        /// <remarks/>
        public object InvoiceNum
        {
            get
            {
                return invoiceNumField;
            }
            set
            {
                invoiceNumField = value;
            }
        }

        /// <remarks/>
        public object PassLastName
        {
            get
            {
                return passLastNameField;
            }
            set
            {
                passLastNameField = value;
            }
        }

        /// <remarks/>
        public object PassFirstName
        {
            get
            {
                return passFirstNameField;
            }
            set
            {
                passFirstNameField = value;
            }
        }

        /// <remarks/>
        public object CreditCardNum
        {
            get
            {
                return creditCardNumField;
            }
            set
            {
                creditCardNumField = value;
            }
        }

        /// <remarks/>
        public object Pseudocity
        {
            get
            {
                return pseudocityField;
            }
            set
            {
                pseudocityField = value;
            }
        }

        /// <remarks/>
        public object Branch
        {
            get
            {
                return branchField;
            }
            set
            {
                branchField = value;
            }
        }

        /// <remarks/>
        public object AgentID
        {
            get
            {
                return agentIDField;
            }
            set
            {
                agentIDField = value;
            }
        }

        /// <remarks/>
        public object UDIDNum
        {
            get
            {
                return uDIDNumField;
            }
            set
            {
                uDIDNumField = value;
            }
        }

        /// <remarks/>
        public object UDIDText
        {
            get
            {
                return uDIDTextField;
            }
            set
            {
                uDIDTextField = value;
            }
        }

        /// <remarks/>
        public object DomesticInternationalTransborder
        {
            get
            {
                return domesticInternationalTransborderField;
            }
            set
            {
                domesticInternationalTransborderField = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteria AdvancedCriteria
        {
            get
            {
                return advancedCriteriaField;
            }
            set
            {
                advancedCriteriaField = value;
            }
        }
        
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaParentAccounts
    {

        private string accountsField;

        private string notInParentAccountField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Accounts
        {
            get
            {
                return accountsField;
            }
            set
            {
                accountsField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInParentAccount
        {
            get
            {
                return notInParentAccountField;
            }
            set
            {
                notInParentAccountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaCustomerNumbers
    {

        private string numbersField;

        private string notInCustomerNumbersField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Numbers
        {
            get
            {
                return numbersField;
            }
            set
            {
                numbersField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInCustomerNumbers
        {
            get
            {
                return notInCustomerNumbersField;
            }
            set
            {
                notInCustomerNumbersField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaBreak
    {

        private string inField;

        private string notInField;

        private string userNameField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string In
        {
            get
            {
                return inField;
            }
            set
            {
                inField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotIn
        {
            get
            {
                return notInField;
            }
            set
            {
                notInField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string UserName
        {
            get
            {
                return userNameField;
            }
            set
            {
                userNameField = value;
            }
        }
    }


    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaValidatingCarrier
    {

        private string carrierField;

        private string notInCarrierField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Carrier
        {
            get
            {
                return carrierField;
            }
            set
            {
                carrierField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInCarrier
        {
            get
            {
                return notInCarrierField;
            }
            set
            {
                notInCarrierField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaOrigination
    {

        private string originField;

        private string notInOriginField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Origin
        {
            get
            {
                return originField;
            }
            set
            {
                originField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInOrigin
        {
            get
            {
                return notInOriginField;
            }
            set
            {
                notInOriginField = value;
            }
        }

        [XmlIgnore]
        public bool IsFirst { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaDestination
    {

        private string destinField;

        private string notInDestinField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Destin
        {
            get
            {
                return destinField;
            }
            set
            {
                destinField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInDestin
        {
            get
            {
                return notInDestinField;
            }
            set
            {
                notInDestinField = value;
            }
        }

        [XmlIgnore]
        public bool IsFirst { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaRoutingCountry
    {

        private string countryField;

        private string notInCountryField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Country
        {
            get
            {
                return countryField;
            }
            set
            {
                countryField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInCountry
        {
            get
            {
                return notInCountryField;
            }
            set
            {
                notInCountryField = value;
            }
        }

        [XmlIgnore]
        public bool IsFirst { get; set; }
    }

   
    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaRoutingRegion
    {

        private string regionField;

        private string notInRegionField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Region
        {
            get
            {
                return regionField;
            }
            set
            {
                regionField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string NotInRegion
        {
            get
            {
                return notInRegionField;
            }
            set
            {
                notInRegionField = value;
            }
        }

        [XmlIgnore]
        public bool IsFirst { get; set; }
    }

   

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaAdvancedCriteria
    {

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria01Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria02Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria03Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria04Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria05Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria06Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria07Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria08Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria09Field;

        private iXMLIBankReportCriteriaAdvancedCriteriaCriteria criteria10Field;

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria01
        {
            get
            {
                return criteria01Field;
            }
            set
            {
                criteria01Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria02
        {
            get
            {
                return criteria02Field;
            }
            set
            {
                criteria02Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria03
        {
            get
            {
                return criteria03Field;
            }
            set
            {
                criteria03Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria04
        {
            get
            {
                return criteria04Field;
            }
            set
            {
                criteria04Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria05
        {
            get
            {
                return criteria05Field;
            }
            set
            {
                criteria05Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria06
        {
            get
            {
                return criteria06Field;
            }
            set
            {
                criteria06Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria07
        {
            get
            {
                return criteria07Field;
            }
            set
            {
                criteria07Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria08
        {
            get
            {
                return criteria08Field;
            }
            set
            {
                criteria08Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria09
        {
            get
            {
                return criteria09Field;
            }
            set
            {
                criteria09Field = value;
            }
        }

        /// <remarks/>
        public iXMLIBankReportCriteriaAdvancedCriteriaCriteria Criteria10
        {
            get
            {
                return criteria10Field;
            }
            set
            {
                criteria10Field = value;
            }
        }

        private string condtionsJoinedByField;
        /// <remarks/>
        [XmlIgnore]
        public string CondtionsJoinedBy
        {
            get
            {
                return condtionsJoinedByField;
            }
            set
            {
                condtionsJoinedByField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBankReportCriteriaAdvancedCriteriaCriteria
    {

        private string fieldNameField;

        private string criteriaField;

        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string FieldName
        {
            get
            {
                return fieldNameField;
            }
            set
            {
                fieldNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Criteria
        {
            get
            {
                return criteriaField;
            }
            set
            {
                criteriaField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItinerary
    {

        private iXMLIBank_TravelItineraryTravelItinerary travelItineraryField;

        private DateTime timeStampField;

        private string primaryLangIDField;

        private string versionField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItinerary TravelItinerary
        {
            get
            {
                return travelItineraryField;
            }
            set
            {
                travelItineraryField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public DateTime TimeStamp
        {
            get
            {
                return timeStampField;
            }
            set
            {
                timeStampField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string PrimaryLangID
        {
            get
            {
                return primaryLangIDField;
            }
            set
            {
                primaryLangIDField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Version
        {
            get
            {
                return versionField;
            }
            set
            {
                versionField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItinerary
    {

        private iXMLIBank_TravelItineraryTravelItineraryOrder orderField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryRef itineraryRefField;

        private iXMLIBank_TravelItineraryTravelItineraryTravelerInfo[] travelerInfoListField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfo itineraryInfoField;

        private iXMLIBank_TravelItineraryTravelItineraryUDIDS[] uDIDListField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryOrder Order
        {
            get
            {
                return orderField;
            }
            set
            {
                orderField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryRef ItineraryRef
        {
            get
            {
                return itineraryRefField;
            }
            set
            {
                itineraryRefField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItemAttribute("TravelerInfo", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryTravelerInfo[] TravelerInfoList
        {
            get
            {
                return travelerInfoListField;
            }
            set
            {
                travelerInfoListField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfo ItineraryInfo
        {
            get
            {
                return itineraryInfoField;
            }
            set
            {
                itineraryInfoField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItemAttribute("UDIDS", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryUDIDS[] UDIDList
        {
            get
            {
                return uDIDListField;
            }
            set
            {
                uDIDListField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryOrder
    {

        private string orderIdentifierField;

        private string entityField;

        private string gDSField;

        private string orderIdentifierContextField;

        private string currencyTypeField;

        private string sourceAbbreviationField;

        private string changeOrderReasonField;

        /// <remarks/>
        [XmlAttribute]
        public string OrderIdentifier
        {
            get
            {
                return orderIdentifierField;
            }
            set
            {
                orderIdentifierField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Entity
        {
            get
            {
                return entityField;
            }
            set
            {
                entityField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string GDS
        {
            get
            {
                return gDSField;
            }
            set
            {
                gDSField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string OrderIdentifierContext
        {
            get
            {
                return orderIdentifierContextField;
            }
            set
            {
                orderIdentifierContextField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string CurrencyType
        {
            get
            {
                return currencyTypeField;
            }
            set
            {
                currencyTypeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string SourceAbbreviation
        {
            get
            {
                return sourceAbbreviationField;
            }
            set
            {
                sourceAbbreviationField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string ChangeOrderReason
        {
            get
            {
                return changeOrderReasonField;
            }
            set
            {
                changeOrderReasonField = value;
            }
        }

       
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryRef
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryRefClientIdentifierContext clientIdentifierContextField;

        private uint creationDateField;

        private DateTime lastModifiedField;

        private string recordLocatorField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryRefClientIdentifierContext ClientIdentifierContext
        {
            get
            {
                return clientIdentifierContextField;
            }
            set
            {
                clientIdentifierContextField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public uint CreationDate
        {
            get
            {
                return creationDateField;
            }
            set
            {
                creationDateField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute(DataType = "date")]
        public DateTime LastModified
        {
            get
            {
                return lastModifiedField;
            }
            set
            {
                lastModifiedField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string RecordLocator
        {
            get
            {
                return recordLocatorField;
            }
            set
            {
                recordLocatorField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryRefClientIdentifierContext
    {

        private ushort agencyAccountNumberField;

        private string accountNameField;

        private string clientIdentifierField;

        /// <remarks/>
        public ushort AgencyAccountNumber
        {
            get
            {
                return agencyAccountNumberField;
            }
            set
            {
                agencyAccountNumberField = value;
            }
        }

        /// <remarks/>
        public string AccountName
        {
            get
            {
                return accountNameField;
            }
            set
            {
                accountNameField = value;
            }
        }

        /// <remarks/>
        public string ClientIdentifier
        {
            get
            {
                return clientIdentifierField;
            }
            set
            {
                clientIdentifierField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryTravelerInfo
    {

        private iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTraveler travelerField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTraveler Traveler
        {
            get
            {
                return travelerField;
            }
            set
            {
                travelerField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTraveler
    {

        private iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerTravelerName travelerNameField;

        private iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerEMail eMailField;

        private iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerTelephone telephoneField;

        private string ticketNumberField;

        private string break1ValueField;

        private string break2ValueField;

        private string break3ValueField;

        private iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerReportBreak[] reportBreaksField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerTravelerName TravelerName
        {
            get
            {
                return travelerNameField;
            }
            set
            {
                travelerNameField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerEMail EMail
        {
            get
            {
                return eMailField;
            }
            set
            {
                eMailField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerTelephone Telephone
        {
            get
            {
                return telephoneField;
            }
            set
            {
                telephoneField = value;
            }
        }

        /// <remarks/>
        public string TicketNumber
        {
            get
            {
                return ticketNumberField;
            }
            set
            {
                ticketNumberField = value;
            }
        }

        /// <remarks/>
        public string Break1Value
        {
            get
            {
                return break1ValueField;
            }
            set
            {
                break1ValueField = value;
            }
        }

        /// <remarks/>
        public string Break2Value
        {
            get
            {
                return break2ValueField;
            }
            set
            {
                break2ValueField = value;
            }
        }

        /// <remarks/>
        public string Break3Value
        {
            get
            {
                return break3ValueField;
            }
            set
            {
                break3ValueField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItemAttribute("ReportBreak", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerReportBreak[] ReportBreaks
        {
            get
            {
                return reportBreaksField;
            }
            set
            {
                reportBreaksField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerTravelerName
    {

        private string firstNameField;

        private string lastNameField;

        /// <remarks/>
        public string FirstName
        {
            get
            {
                return firstNameField;
            }
            set
            {
                firstNameField = value;
            }
        }

        /// <remarks/>
        public string LastName
        {
            get
            {
                return lastNameField;
            }
            set
            {
                lastNameField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerEMail
    {

        private string emailAddressField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string EmailAddress
        {
            get
            {
                return emailAddressField;
            }
            set
            {
                emailAddressField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerTelephone
    {

        private string phoneNumberField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string PhoneNumber
        {
            get
            {
                return phoneNumberField;
            }
            set
            {
                phoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryTravelerInfoTravelerReportBreak
    {

        private string breakKeyField;

        private string breakValueField;

        private byte breakNumberField;

        /// <remarks/>
        public string BreakKey
        {
            get
            {
                return breakKeyField;
            }
            set
            {
                breakKeyField = value;
            }
        }

        /// <remarks/>
        public string BreakValue
        {
            get
            {
                return breakValueField;
            }
            set
            {
                breakValueField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public byte BreakNumber
        {
            get
            {
                return breakNumberField;
            }
            set
            {
                breakNumberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfo
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItems reservationItemsField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItems ReservationItems
        {
            get
            {
                return reservationItemsField;
            }
            set
            {
                reservationItemsField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItems
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetails airDetailsField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetails airRailDetailsField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetails rentDetailsField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetails stayDetailsField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetails AirDetails
        {
            get
            {
                return airDetailsField;
            }
            set
            {
                airDetailsField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetails AirRailDetails
        {
            get
            {
                return airRailDetailsField;
            }
            set
            {
                airRailDetailsField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetails RentDetails
        {
            get
            {
                return rentDetailsField;
            }
            set
            {
                rentDetailsField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetails StayDetails
        {
            get
            {
                return stayDetailsField;
            }
            set
            {
                stayDetailsField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetails
    {

        private object airComponentListField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFare storedAirFareField;

        private decimal airChargeAmountField;

        private decimal offeredFareField;

        private decimal standardChargeField;

        private decimal commissionAmountField;

        private string agentIDField;

        private decimal fareTaxField;

        private DateTime lastUpdateField;

        private object savingCodeField;

        private string exchangeField;

        private string sourceAbbreviationField;

        private string corporateAccountField;

        private string bookToolField;

        private object agentContactField;

        private object changedByField;

        private DateTime changeTimeStampField;

        private DateTime parseTimeStampField;

        private DateTime tripDepartureDateField;

        private DateTime tripArrivalDateField;

        private string ticketingAgentField;

        private string bookingAgentField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsInvoice invoiceField;

        /// <remarks/>
        public object AirComponentList
        {
            get
            {
                return airComponentListField;
            }
            set
            {
                airComponentListField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFare StoredAirFare
        {
            get
            {
                return storedAirFareField;
            }
            set
            {
                storedAirFareField = value;
            }
        }

        /// <remarks/>
        public decimal AirChargeAmount
        {
            get
            {
                return airChargeAmountField;
            }
            set
            {
                airChargeAmountField = value;
            }
        }

        /// <remarks/>
        public decimal OfferedFare
        {
            get
            {
                return offeredFareField;
            }
            set
            {
                offeredFareField = value;
            }
        }

        /// <remarks/>
        public decimal StandardCharge
        {
            get
            {
                return standardChargeField;
            }
            set
            {
                standardChargeField = value;
            }
        }

        /// <remarks/>
        public decimal CommissionAmount
        {
            get
            {
                return commissionAmountField;
            }
            set
            {
                commissionAmountField = value;
            }
        }

        /// <remarks/>
        public string AgentID
        {
            get
            {
                return agentIDField;
            }
            set
            {
                agentIDField = value;
            }
        }

        /// <remarks/>
        public decimal FareTax
        {
            get
            {
                return fareTaxField;
            }
            set
            {
                fareTaxField = value;
            }
        }

        /// <remarks/>
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdateField;
            }
            set
            {
                lastUpdateField = value;
            }
        }

        /// <remarks/>
        public object SavingCode
        {
            get
            {
                return savingCodeField;
            }
            set
            {
                savingCodeField = value;
            }
        }

        /// <remarks/>
        public string Exchange
        {
            get
            {
                return exchangeField;
            }
            set
            {
                exchangeField = value;
            }
        }

        /// <remarks/>
        public string SourceAbbreviation
        {
            get
            {
                return sourceAbbreviationField;
            }
            set
            {
                sourceAbbreviationField = value;
            }
        }

        /// <remarks/>
        public string CorporateAccount
        {
            get
            {
                return corporateAccountField;
            }
            set
            {
                corporateAccountField = value;
            }
        }

        /// <remarks/>
        public string BookTool
        {
            get
            {
                return bookToolField;
            }
            set
            {
                bookToolField = value;
            }
        }

        /// <remarks/>
        public object AgentContact
        {
            get
            {
                return agentContactField;
            }
            set
            {
                agentContactField = value;
            }
        }

        /// <remarks/>
        public object ChangedBy
        {
            get
            {
                return changedByField;
            }
            set
            {
                changedByField = value;
            }
        }

        /// <remarks/>
        public DateTime ChangeTimeStamp
        {
            get
            {
                return changeTimeStampField;
            }
            set
            {
                changeTimeStampField = value;
            }
        }

        /// <remarks/>
        public DateTime ParseTimeStamp
        {
            get
            {
                return parseTimeStampField;
            }
            set
            {
                parseTimeStampField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime TripDepartureDate
        {
            get
            {
                return tripDepartureDateField;
            }
            set
            {
                tripDepartureDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime TripArrivalDate
        {
            get
            {
                return tripArrivalDateField;
            }
            set
            {
                tripArrivalDateField = value;
            }
        }

        /// <remarks/>
        public string TicketingAgent
        {
            get
            {
                return ticketingAgentField;
            }
            set
            {
                ticketingAgentField = value;
            }
        }

        /// <remarks/>
        public string BookingAgent
        {
            get
            {
                return bookingAgentField;
            }
            set
            {
                bookingAgentField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsInvoice Invoice
        {
            get
            {
                return invoiceField;
            }
            set
            {
                invoiceField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFare
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFareTravelTaxList travelTaxListField;

        private object reasonCodeField;

        private object reasonCodeDescField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFarePaymentInformation paymentInformationField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFareTravelTaxList TravelTaxList
        {
            get
            {
                return travelTaxListField;
            }
            set
            {
                travelTaxListField = value;
            }
        }

        /// <remarks/>
        public object ReasonCode
        {
            get
            {
                return reasonCodeField;
            }
            set
            {
                reasonCodeField = value;
            }
        }

        /// <remarks/>
        public object ReasonCodeDesc
        {
            get
            {
                return reasonCodeDescField;
            }
            set
            {
                reasonCodeDescField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFarePaymentInformation PaymentInformation
        {
            get
            {
                return paymentInformationField;
            }
            set
            {
                paymentInformationField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFareTravelTaxList
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFareTravelTaxListTravelTax[] travelTaxField;

        private decimal totalTaxAmountField;

        /// <remarks/>
        [XmlElementAttribute("TravelTax")]
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFareTravelTaxListTravelTax[] TravelTax
        {
            get
            {
                return travelTaxField;
            }
            set
            {
                travelTaxField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public decimal TotalTaxAmount
        {
            get
            {
                return totalTaxAmountField;
            }
            set
            {
                totalTaxAmountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFareTravelTaxListTravelTax
    {

        private decimal travelTaxAmountField;

        private object taxCodeField;

        /// <remarks/>
        public decimal TravelTaxAmount
        {
            get
            {
                return travelTaxAmountField;
            }
            set
            {
                travelTaxAmountField = value;
            }
        }

        /// <remarks/>
        public object TaxCode
        {
            get
            {
                return taxCodeField;
            }
            set
            {
                taxCodeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFarePaymentInformation
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFarePaymentInformationFormOfPayment formOfPaymentField;

        private decimal paymentAmountField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFarePaymentInformationFormOfPayment FormOfPayment
        {
            get
            {
                return formOfPaymentField;
            }
            set
            {
                formOfPaymentField = value;
            }
        }

        /// <remarks/>
        public decimal PaymentAmount
        {
            get
            {
                return paymentAmountField;
            }
            set
            {
                paymentAmountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsStoredAirFarePaymentInformationFormOfPayment
    {

        private string cardNumberField;

        private string cardTypeField;

        /// <remarks/>
        public string CardNumber
        {
            get
            {
                return cardNumberField;
            }
            set
            {
                cardNumberField = value;
            }
        }

        /// <remarks/>
        public string CardType
        {
            get
            {
                return cardTypeField;
            }
            set
            {
                cardTypeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirDetailsInvoice
    {

        private string ticketTypeField;

        private object ticketNumberField;

        private string validatingCarrierField;

        private uint invoiceNumberField;

        private DateTime invoiceGMTTimestampField;

        private uint iATANumberField;

        private object origTicketNumberField;

        private string invoiceTypeField;

        /// <remarks/>
        public string TicketType
        {
            get
            {
                return ticketTypeField;
            }
            set
            {
                ticketTypeField = value;
            }
        }

        /// <remarks/>
        public object TicketNumber
        {
            get
            {
                return ticketNumberField;
            }
            set
            {
                ticketNumberField = value;
            }
        }

        /// <remarks/>
        public string ValidatingCarrier
        {
            get
            {
                return validatingCarrierField;
            }
            set
            {
                validatingCarrierField = value;
            }
        }

        /// <remarks/>
        public uint InvoiceNumber
        {
            get
            {
                return invoiceNumberField;
            }
            set
            {
                invoiceNumberField = value;
            }
        }

        /// <remarks/>
        public DateTime InvoiceGMTTimestamp
        {
            get
            {
                return invoiceGMTTimestampField;
            }
            set
            {
                invoiceGMTTimestampField = value;
            }
        }

        /// <remarks/>
        public uint IATANumber
        {
            get
            {
                return iATANumberField;
            }
            set
            {
                iATANumberField = value;
            }
        }

        /// <remarks/>
        public object OrigTicketNumber
        {
            get
            {
                return origTicketNumberField;
            }
            set
            {
                origTicketNumberField = value;
            }
        }

        /// <remarks/>
        public string InvoiceType
        {
            get
            {
                return invoiceTypeField;
            }
            set
            {
                invoiceTypeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetails
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetails airDetailsField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetails railDetailsField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetails AirDetails
        {
            get
            {
                return airDetailsField;
            }
            set
            {
                airDetailsField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetails RailDetails
        {
            get
            {
                return railDetailsField;
            }
            set
            {
                railDetailsField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetails
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponent[] airComponentListField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFare storedAirFareField;

        private decimal airChargeAmountField;

        private decimal offeredFareField;

        private decimal standardChargeField;

        private decimal commissionAmountField;

        private string agentIDField;

        private decimal fareTaxField;

        private DateTime lastUpdateField;

        private string savingCodeField;

        private string exchangeField;

        private string sourceAbbreviationField;

        private string corporateAccountField;

        private string bookToolField;

        private object agentContactField;

        private object changedByField;

        private DateTime changeTimeStampField;

        private DateTime parseTimeStampField;

        private DateTime tripDepartureDateField;

        private DateTime tripArrivalDateField;

        private string ticketingAgentField;

        private string bookingAgentField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsInvoice invoiceField;

        /// <remarks/>
        [XmlArrayItemAttribute("AirComponent", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponent[] AirComponentList
        {
            get
            {
                return airComponentListField;
            }
            set
            {
                airComponentListField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFare StoredAirFare
        {
            get
            {
                return storedAirFareField;
            }
            set
            {
                storedAirFareField = value;
            }
        }

        /// <remarks/>
        public decimal AirChargeAmount
        {
            get
            {
                return airChargeAmountField;
            }
            set
            {
                airChargeAmountField = value;
            }
        }

        /// <remarks/>
        public decimal OfferedFare
        {
            get
            {
                return offeredFareField;
            }
            set
            {
                offeredFareField = value;
            }
        }

        /// <remarks/>
        public decimal StandardCharge
        {
            get
            {
                return standardChargeField;
            }
            set
            {
                standardChargeField = value;
            }
        }

        /// <remarks/>
        public decimal CommissionAmount
        {
            get
            {
                return commissionAmountField;
            }
            set
            {
                commissionAmountField = value;
            }
        }

        /// <remarks/>
        public string AgentID
        {
            get
            {
                return agentIDField;
            }
            set
            {
                agentIDField = value;
            }
        }

        /// <remarks/>
        public decimal FareTax
        {
            get
            {
                return fareTaxField;
            }
            set
            {
                fareTaxField = value;
            }
        }

        /// <remarks/>
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdateField;
            }
            set
            {
                lastUpdateField = value;
            }
        }

        /// <remarks/>
        public string SavingCode
        {
            get
            {
                return savingCodeField;
            }
            set
            {
                savingCodeField = value;
            }
        }

        /// <remarks/>
        public string Exchange
        {
            get
            {
                return exchangeField;
            }
            set
            {
                exchangeField = value;
            }
        }

        /// <remarks/>
        public string SourceAbbreviation
        {
            get
            {
                return sourceAbbreviationField;
            }
            set
            {
                sourceAbbreviationField = value;
            }
        }

        /// <remarks/>
        public string CorporateAccount
        {
            get
            {
                return corporateAccountField;
            }
            set
            {
                corporateAccountField = value;
            }
        }

        /// <remarks/>
        public string BookTool
        {
            get
            {
                return bookToolField;
            }
            set
            {
                bookToolField = value;
            }
        }

        /// <remarks/>
        public object AgentContact
        {
            get
            {
                return agentContactField;
            }
            set
            {
                agentContactField = value;
            }
        }

        /// <remarks/>
        public object ChangedBy
        {
            get
            {
                return changedByField;
            }
            set
            {
                changedByField = value;
            }
        }

        /// <remarks/>
        public DateTime ChangeTimeStamp
        {
            get
            {
                return changeTimeStampField;
            }
            set
            {
                changeTimeStampField = value;
            }
        }

        /// <remarks/>
        public DateTime ParseTimeStamp
        {
            get
            {
                return parseTimeStampField;
            }
            set
            {
                parseTimeStampField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime TripDepartureDate
        {
            get
            {
                return tripDepartureDateField;
            }
            set
            {
                tripDepartureDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime TripArrivalDate
        {
            get
            {
                return tripArrivalDateField;
            }
            set
            {
                tripArrivalDateField = value;
            }
        }

        /// <remarks/>
        public string TicketingAgent
        {
            get
            {
                return ticketingAgentField;
            }
            set
            {
                ticketingAgentField = value;
            }
        }

        /// <remarks/>
        public string BookingAgent
        {
            get
            {
                return bookingAgentField;
            }
            set
            {
                bookingAgentField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsInvoice Invoice
        {
            get
            {
                return invoiceField;
            }
            set
            {
                invoiceField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponent
    {

        private string recordLocatorField;

        private string bookingPseudoCityField;

        private byte branchField;

        private string connectionFlagField;

        private string departureAirportField;

        private string departureCityField;

        private string arrivalAirportField;

        private string arrivalCityField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentMarketingAirline marketingAirlineField;

        private ushort flightNumberField;

        private DateTime departureDateField;

        private DateTime arrivalDateField;

        private DateTime departureTimeField;

        private DateTime arrivalTimeField;

        private string classCategoryField;

        private string bookingClassField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentSeats seatsField;

        private object segmentStatusField;

        private string fareBasisField;

        private object numberOfStopsField;

        private decimal baseFareAmountField;

        private decimal surchargeAmountField;

        private ushort milesField;

        private string tourCodeField;

        private string ticketDesignatorField;

        private ushort accountNumberField;

        private string dITCodeField;

        private byte sequenceNumberField;

        private string supplierServiceNameField;

        private string typeField;

        /// <remarks/>
        public string RecordLocator
        {
            get
            {
                return recordLocatorField;
            }
            set
            {
                recordLocatorField = value;
            }
        }

        /// <remarks/>
        public string BookingPseudoCity
        {
            get
            {
                return bookingPseudoCityField;
            }
            set
            {
                bookingPseudoCityField = value;
            }
        }

        /// <remarks/>
        public byte Branch
        {
            get
            {
                return branchField;
            }
            set
            {
                branchField = value;
            }
        }

        /// <remarks/>
        public string ConnectionFlag
        {
            get
            {
                return connectionFlagField;
            }
            set
            {
                connectionFlagField = value;
            }
        }

        /// <remarks/>
        public string DepartureAirport
        {
            get
            {
                return departureAirportField;
            }
            set
            {
                departureAirportField = value;
            }
        }

        /// <remarks/>
        public string DepartureCity
        {
            get
            {
                return departureCityField;
            }
            set
            {
                departureCityField = value;
            }
        }

        /// <remarks/>
        public string ArrivalAirport
        {
            get
            {
                return arrivalAirportField;
            }
            set
            {
                arrivalAirportField = value;
            }
        }

        /// <remarks/>
        public string ArrivalCity
        {
            get
            {
                return arrivalCityField;
            }
            set
            {
                arrivalCityField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentMarketingAirline MarketingAirline
        {
            get
            {
                return marketingAirlineField;
            }
            set
            {
                marketingAirlineField = value;
            }
        }

        /// <remarks/>
        public ushort FlightNumber
        {
            get
            {
                return flightNumberField;
            }
            set
            {
                flightNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime DepartureDate
        {
            get
            {
                return departureDateField;
            }
            set
            {
                departureDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime ArrivalDate
        {
            get
            {
                return arrivalDateField;
            }
            set
            {
                arrivalDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "time")]
        public DateTime DepartureTime
        {
            get
            {
                return departureTimeField;
            }
            set
            {
                departureTimeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "time")]
        public DateTime ArrivalTime
        {
            get
            {
                return arrivalTimeField;
            }
            set
            {
                arrivalTimeField = value;
            }
        }

        /// <remarks/>
        public string ClassCategory
        {
            get
            {
                return classCategoryField;
            }
            set
            {
                classCategoryField = value;
            }
        }

        /// <remarks/>
        public string BookingClass
        {
            get
            {
                return bookingClassField;
            }
            set
            {
                bookingClassField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentSeats Seats
        {
            get
            {
                return seatsField;
            }
            set
            {
                seatsField = value;
            }
        }

        /// <remarks/>
        public object SegmentStatus
        {
            get
            {
                return segmentStatusField;
            }
            set
            {
                segmentStatusField = value;
            }
        }

        /// <remarks/>
        public string FareBasis
        {
            get
            {
                return fareBasisField;
            }
            set
            {
                fareBasisField = value;
            }
        }

        /// <remarks/>
        public object NumberOfStops
        {
            get
            {
                return numberOfStopsField;
            }
            set
            {
                numberOfStopsField = value;
            }
        }

        /// <remarks/>
        public decimal BaseFareAmount
        {
            get
            {
                return baseFareAmountField;
            }
            set
            {
                baseFareAmountField = value;
            }
        }

        /// <remarks/>
        public decimal SurchargeAmount
        {
            get
            {
                return surchargeAmountField;
            }
            set
            {
                surchargeAmountField = value;
            }
        }

        /// <remarks/>
        public ushort Miles
        {
            get
            {
                return milesField;
            }
            set
            {
                milesField = value;
            }
        }

        /// <remarks/>
        public string TourCode
        {
            get
            {
                return tourCodeField;
            }
            set
            {
                tourCodeField = value;
            }
        }

        /// <remarks/>
        public string TicketDesignator
        {
            get
            {
                return ticketDesignatorField;
            }
            set
            {
                ticketDesignatorField = value;
            }
        }

        /// <remarks/>
        public ushort AccountNumber
        {
            get
            {
                return accountNumberField;
            }
            set
            {
                accountNumberField = value;
            }
        }

        /// <remarks/>
        public string DITCode
        {
            get
            {
                return dITCodeField;
            }
            set
            {
                dITCodeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public byte SequenceNumber
        {
            get
            {
                return sequenceNumberField;
            }
            set
            {
                sequenceNumberField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string SupplierServiceName
        {
            get
            {
                return supplierServiceNameField;
            }
            set
            {
                supplierServiceNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentMarketingAirline
    {

        private string codeField;

        private string nameField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentSeats
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentSeatsSeat seatField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentSeatsSeat Seat
        {
            get
            {
                return seatField;
            }
            set
            {
                seatField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsAirComponentSeatsSeat
    {

        private byte customerSeqnoField;

        private string characteristicField;

        private string numberField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public byte CustomerSeqno
        {
            get
            {
                return customerSeqnoField;
            }
            set
            {
                customerSeqnoField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Characteristic
        {
            get
            {
                return characteristicField;
            }
            set
            {
                characteristicField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Number
        {
            get
            {
                return numberField;
            }
            set
            {
                numberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFare
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFareTravelTaxList travelTaxListField;

        private string reasonCodeField;

        private string reasonCodeDescField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFarePaymentInformation paymentInformationField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFareTravelTaxList TravelTaxList
        {
            get
            {
                return travelTaxListField;
            }
            set
            {
                travelTaxListField = value;
            }
        }

        /// <remarks/>
        public string ReasonCode
        {
            get
            {
                return reasonCodeField;
            }
            set
            {
                reasonCodeField = value;
            }
        }

        /// <remarks/>
        public string ReasonCodeDesc
        {
            get
            {
                return reasonCodeDescField;
            }
            set
            {
                reasonCodeDescField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFarePaymentInformation PaymentInformation
        {
            get
            {
                return paymentInformationField;
            }
            set
            {
                paymentInformationField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFareTravelTaxList
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFareTravelTaxListTravelTax[] travelTaxField;

        private decimal totalTaxAmountField;

        /// <remarks/>
        [XmlElementAttribute("TravelTax")]
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFareTravelTaxListTravelTax[] TravelTax
        {
            get
            {
                return travelTaxField;
            }
            set
            {
                travelTaxField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public decimal TotalTaxAmount
        {
            get
            {
                return totalTaxAmountField;
            }
            set
            {
                totalTaxAmountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFareTravelTaxListTravelTax
    {

        private decimal travelTaxAmountField;

        private object taxCodeField;

        /// <remarks/>
        public decimal TravelTaxAmount
        {
            get
            {
                return travelTaxAmountField;
            }
            set
            {
                travelTaxAmountField = value;
            }
        }

        /// <remarks/>
        public object TaxCode
        {
            get
            {
                return taxCodeField;
            }
            set
            {
                taxCodeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFarePaymentInformation
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFarePaymentInformationFormOfPayment formOfPaymentField;

        private decimal paymentAmountField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFarePaymentInformationFormOfPayment FormOfPayment
        {
            get
            {
                return formOfPaymentField;
            }
            set
            {
                formOfPaymentField = value;
            }
        }

        /// <remarks/>
        public decimal PaymentAmount
        {
            get
            {
                return paymentAmountField;
            }
            set
            {
                paymentAmountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsStoredAirFarePaymentInformationFormOfPayment
    {

        private string cardNumberField;

        private string cardTypeField;

        /// <remarks/>
        public string CardNumber
        {
            get
            {
                return cardNumberField;
            }
            set
            {
                cardNumberField = value;
            }
        }

        /// <remarks/>
        public string CardType
        {
            get
            {
                return cardTypeField;
            }
            set
            {
                cardTypeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsAirDetailsInvoice
    {

        private string ticketTypeField;

        private string ticketNumberField;

        private string validatingCarrierField;

        private uint invoiceNumberField;

        private DateTime invoiceGMTTimestampField;

        private uint iATANumberField;

        private string origTicketNumberField;

        private string invoiceTypeField;

        /// <remarks/>
        public string TicketType
        {
            get
            {
                return ticketTypeField;
            }
            set
            {
                ticketTypeField = value;
            }
        }

        /// <remarks/>
        public string TicketNumber
        {
            get
            {
                return ticketNumberField;
            }
            set
            {
                ticketNumberField = value;
            }
        }

        /// <remarks/>
        public string ValidatingCarrier
        {
            get
            {
                return validatingCarrierField;
            }
            set
            {
                validatingCarrierField = value;
            }
        }

        /// <remarks/>
        public uint InvoiceNumber
        {
            get
            {
                return invoiceNumberField;
            }
            set
            {
                invoiceNumberField = value;
            }
        }

        /// <remarks/>
        public DateTime InvoiceGMTTimestamp
        {
            get
            {
                return invoiceGMTTimestampField;
            }
            set
            {
                invoiceGMTTimestampField = value;
            }
        }

        /// <remarks/>
        public uint IATANumber
        {
            get
            {
                return iATANumberField;
            }
            set
            {
                iATANumberField = value;
            }
        }

        /// <remarks/>
        public string OrigTicketNumber
        {
            get
            {
                return origTicketNumberField;
            }
            set
            {
                origTicketNumberField = value;
            }
        }

        /// <remarks/>
        public string InvoiceType
        {
            get
            {
                return invoiceTypeField;
            }
            set
            {
                invoiceTypeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetails
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponent[] railComponentListField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFare storedRailFareField;

        private decimal railChargeAmountField;

        private decimal offeredFareField;

        private decimal standardChargeField;

        private decimal commissionAmountField;

        private string agentIDField;

        private decimal fareTaxField;

        private DateTime lastUpdateField;

        private string savingCodeField;

        private string exchangeField;

        private string sourceAbbreviationField;

        private string corporateAccountField;

        private string bookToolField;

        private object agentContactField;

        private object changedByField;

        private DateTime changeTimeStampField;

        private DateTime parseTimeStampField;

        private DateTime tripDepartureDateField;

        private DateTime tripArrivalDateField;

        private string ticketingAgentField;

        private string bookingAgentField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsInvoice invoiceField;

        /// <remarks/>
        [XmlArrayItemAttribute("RailComponent", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponent[] RailComponentList
        {
            get
            {
                return railComponentListField;
            }
            set
            {
                railComponentListField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFare StoredRailFare
        {
            get
            {
                return storedRailFareField;
            }
            set
            {
                storedRailFareField = value;
            }
        }

        /// <remarks/>
        public decimal RailChargeAmount
        {
            get
            {
                return railChargeAmountField;
            }
            set
            {
                railChargeAmountField = value;
            }
        }

        /// <remarks/>
        public decimal OfferedFare
        {
            get
            {
                return offeredFareField;
            }
            set
            {
                offeredFareField = value;
            }
        }

        /// <remarks/>
        public decimal StandardCharge
        {
            get
            {
                return standardChargeField;
            }
            set
            {
                standardChargeField = value;
            }
        }

        /// <remarks/>
        public decimal CommissionAmount
        {
            get
            {
                return commissionAmountField;
            }
            set
            {
                commissionAmountField = value;
            }
        }

        /// <remarks/>
        public string AgentID
        {
            get
            {
                return agentIDField;
            }
            set
            {
                agentIDField = value;
            }
        }

        /// <remarks/>
        public decimal FareTax
        {
            get
            {
                return fareTaxField;
            }
            set
            {
                fareTaxField = value;
            }
        }

        /// <remarks/>
        public DateTime LastUpdate
        {
            get
            {
                return lastUpdateField;
            }
            set
            {
                lastUpdateField = value;
            }
        }

        /// <remarks/>
        public string SavingCode
        {
            get
            {
                return savingCodeField;
            }
            set
            {
                savingCodeField = value;
            }
        }

        /// <remarks/>
        public string Exchange
        {
            get
            {
                return exchangeField;
            }
            set
            {
                exchangeField = value;
            }
        }

        /// <remarks/>
        public string SourceAbbreviation
        {
            get
            {
                return sourceAbbreviationField;
            }
            set
            {
                sourceAbbreviationField = value;
            }
        }

        /// <remarks/>
        public string CorporateAccount
        {
            get
            {
                return corporateAccountField;
            }
            set
            {
                corporateAccountField = value;
            }
        }

        /// <remarks/>
        public string BookTool
        {
            get
            {
                return bookToolField;
            }
            set
            {
                bookToolField = value;
            }
        }

        /// <remarks/>
        public object AgentContact
        {
            get
            {
                return agentContactField;
            }
            set
            {
                agentContactField = value;
            }
        }

        /// <remarks/>
        public object ChangedBy
        {
            get
            {
                return changedByField;
            }
            set
            {
                changedByField = value;
            }
        }

        /// <remarks/>
        public DateTime ChangeTimeStamp
        {
            get
            {
                return changeTimeStampField;
            }
            set
            {
                changeTimeStampField = value;
            }
        }

        /// <remarks/>
        public DateTime ParseTimeStamp
        {
            get
            {
                return parseTimeStampField;
            }
            set
            {
                parseTimeStampField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime TripDepartureDate
        {
            get
            {
                return tripDepartureDateField;
            }
            set
            {
                tripDepartureDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime TripArrivalDate
        {
            get
            {
                return tripArrivalDateField;
            }
            set
            {
                tripArrivalDateField = value;
            }
        }

        /// <remarks/>
        public string TicketingAgent
        {
            get
            {
                return ticketingAgentField;
            }
            set
            {
                ticketingAgentField = value;
            }
        }

        /// <remarks/>
        public string BookingAgent
        {
            get
            {
                return bookingAgentField;
            }
            set
            {
                bookingAgentField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsInvoice Invoice
        {
            get
            {
                return invoiceField;
            }
            set
            {
                invoiceField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponent
    {

        private string recordLocatorField;

        private string bookingPseudoCityField;

        private byte branchField;

        private string departureStationField;

        private string departureCityField;

        private string arrivalStationField;

        private string arrivalCityField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponentRailCode railCodeField;

        private ushort railNumberField;

        private DateTime departureDateField;

        private DateTime arrivalDateField;

        private DateTime departureTimeField;

        private DateTime arrivalTimeField;

        private string connectionFlagField;

        private object classCategoryField;

        private string bookingClassField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponentSeats seatsField;

        private object segmentStatusField;

        private string fareBasisField;

        private object numberOfStopsField;

        private decimal baseFareAmountField;

        private decimal surchargeAmountField;

        private byte milesField;

        private object tourCodeField;

        private ushort accountNumberField;

        private string dITCodeField;

        private byte sequenceNumberField;

        private string supplierServiceNameField;

        private string typeField;

        /// <remarks/>
        public string RecordLocator
        {
            get
            {
                return recordLocatorField;
            }
            set
            {
                recordLocatorField = value;
            }
        }

        /// <remarks/>
        public string BookingPseudoCity
        {
            get
            {
                return bookingPseudoCityField;
            }
            set
            {
                bookingPseudoCityField = value;
            }
        }

        /// <remarks/>
        public byte Branch
        {
            get
            {
                return branchField;
            }
            set
            {
                branchField = value;
            }
        }

        /// <remarks/>
        public string DepartureStation
        {
            get
            {
                return departureStationField;
            }
            set
            {
                departureStationField = value;
            }
        }

        /// <remarks/>
        public string DepartureCity
        {
            get
            {
                return departureCityField;
            }
            set
            {
                departureCityField = value;
            }
        }

        /// <remarks/>
        public string ArrivalStation
        {
            get
            {
                return arrivalStationField;
            }
            set
            {
                arrivalStationField = value;
            }
        }

        /// <remarks/>
        public string ArrivalCity
        {
            get
            {
                return arrivalCityField;
            }
            set
            {
                arrivalCityField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponentRailCode RailCode
        {
            get
            {
                return railCodeField;
            }
            set
            {
                railCodeField = value;
            }
        }

        /// <remarks/>
        public ushort RailNumber
        {
            get
            {
                return railNumberField;
            }
            set
            {
                railNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime DepartureDate
        {
            get
            {
                return departureDateField;
            }
            set
            {
                departureDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime ArrivalDate
        {
            get
            {
                return arrivalDateField;
            }
            set
            {
                arrivalDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "time")]
        public DateTime DepartureTime
        {
            get
            {
                return departureTimeField;
            }
            set
            {
                departureTimeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "time")]
        public DateTime ArrivalTime
        {
            get
            {
                return arrivalTimeField;
            }
            set
            {
                arrivalTimeField = value;
            }
        }

        /// <remarks/>
        public string ConnectionFlag
        {
            get
            {
                return connectionFlagField;
            }
            set
            {
                connectionFlagField = value;
            }
        }

        /// <remarks/>
        public object ClassCategory
        {
            get
            {
                return classCategoryField;
            }
            set
            {
                classCategoryField = value;
            }
        }

        /// <remarks/>
        public string BookingClass
        {
            get
            {
                return bookingClassField;
            }
            set
            {
                bookingClassField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponentSeats Seats
        {
            get
            {
                return seatsField;
            }
            set
            {
                seatsField = value;
            }
        }

        /// <remarks/>
        public object SegmentStatus
        {
            get
            {
                return segmentStatusField;
            }
            set
            {
                segmentStatusField = value;
            }
        }

        /// <remarks/>
        public string FareBasis
        {
            get
            {
                return fareBasisField;
            }
            set
            {
                fareBasisField = value;
            }
        }

        /// <remarks/>
        public object NumberOfStops
        {
            get
            {
                return numberOfStopsField;
            }
            set
            {
                numberOfStopsField = value;
            }
        }

        /// <remarks/>
        public decimal BaseFareAmount
        {
            get
            {
                return baseFareAmountField;
            }
            set
            {
                baseFareAmountField = value;
            }
        }

        /// <remarks/>
        public decimal SurchargeAmount
        {
            get
            {
                return surchargeAmountField;
            }
            set
            {
                surchargeAmountField = value;
            }
        }

        /// <remarks/>
        public byte Miles
        {
            get
            {
                return milesField;
            }
            set
            {
                milesField = value;
            }
        }

        /// <remarks/>
        public object TourCode
        {
            get
            {
                return tourCodeField;
            }
            set
            {
                tourCodeField = value;
            }
        }

        /// <remarks/>
        public ushort AccountNumber
        {
            get
            {
                return accountNumberField;
            }
            set
            {
                accountNumberField = value;
            }
        }

        /// <remarks/>
        public string DITCode
        {
            get
            {
                return dITCodeField;
            }
            set
            {
                dITCodeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public byte SequenceNumber
        {
            get
            {
                return sequenceNumberField;
            }
            set
            {
                sequenceNumberField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string SupplierServiceName
        {
            get
            {
                return supplierServiceNameField;
            }
            set
            {
                supplierServiceNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponentRailCode
    {

        private string codeField;

        private string nameField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsRailComponentSeats
    {

        private object seatField;

        private string numberField;

        /// <remarks/>
        public object Seat
        {
            get
            {
                return seatField;
            }
            set
            {
                seatField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Number
        {
            get
            {
                return numberField;
            }
            set
            {
                numberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFare
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFareTravelTaxList travelTaxListField;

        private string reasonCodeField;

        private string reasonCodeDescField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFarePaymentInformation paymentInformationField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFareTravelTaxList TravelTaxList
        {
            get
            {
                return travelTaxListField;
            }
            set
            {
                travelTaxListField = value;
            }
        }

        /// <remarks/>
        public string ReasonCode
        {
            get
            {
                return reasonCodeField;
            }
            set
            {
                reasonCodeField = value;
            }
        }

        /// <remarks/>
        public string ReasonCodeDesc
        {
            get
            {
                return reasonCodeDescField;
            }
            set
            {
                reasonCodeDescField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFarePaymentInformation PaymentInformation
        {
            get
            {
                return paymentInformationField;
            }
            set
            {
                paymentInformationField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFareTravelTaxList
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFareTravelTaxListTravelTax travelTaxField;

        private decimal totalTaxAmountField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFareTravelTaxListTravelTax TravelTax
        {
            get
            {
                return travelTaxField;
            }
            set
            {
                travelTaxField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public decimal TotalTaxAmount
        {
            get
            {
                return totalTaxAmountField;
            }
            set
            {
                totalTaxAmountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFareTravelTaxListTravelTax
    {

        private object[] itemsField;

        /// <remarks/>
        [XmlElementAttribute("TaxCode", typeof(object))]
        [XmlElementAttribute("TravelTaxAmount", typeof(decimal))]
        public object[] Items
        {
            get
            {
                return itemsField;
            }
            set
            {
                itemsField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFarePaymentInformation
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFarePaymentInformationFormOfPayment formOfPaymentField;

        private decimal paymentAmountField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFarePaymentInformationFormOfPayment FormOfPayment
        {
            get
            {
                return formOfPaymentField;
            }
            set
            {
                formOfPaymentField = value;
            }
        }

        /// <remarks/>
        public decimal PaymentAmount
        {
            get
            {
                return paymentAmountField;
            }
            set
            {
                paymentAmountField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsStoredRailFarePaymentInformationFormOfPayment
    {

        private string cardNumberField;

        private string cardTypeField;

        /// <remarks/>
        public string CardNumber
        {
            get
            {
                return cardNumberField;
            }
            set
            {
                cardNumberField = value;
            }
        }

        /// <remarks/>
        public string CardType
        {
            get
            {
                return cardTypeField;
            }
            set
            {
                cardTypeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsAirRailDetailsRailDetailsInvoice
    {

        private string ticketTypeField;

        private ulong ticketNumberField;

        private string validatingCarrierField;

        private uint invoiceNumberField;

        private DateTime invoiceGMTTimestampField;

        private uint iATANumberField;

        private object origTicketNumberField;

        /// <remarks/>
        public string TicketType
        {
            get
            {
                return ticketTypeField;
            }
            set
            {
                ticketTypeField = value;
            }
        }

        /// <remarks/>
        public ulong TicketNumber
        {
            get
            {
                return ticketNumberField;
            }
            set
            {
                ticketNumberField = value;
            }
        }

        /// <remarks/>
        public string ValidatingCarrier
        {
            get
            {
                return validatingCarrierField;
            }
            set
            {
                validatingCarrierField = value;
            }
        }

        /// <remarks/>
        public uint InvoiceNumber
        {
            get
            {
                return invoiceNumberField;
            }
            set
            {
                invoiceNumberField = value;
            }
        }

        /// <remarks/>
        public DateTime InvoiceGMTTimestamp
        {
            get
            {
                return invoiceGMTTimestampField;
            }
            set
            {
                invoiceGMTTimestampField = value;
            }
        }

        /// <remarks/>
        public uint IATANumber
        {
            get
            {
                return iATANumberField;
            }
            set
            {
                iATANumberField = value;
            }
        }

        /// <remarks/>
        public object OrigTicketNumber
        {
            get
            {
                return origTicketNumberField;
            }
            set
            {
                origTicketNumberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetails
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponent[] rentComponentListField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsInvoice invoiceField;

        /// <remarks/>
        [XmlArrayItemAttribute("RentComponent", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponent[] RentComponentList
        {
            get
            {
                return rentComponentListField;
            }
            set
            {
                rentComponentListField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsInvoice Invoice
        {
            get
            {
                return invoiceField;
            }
            set
            {
                invoiceField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponent
    {

        private string recordLocatorField;

        private string bookingPseudoCityField;

        private byte branchField;

        private string confirmationNumberField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentVendor vendorField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentPickLocation pickLocationField;

        private DateTime pickupDateField;

        private DateTime returnDateField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentVehicle vehicleField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentRentalRate rentalRateField;

        private string autoStateField;

        private decimal rentalExceptionRateField;

        private string sequenceNumberField;

        private string supplierServiceNameField;

        private string typeField;

        /// <remarks/>
        public string RecordLocator
        {
            get
            {
                return recordLocatorField;
            }
            set
            {
                recordLocatorField = value;
            }
        }

        /// <remarks/>
        public string BookingPseudoCity
        {
            get
            {
                return bookingPseudoCityField;
            }
            set
            {
                bookingPseudoCityField = value;
            }
        }

        /// <remarks/>
        public byte Branch
        {
            get
            {
                return branchField;
            }
            set
            {
                branchField = value;
            }
        }

        /// <remarks/>
        public string ConfirmationNumber
        {
            get
            {
                return confirmationNumberField;
            }
            set
            {
                confirmationNumberField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentVendor Vendor
        {
            get
            {
                return vendorField;
            }
            set
            {
                vendorField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentPickLocation PickLocation
        {
            get
            {
                return pickLocationField;
            }
            set
            {
                pickLocationField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime PickupDate
        {
            get
            {
                return pickupDateField;
            }
            set
            {
                pickupDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime ReturnDate
        {
            get
            {
                return returnDateField;
            }
            set
            {
                returnDateField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentVehicle Vehicle
        {
            get
            {
                return vehicleField;
            }
            set
            {
                vehicleField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentRentalRate RentalRate
        {
            get
            {
                return rentalRateField;
            }
            set
            {
                rentalRateField = value;
            }
        }

        /// <remarks/>
        public string AutoState
        {
            get
            {
                return autoStateField;
            }
            set
            {
                autoStateField = value;
            }
        }

        /// <remarks/>
        public decimal RentalExceptionRate
        {
            get
            {
                return rentalExceptionRateField;
            }
            set
            {
                rentalExceptionRateField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string SequenceNumber
        {
            get
            {
                return sequenceNumberField;
            }
            set
            {
                sequenceNumberField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string SupplierServiceName
        {
            get
            {
                return supplierServiceNameField;
            }
            set
            {
                supplierServiceNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentVendor
    {

        private string codeField;

        private string nameField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentPickLocation
    {

        private string codeField;

        private string nameField;

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Code
        {
            get
            {
                return codeField;
            }
            set
            {
                codeField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentVehicle
    {

        private string vehicleTypeField;

        private byte vehicleQuantityField;

        /// <remarks/>
        public string VehicleType
        {
            get
            {
                return vehicleTypeField;
            }
            set
            {
                vehicleTypeField = value;
            }
        }

        /// <remarks/>
        public byte VehicleQuantity
        {
            get
            {
                return vehicleQuantityField;
            }
            set
            {
                vehicleQuantityField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentRentalRate
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentRentalRateRateDistance rateDistanceField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentRentalRateRateDistance RateDistance
        {
            get
            {
                return rateDistanceField;
            }
            set
            {
                rateDistanceField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsRentComponentRentalRateRateDistance
    {

        private object currencyCodeField;

        private object rateCodeField;

        private decimal rateAmountField;

        private string unitCostField;

        private string currencyCode1Field;

        /// <remarks/>
        public object CurrencyCode
        {
            get
            {
                return currencyCodeField;
            }
            set
            {
                currencyCodeField = value;
            }
        }

        /// <remarks/>
        public object RateCode
        {
            get
            {
                return rateCodeField;
            }
            set
            {
                rateCodeField = value;
            }
        }

        /// <remarks/>
        public decimal RateAmount
        {
            get
            {
                return rateAmountField;
            }
            set
            {
                rateAmountField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string UnitCost
        {
            get
            {
                return unitCostField;
            }
            set
            {
                unitCostField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute("CurrencyCode")]
        public string CurrencyCode1
        {
            get
            {
                return currencyCode1Field;
            }
            set
            {
                currencyCode1Field = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsRentDetailsInvoice
    {

        private uint invoiceNumberField;

        private DateTime invoiceDateField;

        private uint iATANumberField;

        /// <remarks/>
        public uint InvoiceNumber
        {
            get
            {
                return invoiceNumberField;
            }
            set
            {
                invoiceNumberField = value;
            }
        }

        /// <remarks/>
        public DateTime InvoiceDate
        {
            get
            {
                return invoiceDateField;
            }
            set
            {
                invoiceDateField = value;
            }
        }

        /// <remarks/>
        public uint IATANumber
        {
            get
            {
                return iATANumberField;
            }
            set
            {
                iATANumberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetails
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponent[] stayComponentListField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsInvoice invoiceField;

        /// <remarks/>
        [XmlArrayItemAttribute("StayComponent", IsNullable = false)]
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponent[] StayComponentList
        {
            get
            {
                return stayComponentListField;
            }
            set
            {
                stayComponentListField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsInvoice Invoice
        {
            get
            {
                return invoiceField;
            }
            set
            {
                invoiceField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponent
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservation reservationField;

        private string sequenceNumberField;

        private string supplierServiceNameField;

        private string typeField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservation Reservation
        {
            get
            {
                return reservationField;
            }
            set
            {
                reservationField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string SequenceNumber
        {
            get
            {
                return sequenceNumberField;
            }
            set
            {
                sequenceNumberField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string SupplierServiceName
        {
            get
            {
                return supplierServiceNameField;
            }
            set
            {
                supplierServiceNameField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute]
        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservation
    {

        private string recordLocatorField;

        private string bookingPseudoCityField;

        private byte branchField;

        private byte numberOfUnitsField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoom roomField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoomRates roomRatesField;

        private byte guestCountField;

        private string confirmationNumberField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationTimeSpan timeSpanField;

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationPropertyInfo propertyInfoField;

        private decimal hotelExceptionRateField;

        private object guaranteedIndicatorField;

        /// <remarks/>
        public string RecordLocator
        {
            get
            {
                return recordLocatorField;
            }
            set
            {
                recordLocatorField = value;
            }
        }

        /// <remarks/>
        public string BookingPseudoCity
        {
            get
            {
                return bookingPseudoCityField;
            }
            set
            {
                bookingPseudoCityField = value;
            }
        }

        /// <remarks/>
        public byte Branch
        {
            get
            {
                return branchField;
            }
            set
            {
                branchField = value;
            }
        }

        /// <remarks/>
        public byte NumberOfUnits
        {
            get
            {
                return numberOfUnitsField;
            }
            set
            {
                numberOfUnitsField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoom Room
        {
            get
            {
                return roomField;
            }
            set
            {
                roomField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoomRates RoomRates
        {
            get
            {
                return roomRatesField;
            }
            set
            {
                roomRatesField = value;
            }
        }

        /// <remarks/>
        public byte GuestCount
        {
            get
            {
                return guestCountField;
            }
            set
            {
                guestCountField = value;
            }
        }

        /// <remarks/>
        public string ConfirmationNumber
        {
            get
            {
                return confirmationNumberField;
            }
            set
            {
                confirmationNumberField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationTimeSpan TimeSpan
        {
            get
            {
                return timeSpanField;
            }
            set
            {
                timeSpanField = value;
            }
        }

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationPropertyInfo PropertyInfo
        {
            get
            {
                return propertyInfoField;
            }
            set
            {
                propertyInfoField = value;
            }
        }

        /// <remarks/>
        public decimal HotelExceptionRate
        {
            get
            {
                return hotelExceptionRateField;
            }
            set
            {
                hotelExceptionRateField = value;
            }
        }

        /// <remarks/>
        public object GuaranteedIndicator
        {
            get
            {
                return guaranteedIndicatorField;
            }
            set
            {
                guaranteedIndicatorField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoom
    {

        private string roomTypeCodeField;

        /// <remarks/>
        public string RoomTypeCode
        {
            get
            {
                return roomTypeCodeField;
            }
            set
            {
                roomTypeCodeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoomRates
    {

        private iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoomRatesRoomRate roomRateField;

        /// <remarks/>
        public iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoomRatesRoomRate RoomRate
        {
            get
            {
                return roomRateField;
            }
            set
            {
                roomRateField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationRoomRatesRoomRate
    {

        private decimal rateAmountField;

        private string currencyCodeField;

        private string rateTypeCodeField;

        /// <remarks/>
        public decimal RateAmount
        {
            get
            {
                return rateAmountField;
            }
            set
            {
                rateAmountField = value;
            }
        }

        /// <remarks/>
        public string CurrencyCode
        {
            get
            {
                return currencyCodeField;
            }
            set
            {
                currencyCodeField = value;
            }
        }

        /// <remarks/>
        public string RateTypeCode
        {
            get
            {
                return rateTypeCodeField;
            }
            set
            {
                rateTypeCodeField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationTimeSpan
    {

        private DateTime checkInDateField;

        private DateTime checkOutDateField;

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime CheckInDate
        {
            get
            {
                return checkInDateField;
            }
            set
            {
                checkInDateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(DataType = "date")]
        public DateTime CheckOutDate
        {
            get
            {
                return checkOutDateField;
            }
            set
            {
                checkOutDateField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsStayComponentReservationPropertyInfo
    {

        private string hotelNameField;

        private string hotelCityCodeField;

        private string hotelCityField;

        private string hotelStateField;

        private string chainCodeField;

        private string phoneNumberField;

        /// <remarks/>
        public string HotelName
        {
            get
            {
                return hotelNameField;
            }
            set
            {
                hotelNameField = value;
            }
        }

        /// <remarks/>
        public string HotelCityCode
        {
            get
            {
                return hotelCityCodeField;
            }
            set
            {
                hotelCityCodeField = value;
            }
        }

        /// <remarks/>
        public string HotelCity
        {
            get
            {
                return hotelCityField;
            }
            set
            {
                hotelCityField = value;
            }
        }

        /// <remarks/>
        public string HotelState
        {
            get
            {
                return hotelStateField;
            }
            set
            {
                hotelStateField = value;
            }
        }

        /// <remarks/>
        public string ChainCode
        {
            get
            {
                return chainCodeField;
            }
            set
            {
                chainCodeField = value;
            }
        }

        /// <remarks/>
        public string PhoneNumber
        {
            get
            {
                return phoneNumberField;
            }
            set
            {
                phoneNumberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryItineraryInfoReservationItemsStayDetailsInvoice
    {

        private uint invoiceNumberField;

        private uint iATANumberField;

        /// <remarks/>
        public uint InvoiceNumber
        {
            get
            {
                return invoiceNumberField;
            }
            set
            {
                invoiceNumberField = value;
            }
        }

        /// <remarks/>
        public uint IATANumber
        {
            get
            {
                return iATANumberField;
            }
            set
            {
                iATANumberField = value;
            }
        }
    }

    /// <remarks/>
    [SerializableAttribute]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class iXMLIBank_TravelItineraryTravelItineraryUDIDS
    {

        private byte uDIDNumberField;

        private string uDIDValueField;

        /// <remarks/>
        public byte UDIDNumber
        {
            get
            {
                return uDIDNumberField;
            }
            set
            {
                uDIDNumberField = value;
            }
        }

        /// <remarks/>
        public string UDIDValue
        {
            get
            {
                return uDIDValueField;
            }
            set
            {
                uDIDValueField = value;
            }
        }
    }


}