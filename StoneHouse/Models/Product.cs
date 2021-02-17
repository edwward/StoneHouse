using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    public class Products
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public bool Available { get; set; }

        public string Image { get; set; }       //nebudeme ukladate image samotny ale pouze cestu k nemu

        public string ShadeColor { get; set; }

        [Display(Name = "Product Type")]        //toto je definice foreign key, slouzi jako odkaz na jinou tabulku v Db
        public int ProductTypeId { get; set; }

        [ForeignKey("ProductTypeId")]       //zde foreign key pouzivam
        [Display(Name = "Product Type")]
        public virtual ProductTypes ProductTypes { get; set; }  //virtual property pro ProductTypes (jina trida/model), virtual p. nebude pridana do Db, je pouze jako odkaz pro foreign key

        [Display(Name = "Special Tag")]     //to same jako vyse, akorat jiny foreign key
        public int SpecialTagsID { get; set; }

        [ForeignKey("SpecialTagsID")]
        [Display(Name = "Special Tag")]
        public virtual SpecialTags SpecialTags { get; set; }
    }
}
