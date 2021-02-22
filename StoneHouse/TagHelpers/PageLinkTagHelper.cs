using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StoneHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]  //target pro tento taghelper
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory; //kazda stranka v paginaci bude mit url, proto potrebujeme urlhelperfactory

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } //viewkontext poskytuje ruzne podpurne http metody

        public PagingInfo PageModel { get; set; }  //viz PageInfo class
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext); //get url for urlhelper
            TagBuilder result = new TagBuilder("div");  //taghelper bude pro div

            for (int i = 1; i <= PageModel.totalPage; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.urlParam.Replace(":", i.ToString()); //build url for taghelper, replace ":" with "i" value from loop
                tag.Attributes["href"] = url;
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal); //aplikuj css styling na zaklade aktualne vybrane strance
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml); //pridej k outputu vsechno html, ktere se vytvorilo vyse
        }

    }
}
