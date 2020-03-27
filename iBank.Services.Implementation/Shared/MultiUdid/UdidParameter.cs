
namespace iBank.Services.Implementation.Shared.MultiUdid
{
    public class UdidParameter
    {
        public UdidParameter()
        {
            Seq = 0;
            UdidNumber = 0;
            UdidText = string.Empty;
        }
        public int Seq { get; set; }
        public int UdidNumber { get; set; }
        public string UdidText { get; set; }
    }
}
