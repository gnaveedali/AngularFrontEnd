using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    [DataContract]
    public class ItemAccountDataType
    {
        public class ItemAccount : DefaultRequest
        {
            [DataMember]
           
            public int ItemAccountId { get; set; }
            [DataMember]
            public string ItemAccountName { get; set; }
            [DataMember]
            public int GlSalesAccount { get; set; }
            [DataMember]
            public int GlStockAccount { get; set; }
            [DataMember]
            public int GlCostAccount { get; set; }
            [DataMember]
            public int GlConsumptionAccount { get; set; }
            [DataMember]
            public int BookId { get; set; }
            [DataMember]
            public int? GlSalePromotionAccount { get; set; }
            [DataMember]
            public string SalesAcName { get; set; }
            [DataMember]
            public string StockAcName { get; set; }
            [DataMember]
            public string CostAcName { get; set; }
            [DataMember]
            public string ConsumptionAcName { get; set; }
            [DataMember]
            public string SalePromotionAcName { get; set; }
            [DataMember]
            public string SalesAcCode { get; set; }
            [DataMember]
            public string StockAcCode { get; set; }
            [DataMember]
            public string CostAcCode { get; set; }
            [DataMember]
            public string ConsumptionAcCode { get; set; }
            [DataMember]
            public string SalePromotionAcCode { get; set; }
            [DataMember]
            public string transactionType { get; set; }
        }

    }
}
