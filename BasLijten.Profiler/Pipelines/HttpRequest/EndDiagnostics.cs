using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;

namespace BasLijten.Profiler.Pipelines.HttpRequest
{
    public class EndDiagnostics : HttpRequestProcessor
    {
        /// <summary>
        /// Runs the processor.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (StringUtil.GetString(args.Context.Items["abort"]) == "1")
            {
                args.Context.Response.End();
            }
            if (!WebUtil.IsOnAspxPage() && !string.IsNullOrEmpty(WebUtil.GetRequestExtension(true)))
            {
                return;
            }
            Tracer.Info("Ending diagnostics.");
            DiagnosticContext diagnostics = Context.Diagnostics;
            diagnostics.Profiling = false;
            diagnostics.Tracing = false;
        }
    }
}