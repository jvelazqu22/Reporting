using System;

namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class TimeStrings
    {
        private DateTime _current;

        public TimeStrings()
        {
            _current = DateTime.Now;
        }

        public TimeStrings(DateTime date)
        {
            _current = date;
        }

        public string Year { get
        {
            return _current.ToString("yyyy");
        } }

        public string Month { get
        {
            return _current.ToString("MM");
        } }

        public string Day { get
        {
            return _current.ToString("dd");
        } }

        public string Hour { get
        {
            return _current.ToString("hh");
        } }

        public string Min { get
        {
            return _current.ToString("mm");
        } }

        public string Sec { get
        {
            return _current.ToString("ss");
        } }

        public string Ms { get
        {
            return _current.ToString("fff");
        } }
    }
}