//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace E_Commerce.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ProductSizes = new HashSet<ProductSize>();
            this.Reviews = new HashSet<Review>();
        }
    
        public int Product_id { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageFile { get; set; }
        public decimal Cost { get; set; }
        public Nullable<int> SaleID { get; set; }
        public Nullable<int> FinalSubCategoryID { get; set; }
        public string Product_name { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public string SizeCategory { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<int> OnHand { get; set; }
    
        public virtual FinalSubCategory FinalSubCategory { get; set; }
        public virtual MainCategory MainCategory { get; set; }
        public virtual Sale Sale { get; set; }
        public virtual SubCatetory SubCatetory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductSize> ProductSizes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
