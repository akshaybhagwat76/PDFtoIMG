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

        //public void Pdf2ImageConversion(string FileName, string PdfFolderPath)
        //{
        //    String FileNamewithoutExtension = System.IO.Path.GetFileNameWithoutExtension(FileName);
        //    String ImgFolderPath = System.Web.Hosting.HostingEnvironment.MapPath("-/ImgFolder1" + FileNamewithoutExtension + ".png");
        //    var info = new System.IO.FileInfo(ImgFolderPath);
        //    if (info.Exists.Equals(false))
        //    {

        //        GhostscriptPngDevice img = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png16m);
        //        img.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
        //        img.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
        //        img.ResolutionXY = new GhostscriptImageDeviceResolution(200, 200);
        //        img.InputFiles.Add(PdfFolderPath);
        //        img.Pdf.FirstPage = 1;
        //        img.Pdf.LastPage = 1;
        //        img.PostScript = string.Empty;
        //        img.OutputPath = ImgFolderPath;
        //        img.Process();
        //    }
        //}

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
                    using (var converter= new Aspose.Pdf.Facades.PdfConverter())
                    {
                        converter.BindPdf(FolderPath);
                        converter.DoConvert();
                        var settings = new Aspose.Pdf.Devices.JpegDevice()
                        {
                            Compression = Aspose.Pdf.Devices.CompressionType.RLE,
                        };
                    }
                    doc.Save(Server.MapPath("~/PdfFolder/") + "1" + ".pdf");
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