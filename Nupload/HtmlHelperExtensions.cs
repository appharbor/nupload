using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Nupload
{
	public static class HtmlHelperExtensions
	{
		public static MvcForm BeginAsyncUploadForm<T>(this HtmlHelper<T> htmlHelper, Uri callbackUrl, IUploadConfiguration uploadConfiguration, object htmlAttributes = null)
		{
			var formAttributes = new Dictionary<string, string>
			{
				{ "data-post", callbackUrl.OriginalString },
				{ "enctype", "multipart/form-data" },
			}.Concat(uploadConfiguration.FormAttributes);

			var formTagBuilder = new TagBuilder("form");
			foreach (var attribute in formAttributes)
			{
				formTagBuilder.MergeAttribute(attribute.Key, attribute.Value);
			}

			if (htmlAttributes != null)
			{
				var htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
				formTagBuilder.MergeAttributes(htmlAttributeDictionary, replaceExisting: true);
			}

			var writer = htmlHelper.ViewContext.Writer;

			writer.Write(formTagBuilder.ToString(TagRenderMode.StartTag));

			foreach (var field in uploadConfiguration.FormFields)
			{
				writer.Write(htmlHelper.Hidden(field.Key, field.Value));
			}
			
			return new MvcForm(htmlHelper.ViewContext);
		}
	}
}
