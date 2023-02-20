using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCFx.Helpers
{
    public static class ViewModelHelper
    {
        public static HtmlString ViewModel(this HtmlHelper htmlHelper, object model)
        {
            var id = "app-view-model";
            return htmlHelper.Hidden(
                id,
                JsonSerializer.Serialize(model),
                new { id, modeltype = model.GetType().FullName });
        }
    }
}