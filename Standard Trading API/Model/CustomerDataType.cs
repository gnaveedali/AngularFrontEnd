﻿using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    [DataContract]
    public class CustomerDataType
    {
        public class Ar_Customer : DefaultRequest
        {
            [DataMember]
            public int ? customerId { get; set; }
            [DataMember]
            public string customerName { get; set; }
            [DataMember]
            public string address { get; set; }
            
            [DataMember]
            public string mobileNo { get; set; }
            [DataMember]
            public string phoneNo { get; set; }
            [DataMember]
            public string faxNo { get; set; }
            [DataMember]
            public string emailAddress { get; set; }
            [DataMember]
            public string salesTaxNo { get; set; }
            [DataMember]
            public string ntnNo { get; set; }
            [DataMember]
            public int? accountId { get; set; }
            [DataMember]
            public string photoPath { get; set; }
            [DataMember]
            public string addorEdit { get; set; }
            [DataMember]
            public Boolean addManually { get; set; }
            [DataMember]
            public string cityName { get; set; }
            [DataMember]
            public string countryName { get; set; }

            [DataMember]
            public string AccountCode { get; set; }

            








        }

    }
}
