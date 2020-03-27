using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    [Serializable]
    public class LegRawData : ICarbonCalculations, IRouteWhere, ICollapsible, IAirMileage
    {
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; }
        public string Recloc { get; set; }
        public string Agency { get; set; }
        public bool Exchange { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public decimal? Basefare { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public DateTime? RDepDate { get; set; }
        public string DepTime { get; set; }
        public DateTime? RArrDate { get; set; }        
        public string ArrTime { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public string Connect { get; set; }
        public string Mode { get; set; }
        public int SeqNo { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal ActFare { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal MiscAmt { get; set; }

        public string Farebase { get; set; }
        public string DitCode { get; set; }
        public string Mktpair { get; set; }
        public int? Smartctrfl { get; set; }
        public string Carriertyp { get; set; }
        public string Tktsegstat { get; set; }
        public string Endodflag { get; set; }
        public int Flduration { get; set; }
        public string Agentid { get; set; }
        public string Pseudocity { get; set; }
        public string Branch { get; set; }

        [AirCurrency]
        public string Moneytype { get; set; }

        [ExchangeDate1]
        public DateTime? Invdate { get; set; }

        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }

        public int Rplusmin { get; set; }
        public string Tktdesig { get; set; }
        public int Segnum { get; set; }
        public string Origorigin { get; set; }
        public string Origdest { get; set; }
        public string Origcarr { get; set; }
        public string Seat { get; set; }
        public string Stops { get; set; }
        public string Segstatus { get; set; }
        public string Emailaddr { get; set; }
        public string Gds { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Fltgrsfare { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal? Fltprofare { get; set; }

        public string Equip { get; set; }


        public string ClassCat { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string HaulType { get; set; }

        //Calculated fields for Air Legs
        public decimal AirSgcost
        {
            get { return ActFare + MiscAmt; }
        }
    }
}
