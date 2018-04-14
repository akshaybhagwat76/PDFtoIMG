using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ghostscript;
using Ghostscript.NET;
using Aspose.Pdf.Devices;
using Aspose.Pdf;
using Bytescout.PDFRenderer;
using System.Drawing;

namespace PDFtoIMG.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ImageUpload()
        {
            var file = Request.Files["imageFile"];
            try
            {
                var uploadpath = "";
                if (file != null)
                {
                    var fname = Path.GetFileName(file.FileName);
                    var ext = Path.GetExtension(file.FileName);
                    var path = System.Web.Hosting.HostingEnvironment.MapPath("~/MyFiles/" + fname);

                    //var fileName1 = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ext;
                    //uploadpath = string.Format("{0}\\{1}", path.ToString(), fileName1);

                    var without = Path.GetFileNameWithoutExtension(file.FileName);
                    file.SaveAs(path);
                    TempData["uploadPath"] = path;
                    TempData["fname"] = fname;
                    return Json(file.FileName, JsonRequestBehavior.AllowGet);
                    //Pdf2ImageConversion(fname, path);
                }
                return Json("ok");
            }
            catch (Exception ex)
            {

            }
            return Json(file.FileName, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AsposeConverter()
        {
            try
            {
                var FolderPath = TempData["uploadPath"].ToString();
                if (FolderPath.EndsWith(".pdf"))
                {
                    Document pdfDocument = new Document(FolderPath);

                    for (int pageCount = 1; pageCount <= pdfDocument.Pages.Count; pageCount++)
                    {
                        using (FileStream imageStream = new FileStream(Server.MapPath("~/ImgFolder1/") + "image" + pageCount + ".jpg", FileMode.Create))
                        {
                            Resolution resolution = new Resolution(300);
                            JpegDevice jpegDevice = new JpegDevice(resolution, 100);
                            jpegDevice.Process(pdfDocument.Pages[pageCount], imageStream);
                            imageStream.Close();
                        }
                    }
                }
               else
                {
                    //Document imgDocument = new Document(FolderPath);
                    Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document();
                    pdfDocument.Pages.Add();
                    int lowerLeftX = 100;
                    int lowerLeftY = 100;

                    int upperRightX = 200;
                    int upperRightY = 200;
                    Page page = pdfDocument.Pages[1];
                    FileStream imageStream = new FileStream(FolderPath, FileMode.Open);
                    page.Resources.Images.Add(imageStream);
                    page.Contents.Add(new Operator.GSave());
                    Aspose.Pdf.Rectangle rectangle = new Aspose.Pdf.Rectangle(lowerLeftX, lowerLeftY, upperRightX, upperRightY);
                    Aspose.Pdf.DOM.Matrix matrix = new Aspose.Pdf.DOM.Matrix(new double[] { rectangle.URX - rectangle.LLX, 0, 0, rectangle.URY - rectangle.LLY, rectangle.LLX, rectangle.LLY });
                    page.Contents.Add(new Operator.ConcatenateMatrix(matrix));
                    XImage ximage = page.Resources.Images[page.Resources.Images.Count];
                    page.Contents.Add(new Operator.Do(ximage.Name));
                    page.Contents.Add(new Operator.GRestore());
                    //var fileName1 = DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ext;
                    //uploadpath = string.Format("{0}\\{1}", path.ToString(), fileName1);
                    pdfDocument.Save(Server.MapPath("~/PdfFolder/") + "Image -In-PDF.pdf");
                }

            }
            catch (Exception ex)
            {

            }

            return View();
        }

    }
}
public class FileUploadModel

{

    public string FileName { get; set; }

    public string FilePath { get; set; }

}