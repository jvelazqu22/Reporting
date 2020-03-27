using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IXmlTravelerInfo
    {
        string Passfrst { get; set; }
        string Passlast { get; set; }
        string Emailaddr { get; set; }
        string Phone { get; set; }
        string Ticket { get; set; }
        string Break1 { get; set; }
        string Break2 { get; set; }
        string Break3 { get; set; }
    }
}
