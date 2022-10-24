using System;
using System.Web.Mvc;
// [C# code]
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MVC_ViewToPDF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConvertToPDF()
        {
            //Getting Index view page as HTML
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, "Index", "");
            string html = GetHtmlFromView(ControllerContext, viewResult, "Index", "");
            string baseUrl = string.Empty;

            //Convert the HTML string to PDF using WebKit
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

            WebKitConverterSettings settings = new WebKitConverterSettings();
            
            //Assign WebKit settings to HTML converter
            htmlConverter.ConverterSettings = settings;

            //Convert HTML string to PDF
            PdfDocument document = htmlConverter.Convert(html, baseUrl);

            MemoryStream stream = new MemoryStream();

            //Save and close the PDF document 
            document.Save(stream);
            document.Close(true);

            return File(stream.ToArray(), "application/pdf", "ViewAsPdf.pdf");
        }

        private string GetHtmlFromView(ControllerContext context, ViewEngineResult viewResult, string viewName, object model)
        {
            context.Controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                // view not found, throw an exception with searched locations
                if (viewResult.View == null)
                {
                    var locations = new StringBuilder();
                    locations.AppendLine();

                    foreach (string location in viewResult.SearchedLocations)
                    {
                        locations.AppendLine(location);
                    }

                    throw new InvalidOperationException(
                        string.Format(
                            "The view '{0}' or its master was not found, searched locations: {1}", viewName, locations));
                }

                ViewContext viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                string html = sw.GetStringBuilder().ToString();
                string baseUrl = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);
                html = Regex.Replace(html, "<head>", string.Format("<head><base href=\"{0}\" />", baseUrl), RegexOptions.IgnoreCase);
                return html;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}