using EcommerceBlazor.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBlazor.Shared
{
    public class Product : IEntityBase
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public List<Image> Images { get; set; } = new List<Image>();

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public bool Featured { get; set; } = false;
        public Category? Category { get; set; }
		public bool Visible { get; set; } = true;
		public bool Deleted { get; set; } = false;
		[NotMapped]
		public bool Editing { get; set; } = false;
		[NotMapped]
		public bool IsNew { get; set; } = false;
		public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
