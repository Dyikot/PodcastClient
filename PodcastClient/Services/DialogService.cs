using Microsoft.JSInterop;

namespace PodcastClient.Services
{
	public class DialogService
	{
		private readonly IJSRuntime _js;

		public DialogService(IJSRuntime js)
		{
			_js = js;
		}

		public async ValueTask Open(string id) => await _js.InvokeVoidAsync("OpenDialog", id);
		public async ValueTask Close(string id) => await _js.InvokeVoidAsync("CloseDialog", id);
	}
}
