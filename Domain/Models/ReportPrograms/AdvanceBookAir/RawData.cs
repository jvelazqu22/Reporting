﻿using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AdvanceBookAir
{
    [Serializable]
    public class RawData : IRouteWhere, ICollapsible
    {
        public string Connect { get; set; }
        public int SeqNo { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; } = DateTime.Now;
        public DateTime? RDepDate { get; set; } = DateTime.Now;
        public string DepTime { get; set; } = string.Empty;
        public DateTime? DepDate { get; set; } = DateTime.Now;
        [ExchangeDate1]
        public DateTime? Invdate { get; set; } = DateTime.Now;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal? Airchg { get; set; } = 0m;
        public int PlusMin { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public decimal ActFare { get; set; } = 0m;
        public decimal MiscAmt { get; set; }
        public string Airline { get; set; } = string.Empty;
        public string fltno { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public DateTime? RArrDate { get; set; } = DateTime.Now;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int Seg_Cntr { get; set; }
        public string Classcat { get; set; } = string.Empty;
        public string DomIntl { get; set; } = string.Empty;
        public int Miles { get; set; } = 0;
        public string DitCode { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }
}