using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Specialized;
using System.Web;

namespace PodcastClient.Components.Controls
{
	public partial class PageNavigationControl
	{
		private UriBuilder _uriBuilder = default!;
		private NameValueCollection _query = default!;

		[Parameter]
		public string Class { get; set; } = string.Empty;

		[Parameter, EditorRequired]
		public int ButtonsAmount { get; set; }

		[Parameter, EditorRequired]
		public int Page { get; set; }

		[Parameter, EditorRequired]
		public int PageAmount { get; set; }

		private int Start { get; set; } = 1;
		private int Count => Math.Min(PageAmount, ButtonsAmount);
		private string FirstPageUri => GetPageUri(1);
		private string NextPageUri => GetPageUri(Page + 1);

		protected override void OnInitialized()
		{
			_uriBuilder = new UriBuilder(NavigationManager.Uri);
			_query = HttpUtility.ParseQueryString(_uriBuilder.Query);
			_query.Add("page", Page.ToString());
		}

		protected override void OnParametersSet()
		{
			var middle = (int)Math.Ceiling(ButtonsAmount / 2.0);
			
			if(Page < middle || PageAmount < ButtonsAmount)
			{
				Start = 1;
			}
			else if(Page > PageAmount - middle)
			{
				Start = PageAmount - ButtonsAmount + 1;
			}
			else
			{
				Start = Page - middle + 1;
			}
		}

		private RenderFragment PageNumberView(int pageNumber)
		{
			return builder =>
			{
				if(pageNumber == Page)
				{
					builder.OpenElement(0, "li");
					builder.AddAttribute(1, "class", "page-item active");
					builder.OpenElement(2, "a");
					builder.AddAttribute(3, "class", "page-link");
					builder.AddAttribute(4, "href", GetPageUri(pageNumber));
					builder.AddAttribute(5, "aria-current", "page");
					builder.AddContent(6, pageNumber.ToString());
					builder.CloseElement();
					builder.CloseElement();
				}
				else
				{
					builder.OpenElement(0, "li");
					builder.AddAttribute(1, "class", "page-item");
					builder.OpenElement(2, "a");
					builder.AddAttribute(3, "class", "page-link");
					builder.AddAttribute(4, "href", GetPageUri(pageNumber));
					builder.AddContent(5, pageNumber.ToString());
					builder.CloseElement();
					builder.CloseElement();
				}
			};
		}

		private string GetPageUri(int page)
		{
			_query.Set("page", page.ToString());
			_uriBuilder.Query = _query.ToString();
			return _uriBuilder.ToString();
		}
	}
}
