//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iBank.Services.Orm
{
    using System;
    using System.Collections.Generic;
    
    public partial class ibMiscSeg
    {
        public int recordNo { get; set; }
        public string agency { get; set; }
        public int reckey { get; set; }
        public string recloc { get; set; }
        public string segType { get; set; }
        public string msOrigin { get; set; }
        public string msDestinat { get; set; }
        public string msOrgCtry { get; set; }
        public string msDestCtry { get; set; }
        public string vendorCode { get; set; }
        public string svcIdNbr { get; set; }
        public Nullable<System.DateTime> msDepDate { get; set; }
        public string msDepTime { get; set; }
        public Nullable<System.DateTime> msArrDate { get; set; }
        public string msArrTime { get; set; }
        public short msSeqno { get; set; }
        public string @class { get; set; }
        public short msPlusmin { get; set; }
        public string msTrantype { get; set; }
        public decimal segAmt { get; set; }
        public string moneyType { get; set; }
        public decimal msExcpRate { get; set; }
        public decimal msStndRate { get; set; }
        public string msLossCode { get; set; }
        public string msSvgCode { get; set; }
        public decimal tax1 { get; set; }
        public decimal tax2 { get; set; }
        public decimal tax3 { get; set; }
        public decimal tax4 { get; set; }
        public string prodcode { get; set; }
        public int numvehics { get; set; }
        public int numadults { get; set; }
        public int numchild { get; set; }
        public string cabintype { get; set; }
        public string confirmno { get; set; }
        public string cabinSeat { get; set; }
        public string mxChainCod { get; set; }
        public string mealDesc { get; set; }
        public Nullable<short> nitecount { get; set; }
        public string opt { get; set; }
        public string arriveRmks { get; set; }
        public string departRmks { get; set; }
        public string mxTourcode { get; set; }
        public string mxTourName { get; set; }
        public string trnsfrRmks { get; set; }
        public string mxVendName { get; set; }
        public string mxSgStatus { get; set; }
        public Nullable<short> tourCount { get; set; }
        public Nullable<decimal> basePrice1 { get; set; }
        public string basePrice2 { get; set; }
        public Nullable<byte> nbrRooms { get; set; }
        public Nullable<decimal> msCommisn { get; set; }
        public string msOrgCode { get; set; }
        public string msDestCode { get; set; }
        public Nullable<byte> msSegNum { get; set; }
        public Nullable<short> msDuration { get; set; }
        public string shipName { get; set; }
        public string cabinCateg { get; set; }
        public string cabinNbr { get; set; }
        public string cabinDeck { get; set; }
        public string pgmID { get; set; }
        public string spclInfo { get; set; }
        public string regionID { get; set; }
        public string msRateType { get; set; }
    }
}
