using Microsoft.JSInterop;
using System.Text.Json;

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

		public async ValueTask SetItem<T>(string key, T value)
		{
			await SetItem(key, JsonSerializer.Serialize(value));
		}

		public async ValueTask<string?> GetItem(string key)
		{
			return await _js.InvokeAsync<string>("localStorage.getItem", key);
		}

		public async ValueTask<T?> GetItem<T>(string key)
		{
			var item = await GetItem(key);

			if(item == null)
			{
				return default;
			}

			return JsonSerializer.Deserialize<T>(item);
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
