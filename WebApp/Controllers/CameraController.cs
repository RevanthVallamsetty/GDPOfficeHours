using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebCam.Controllers
{
    public class CameraController : Controller
    {
        private readonly HostingEnvironment _environment;

        public CameraController(HostingEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment;
        }

        [HttpGet]
        public ActionResult Capture()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Capture(string name)
        {
            try
            {
                var files = HttpContext.Request.Form.;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Getting Filename
                            var fileName = file.FileName;
                            // Unique filename "Guid"
                            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            // Getting Extension
                            var fileExtension = Path.GetExtension(fileName);
                            // Concating filename + fileExtension (unique filename)
                            var newFileName = string.Concat(myUniqueFileName, fileExtension);
                            //  Generating Path to store photo 
                            var filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

                            if (!string.IsNullOrEmpty(filepath))
                            {
                                // Storing Image in Folder
                                StoreInFolder(file, filepath);
                            }

                            var imageBytes = System.IO.File.ReadAllBytes(filepath);
                            if (imageBytes != null)
                            {
                                // Storing Image in Folder
                                //StoreInDatabase(imageBytes);
                            }

                        }
                    }
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Saving captured image into Folder.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        private void StoreInFolder(IFormFile file, string fileName)
        {
            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }        
    }
}