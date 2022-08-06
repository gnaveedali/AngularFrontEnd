using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    [DataContract]
    public class BarcodeDataType
    {

        public class MultiBarcode : DefaultRequest
        {
            [DataMember]

            public int? barcodeId { get; set; }
            [DataMember]
            public string itemCode { get; set; }
            [DataMember]
            public string barcode { get; set; }

            [DataMember]
            public string description { get; set; }
            [DataMember]
            public int? counter { get; set; }

            
        }
    }
}
