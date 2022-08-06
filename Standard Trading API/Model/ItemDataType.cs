
using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;
namespace Standard_Trading_API.Model
{
    public class ItemDataType
    {
        [DataContract]
        public class ItemsDefinition : DefaultRequest
        {

            [DataMember]
            public int? itemId { get; set; }

            [DataMember]
            public string itemCode { get; set; }
            [DataMember]
            public string itemDescription { get; set; }
            [DataMember]
            public string uomName { get; set; }
            [DataMember]
            public string mainTypeName { get; set; }

            [DataMember]
            public string subTypeName { get; set; }

            [DataMember]
            public string manufactureByName { get; set; }
            [DataMember]
            public string packUnitName { get; set; }
            [DataMember]
            public string accountName { get; set; }
            [DataMember]
            public decimal? salesRate { get; set; }
            [DataMember]
            public decimal? mrpRate { get; set; }
            [DataMember]
            public decimal? purchaseRate { get; set; }
            [DataMember]
            public decimal? minStockLevel { get; set; }
            [DataMember]
            public decimal? maxStockLevel { get; set; }
            [DataMember]
            public decimal? reorderStockLevel { get; set; }
            [DataMember]
            public Boolean? pack { get; set; }
            [DataMember]
            public decimal? packQty { get; set; }
            [DataMember]
            public Boolean? salesable { get; set; }
            [DataMember]
            public Boolean? purchaseable { get; set; }
            [DataMember]
            public Boolean? attachBarcode { get; set; }
            [DataMember]
            public Boolean? active { get; set; }
            [DataMember]
            public Boolean? expiryDate { get; set; }
            [DataMember]
            public DateTime itemExpiryDate { get; set; }
            [DataMember]
            public string photoPath { get; set; }

            [DataMember]
            public string addorEdit { get; set; }

            


        }    
    }
}
