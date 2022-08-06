using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    [DataContract]
    public class GlAccountsDataType
    {

        public class GlAccounts: DefaultRequest
        {
            
             [DataMember]
            public int? accountId { get; set; }

            [DataMember]
            public string? accountCode { get; set; }

            [DataMember]
            public string? accountName { get; set; }

            [DataMember]
            public int? levels { get; set; }

            [DataMember]
            public int? parentCode { get; set; }

            [DataMember]
            public int? BookId { get; set; }

            [DataMember]
            public bool? Active { get; set; }

            [DataMember]
            public string? AccountType { get; set; }

            [DataMember]
            public bool? Expanded { get; set; }

            [DataMember]
            public string? icon { get; set; }
        }
    }
}
