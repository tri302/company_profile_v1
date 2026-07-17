using company_profile.Models;
using Microsoft.Extensions.Options;

namespace company_profile.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<AppSettings> _settings;

        public MaintenanceMiddleware(RequestDelegate next, IOptionsMonitor<AppSettings> settings)
        {
            _next = next;
            _settings = settings;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Cek apakah user sudah login dan punya akses admin
            if (context.User.Identity.IsAuthenticated && context.User.IsInRole("Admin"))
            {
                await _next(context); // Admin selalu punya akses
                return;
            }

            // Jika bukan admin dan maintenance aktif, arahkan ke maintenance
            if (_settings.CurrentValue.IsMaintenanceMode)
            {
                // Izinkan akses file statis agar tampilan tetap oke
                if (context.Request.Path.StartsWithSegments("/lib") ||
                    context.Request.Path.StartsWithSegments("/css") ||
                    context.Request.Path.StartsWithSegments("/js"))
                {
                    await _next(context);
                    return;
                }

                if (context.Request.Path.StartsWithSegments("/Maintenance"))
                {
                    await _next(context);
                    return;
                }

                context.Response.Redirect("/Maintenance/Index");
                return;
            }

            await _next(context);            
        }    
    }
}
