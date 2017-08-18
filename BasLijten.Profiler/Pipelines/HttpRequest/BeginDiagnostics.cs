using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using Sitecore.Web;

namespace BasLijten.Profiler.Pipelines.HttpRequest
{
    public class BeginDiagnostics : HttpRequestProcessor
    {
        /// <summary>
        /// Runs the processor.
        /// </summary>
        /// <param name="args">The <c>args</c>.</param>
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            SiteContext site = Context.Site;
            if (site == null)
            {
                return;
            }
            if (!WebUtil.IsOnAspxPage() && !string.IsNullOrEmpty(WebUtil.GetRequestExtension(true)))
            {
                return;
            }
            //bool debugAllowed = BeginDiagnostics.GetDebugAllowed(site);
            //BeginDiagnostics.SetDisplayMode(debugAllowed, site);
            DiagnosticContext diagnostics = Context.Diagnostics;
            if (site.DisplayMode != DisplayMode.Edit && site.DisplayMode != DisplayMode.Preview)
            {
                BeginDiagnostics.SetSwitches(true, site, diagnostics);
            }
            BeginDiagnostics.TraceSettings(site, diagnostics);
        }


        /// <summary>
        /// Gets the switch value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="setCookie">if set to <c>true</c> this value is written to a cookie.</param>
        /// <param name="debugAllowed">if set to <c>true</c> [debug allowed].</param>
        /// <param name="site">The site.</param>
        /// <returns></returns>
        private static bool GetSwitch(string name, bool setCookie, bool debugAllowed, SiteContext site)
        {
            if (!debugAllowed)
            {
                return false;
            }
            string text = WebUtil.GetQueryString(name, null);
            if (text != null)
            {
                text = StringUtil.GetString(new string[]
                {
                    text,
                    "0"
                });
                if (setCookie && site != null)
                {
                    site.Response.SetCookie(name, text);
                }
            }
            else if (site != null)
            {
                text = site.Request.GetCookie(name);
            }
            return text == "1";
        }

        /// <summary>
        /// Sets the switches.
        /// </summary>
        /// <param name="debugAllowed">if set to <c>true</c> [debug allowed].</param>
        /// <param name="site">The site.</param>
        /// <param name="diagnostics">The diagnostics.</param>
        private static void SetSwitches(bool debugAllowed, SiteContext site, DiagnosticContext diagnostics)
        {
            diagnostics.Debugging = false;
            diagnostics.Profiling = true;
            diagnostics.Tracing = true;
            diagnostics.ShowRenderingInfo = false;
            diagnostics.DrawRenderingBorders = false;
            Tracer.RenderOnEndSession = true;
            Sitecore.Diagnostics.Profiler.RenderOnEndSession = true;
        }

        /// <summary>
        /// Traces the settings.
        /// </summary>
        private static void TraceSettings(SiteContext site, DiagnosticContext diagnostics)
        {
            if (!diagnostics.Tracing)
            {
                return;
            }
            string arg = (site != null) ? site.Name : string.Empty;
            Tracer.Info(string.Format("Current site is \"{0}\".", arg));
            Tracer.Info(string.Format("Current domain is \"{0}\".", Context.Domain.Name));
            Tracer.Info(string.Format("Current language is \"{0}\".", Context.Language));
            if (diagnostics.Profiling)
            {
                Tracer.Info("Profiling is active.");
            }
        }
    }
}