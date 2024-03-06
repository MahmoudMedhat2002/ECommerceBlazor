using ECommerceBlazor.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBlazor.Shared
{
	public class OrderItem
	{
        public Order Order { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Product Product { get; set; }
		[ForeignKey("Product")]
		public int ProductId { get; set; }
        public ProductType ProductType { get; set; }
		[ForeignKey("ProductType")]
		public int ProductTypeId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18 , 2)")]
        public decimal TotalPrice { get; set; }
    }
}
