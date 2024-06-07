using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;

namespace Web.TagHelpers.Alerts;

public class DismissibleInfoAlert : TagHelper
{
    public async override void Process(TagHelperContext context, TagHelperOutput output)
    {
        TagBuilder container = new("div");

        container.AddCssClass("alert alert-info alert-dismissible fade show");

        container.Attributes.Add("role", "alert");

        string? childContent = (await output.GetChildContentAsync()).GetContent();
        if (!string.IsNullOrEmpty(childContent))
        {
            container.InnerHtml.AppendHtml(childContent);
        }

        container.InnerHtml.AppendHtml("""<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>""");

        output.Content.SetHtmlContent(container);
    }
}
