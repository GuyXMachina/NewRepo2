﻿using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UFSFacilityManagement.Infrastructure
{
    [HtmlTargetElement("button", Attributes ="[type=submit]")]
    public class ButtonTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class", "btn btn-danger");
        }
    }
}