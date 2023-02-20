using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorWasm.Helpers
{
    public static class ViewModelHelper
    {
        public static async Task<TModel?> GetViewModel<TModel>(this IJSRuntime jsRuntime)
        {
            var modelType = await jsRuntime.InvokeAsync<string>("getModelType");
            var modelJson = await jsRuntime.InvokeAsync<string>("getModelJson");
            if (modelType != null && modelJson != null)
            {
                return JsonSerializer.Deserialize<TModel>(modelJson);
            }

            return default;
        }
    }
}
