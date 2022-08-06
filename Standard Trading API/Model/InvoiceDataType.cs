using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    [DataContract]
    public class InvoiceDataType
    {
        public class Ar_Invoice : DefaultRequest
        {
            [DataMember]
            public int? InvoiceId { get; set; }
            [DataMember]
            public int? InvoiceNo { get; set; }
            [DataMember]
            public DateTime InvoiceDate { get; set; }
            [DataMember]
            public int? WareHouse { get; set; }

            [DataMember]
            public int? Saleman { get; set; }
            [DataMember]
            public int? CustomerId { get; set; }
            [DataMember]
            public string Ar_CustomerName { get; set; }

            [DataMember]
            public int? PeriodId { get; set; }

            [DataMember]
            public int? AccountId { get; set; }
            [DataMember]
            public string CustomerName { get; set; }
            [DataMember]
            public string CustomerAddress { get; set; }
            [DataMember]
            public string RefrenceNo { get; set; }

            [DataMember]
            public string Remarks { get; set; }
            [DataMember]
            public decimal? SalesTaxPercentage { get; set; }

            [DataMember]
            public decimal? SalesTaxAmount { get; set; }
            [DataMember]
            public decimal? ExiseTaxPercentage { get; set; }
            [DataMember]
            public decimal? ExiseTaxAmount { get; set; }
            [DataMember]
            public decimal? DiscountPercentage { get; set; }
            [DataMember]
            public decimal? DiscountAmount { get; set; }
            [DataMember]
            public decimal? InvoiceAmount { get; set; }
            [DataMember]
            public decimal? Freight_Amount { get; set; }
            [DataMember]
            public string Vehicle { get; set; }
            [DataMember]
            public int? InvoiceType { get; set; }
            [DataMember]
            public string CustomerCell { get; set; }
            [DataMember]
            public string Email_Address { get; set; }
            [DataMember]
            public string Sales_Tax_No { get; set; }
            [DataMember]
            public string NTN_No { get; set; }
            [DataMember]
            public Boolean Posted { get; set; }

            [DataMember]
            public string TransactionType { get; set; }

            [DataMember]
            public string AddorEdit { get; set; }

            [DataMember]
            public List<AR_Invoice_Items> InvoiceItems { get; set; }

        }

        public class AR_Invoice_Items
        {
            [DataMember]
            public int?  InvoiceItemID { get; set; }
            [DataMember]
            public int? InvoiceId { get; set; }
            [DataMember]
            public string ItemCode { get; set; }
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public decimal? IssueRate { get; set; }
            [DataMember]
            public decimal? RateEnt { get; set; }

            [DataMember]
            public decimal? Stock { get; set; }
            [DataMember]
            public decimal? Cost_Of_Sales { get; set; }
            [DataMember]
            public decimal? Qty { get; set; }
            [DataMember]
            public decimal? Rate { get; set; }
            [DataMember]
            public decimal? SaleRate { get; set; }
            [DataMember]
            public decimal? DiscountPercentage { get; set; }
            [DataMember]
            public decimal? DiscountAmount { get; set; }
            [DataMember]
            public decimal? Value { get; set; }

            [DataMember]
            public decimal? Net_Value { get; set; }

            [DataMember]
            public decimal? SalesTaxPercentage { get; set; }

            [DataMember]
            public decimal? SalesTaxAmount { get; set; }
            [DataMember]
            public decimal? ExiseTaxPercentage { get; set; }
            [DataMember]
            public decimal? ExiseTaxAmount { get; set; }

            [DataMember]
            public decimal? Sales_Order_Item_ID { get; set; }

        }
        public class GetItems
        {
            [DataMember]
            public string itemCode { get; set; }
            [DataMember]
            public int? wareHouse { get; set; }
            [DataMember]
            public DateTime invoiceDate { get; set; }
            [DataMember]
            public string type { get; set; }
        }
    }
}
