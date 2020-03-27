using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class UserDefinedParameters
    {
        public List<RawData> TripDataList { get; set; } = new List<RawData>();
        public List<RawData> CancelledTripDataList { get; set; } = new List<RawData>();
        public List<CarRawData> CarDataList { get; set; } = new List<CarRawData>();
        public List<HotelRawData> HotelDataList { get; set; } = new List<HotelRawData>();
        public List<MiscRawData> MiscDataList { get; set; } = new List<MiscRawData>();
        public List<TravAuthRawData> TravAuthDataList { get; set; } = new List<TravAuthRawData>();
        public List<TravAuthorizerRawData> TravAuthorizerDataList { get; set; } = new List<TravAuthorizerRawData>();
        public List<ChangeLogData> ChangeLogDataList { get; set; } = new List<ChangeLogData>();
        public List<ServiceFeeData> ServiceFeeDataList { get; set; } = new List<ServiceFeeData>();
        public List<LegRawData> LegDataList { get; set; } = new List<LegRawData>();
        public List<LegRawData> AirLegDataList { get; set; } = new List<LegRawData>();
        public List<LegRawData> RailLegDataList { get; set; } = new List<LegRawData>();
        public List<UdidRawData> UdidDataList { get; set; } = new List<UdidRawData>();
        public List<MarketSegmentRawData> MarketSegmentDataList { get; set; } = new List<MarketSegmentRawData>();
        public List<MiscSegSharedRawData> MiscSegTourDataList { get; set; } = new List<MiscSegSharedRawData>();
        public List<MiscSegSharedRawData> MiscSegCruiseDataList { get; set; } = new List<MiscSegSharedRawData>();
        public List<MiscSegSharedRawData> MiscSegLimoDataList { get; set; } = new List<MiscSegSharedRawData>();
        public List<MiscSegSharedRawData> MiscSegRailTicketDataList { get; set; } = new List<MiscSegSharedRawData>();

        private ILookup<int, RawData> _tripLookup = null;
        public ILookup<int, RawData> TripLookup
        {
            get
            {
                if (_tripLookup == null) _tripLookup = GetLookup(TripDataList);
                return _tripLookup;
            }
        }

        private ILookup<int, RawData> _cancelledTripLookup = null;
        public ILookup<int, RawData> CancelledTripLookup
        {
            get
            {
                if (_cancelledTripLookup == null) _cancelledTripLookup = GetLookup(CancelledTripDataList);
                return _cancelledTripLookup;
            }
        }

        private ILookup<int, CarRawData> _carLookup = null;
        public ILookup<int, CarRawData> CarLookup
        {
            get
            {
                if (_carLookup == null) _carLookup = GetLookup(CarDataList);
                return _carLookup;
            }
        }

        private ILookup<int, HotelRawData> _hotelLookup = null;
        public ILookup<int, HotelRawData> HotelLookup
        {
            get
            {
                if (_hotelLookup == null) _hotelLookup = GetLookup(HotelDataList);
                return _hotelLookup;
            }
        }

        private ILookup<int, MiscRawData> _miscLookup = null;
        public ILookup<int, MiscRawData> MiscLookup
        {
            get
            {
                if (_miscLookup == null) _miscLookup = GetLookup(MiscDataList);
                return _miscLookup;
            }
        }

        private ILookup<int, TravAuthRawData> _travAuthLookup = null;
        public ILookup<int, TravAuthRawData> TravAuthLookup
        {
            get
            {
                if (_travAuthLookup == null) _travAuthLookup = GetLookup(TravAuthDataList);
                return _travAuthLookup;
            }
        }

        private ILookup<int, TravAuthorizerRawData> _travAuthorizerLookup = null;

        public ILookup<int, TravAuthorizerRawData> TravAuthorizerLookup
        {
            get
            {
                if (_travAuthorizerLookup == null) _travAuthorizerLookup = GetLookup(TravAuthorizerDataList);
                return _travAuthorizerLookup;
            }
        }

        private ILookup<int, ChangeLogData> _changeLogLookup = null;

        public ILookup<int, ChangeLogData> ChangeLogLookup
        {
            get
            {
                if (_changeLogLookup == null) _changeLogLookup = GetLookup(ChangeLogDataList);
                return _changeLogLookup;
            }
        }

        private ILookup<int, ServiceFeeData> _serviceFeeLookup = null;

        public ILookup<int, ServiceFeeData> ServiceFeeLookup
        {
            get
            {
                if (_serviceFeeLookup == null) _serviceFeeLookup = GetLookup(ServiceFeeDataList);
                return _serviceFeeLookup;
            }
        }

        private ILookup<int, LegRawData> _legLookup = null;

        public ILookup<int, LegRawData> LegLookup
        {
            get
            {
                if (_legLookup == null) _legLookup = GetLookup(LegDataList);
                return _legLookup;
            }
        }

        private ILookup<int, LegRawData> _airLegLookup = null;

        public ILookup<int, LegRawData> AirLegLookup
        {
            get
            {
                if (_airLegLookup == null) _airLegLookup = GetLookup(AirLegDataList);
                return _airLegLookup;
            }
        }

        private ILookup<int, LegRawData> _railLegLookup = null;

        public ILookup<int, LegRawData> RailLegLookup
        {
            get
            {
                if (_railLegLookup == null) _railLegLookup = GetLookup(RailLegDataList);
                return _railLegLookup;
            }
        }

        private ILookup<int, UdidRawData> _udidLookup = null;

        public ILookup<int, UdidRawData> UdidLookup
        {
            get
            {
                if (_udidLookup == null) _udidLookup = GetLookup(UdidDataList);
                return _udidLookup;
            }
        }

        private ILookup<int, MarketSegmentRawData> _marketSegmentLookup = null;

        public ILookup<int, MarketSegmentRawData> MarketSegmentLookup
        {
            get
            {
                if (_marketSegmentLookup == null) _marketSegmentLookup = GetLookup(MarketSegmentDataList);
                return _marketSegmentLookup;
            }
        }

        private ILookup<int, MiscSegSharedRawData> _miscSegTourLookup = null;

        public ILookup<int, MiscSegSharedRawData> MiscSegTourLookup
        {
            get
            {
                if (_miscSegTourLookup == null) _miscSegTourLookup = GetLookup(MiscSegTourDataList);
                return _miscSegTourLookup;
            }
        }

        private ILookup<int, MiscSegSharedRawData> _miscSegCruiseLookup = null;

        public ILookup<int, MiscSegSharedRawData> MiscSegCruiseLookup
        {
            get
            {
                if (_miscSegCruiseLookup == null) _miscSegCruiseLookup = GetLookup(MiscSegCruiseDataList);
                return _miscSegCruiseLookup;
            }
        }

        private ILookup<int, MiscSegSharedRawData> _miscSegLimoLookup = null;

        public ILookup<int, MiscSegSharedRawData> MiscSegLimoLookup
        {
            get
            {
                if (_miscSegLimoLookup == null) _miscSegLimoLookup = GetLookup(MiscSegLimoDataList);
                return _miscSegLimoLookup;
            }
        }

        private ILookup<int, MiscSegSharedRawData> _miscSegRailTicketLookup = null;

        public ILookup<int, MiscSegSharedRawData> MiscSegRailTicketLookup
        {
            get
            {
                if (_miscSegRailTicketLookup == null) _miscSegRailTicketLookup = GetLookup(MiscSegRailTicketDataList);
                return _miscSegRailTicketLookup;
            }
        }

        private ILookup<int, T> GetLookup<T>(List<T> list) where T : IRecKey
        {
            return list.ToLookup(x => x.RecKey);
        }
    }
}
