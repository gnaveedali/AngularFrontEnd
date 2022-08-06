using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    public class VoucherDataType
    {
        [DataContract]

        public class GLVoucherType : DefaultRequest
        {
            [DataMember]
            public string voucherType { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string RefNarration { get; set; }
            [DataMember]
            public DateTime FrontDate { get; set; }

        }

        [DataContract]

        public class VoucherData
        {
            [DataMember]
            public string voucherType { get; set; }
            [DataMember]
            public string voucherPeriod { get; set; }
            [DataMember]
            public DateTime? defaultDate { get; set; }
            [DataMember]
            public DateTime? periodStDate { get; set; }
            [DataMember]
            public DateTime? periodEnDate { get; set; }
            [DataMember]
            public string sPeriod;
            [DataMember]
            public string voucherDescription;
            [DataMember]
            public string refNarration;
           

        }
    }
}
