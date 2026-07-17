namespace company_profile.Helper
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;    

    public class SanitizeHtmlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sanitizer = new HtmlSanitizer();

            // Konfigurasi whitelist jika perlu
            sanitizer.AllowedTags.Add("iframe");
            sanitizer.AllowedAttributes.Add("src");
            sanitizer.AllowedAttributes.Add("frameborder");
            sanitizer.AllowedAttributes.Add("allowfullscreen");

            // Loop melalui semua parameter di action
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is string content)
                {
                    // Bersihkan konten dan ganti nilai argumennya
                    context.ActionArguments[argument.Key] = sanitizer.Sanitize(content);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
