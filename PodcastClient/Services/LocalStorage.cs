using Microsoft.JSInterop;

namespace PodcastClient.Services
{
	public class LocalStorage
	{
		private readonly IJSRuntime _js;

		public LocalStorage(IJSRuntime js)
		{
			_js = js;
		}

		public async ValueTask SetItem(string key, string value)
		{
			await _js.InvokeVoidAsync("localStorage.setItem", key, value);
		}

		public async ValueTask<string?> GetItem(string key)
		{
			return await _js.InvokeAsync<string>("localStorage.getItem", key);
		}

		public async ValueTask RemoveItem(string key)
		{
			await _js.InvokeVoidAsync("localStorage.removeItem", key);
		}

		public async ValueTask Clear()
		{
			await _js.InvokeVoidAsync("localStorage.clear");
		}
	}
}
