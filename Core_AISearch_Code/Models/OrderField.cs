using Azure.Search.Documents.Indexes;

namespace Core_AISearch_Code.Models
{
    public class ShippingIndex
    {
        [SimpleField(IsKey = true)]
        public string OrderID { get; set; }

        [SearchableField(IsFilterable = true)]
        public string? CustomerName { get; set; }

        [SearchableField(IsFilterable = true)]
        public string? EmployeeName { get; set; }

        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }

        [SearchableField(IsFilterable = true)]
        public string? ShipperName { get; set; }

        [FieldBuilderIgnore]
        public decimal? Freight { get; set; } // Manually define in SearchField

        [SearchableField(IsFilterable = true)]
        public string? ShipName { get; set; }

        public string? ShipAddress { get; set; }

        [SearchableField(IsFilterable = true)]
        public string? ShipCity { get; set; }

        public string? ShipPostalCode { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string? ShipCountry { get; set; }
    }

}
