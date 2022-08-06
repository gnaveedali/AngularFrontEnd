using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    public class SegmentManagementDataType
    {
        [DataContract]
        public class ValueSets : DefaultRequest
        {
            [DataMember]
            public int? ValueSetID { get; set; }

            [DataMember]
            public string? ValueSet_Name { get; set; }

           


        }

        [DataContract]
        public class DataValues 
        {
            [DataMember]
            public int? DataValueID { get; set; }
           
            [DataMember]
            public int? ValueSetID { get; set; }


            [DataMember]
            public string? DataValueName { get; set; }

            [DataMember]
            public string? Description { get; set; }

            [DataMember]
            public string? TransactionType { get; set; }

        }
    }
}
