using System.Web;
using System.Web.Optimization;

namespace HealthSolution
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.mask.js",
                        "~/Scripts/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/util").Include(
                "~/Scripts/Util.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/open-iconic-bootstrap.css",
                      "~/Content/open-iconic.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/tingle.css"));

            bundles.Add(new ScriptBundle("~/consulta").Include("~/Scripts/health-solution/consultas.js"));
            bundles.Add(new ScriptBundle("~/consultaEdit").Include("~/Scripts/health-solution/consultasEdit.js"));
            bundles.Add(new ScriptBundle("~/intervencoes").Include("~/Scripts/health-solution/intervencoes.js"));
            bundles.Add(new ScriptBundle("~/intervencaoEdit").Include("~/Scripts/health-solution/intervencaoEdit.js"));
            bundles.Add(new ScriptBundle("~/home").Include("~/Scripts/health-solution/home.js"));
            bundles.Add(new ScriptBundle("~/prontuario").Include("~/Scripts/health-solution/prontuario.js"));
            bundles.Add(new ScriptBundle("~/atendimento").Include("~/Scripts/health-solution/atendimento.js"));
            bundles.Add(new ScriptBundle("~/print").Include("~/Scripts/print.min.js", "~/Scripts/jspdf.debug.js", "~/Scripts/canvas.js", "~/Scripts/canvas.js", "~/Scripts/pdfmake.min.js", "~/Scripts/vfs_fonts.js"));
            bundles.Add(new ScriptBundle("~/modal").Include("~/Scripts/tingle.js"));
            bundles.Add(new StyleBundle("~/printcss").Include("~/Scripts/print.min.css"));
        }
    }
}
