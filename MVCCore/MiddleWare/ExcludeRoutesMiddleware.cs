namespace MVCCore.MiddleWare
{
    public class ExcludeRoutesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<PathString> _excludedRoutes;

        public ExcludeRoutesMiddleware(RequestDelegate next, IEnumerable<string> excludedRoutes)
        {
            _next = next;
            _excludedRoutes = excludedRoutes.Select(r => new PathString(r));
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the request path matches an excluded route
            if (_excludedRoutes.Any(r => context.Request.Path.StartsWithSegments(r)))
            {
                // Skip the YARP pipeline and return the response immediately
                return;
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
