using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models.ViewModel
{
    //ViewModel je View, ktery do sebe kombinuje vice modelu - zde Products, ProductTypes in SpecialTags
    public class ProductsViewModel
    {
        public Products Products { get; set; }
        public IEnumerable<ProductTypes> ProductTypes { get; set; }
        public IEnumerable<SpecialTags> SpecialTags { get; set; }

    }
}
