using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models.Models;
using ML_ASP.Models.ViewModel;
using ML_ASP.Utility;
using System.Security.Claims;

namespace ML_ASP.Controllers
{
    public class RequirementFileController : Controller
    {
        public readonly IUnitOfWork _unit;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public RequirementVM requirementVM { get; set; }

        public RequirementFileController(IUnitOfWork unit, IWebHostEnvironment environment)
        {
            _unit = unit;
            _environment = environment;

        }


        [Authorize(Roles = SD.Role_Unregistered)]
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //getall var
            var getAllFile = _unit.RequirementForm.GetAll(u => u.UserId == claim.Value);
            var userForm = _unit.RequirementForm.GetFirstOrDefault(u => u.UserId == claim.Value);

            string form1FileName = "";
            string form1FileName2 = "";
            string form1FileName3 = "";

            //needs foreach loop

            foreach (var i in getAllFile)
            {
                if (i.FormNumber == 1)
                {
                    form1FileName = userForm.FileId;
                }
                if (i.FormNumber == 2)
                {
                    form1FileName2 = userForm.FileId;
                }
                if (i.FormNumber == 3)
                {
                    form1FileName3 = userForm.FileId;
                }
            }


            requirementVM = new RequirementVM
            {
                FileName1 = form1FileName,
                FileName2 = form1FileName2,
                FileName3 = form1FileName3,
                IsSubmittedFile1 = true,
                IsSubmittedFile2 = true,
                IsSubmittedFile3 = true,
            };

            return View(requirementVM);
        }

        public IActionResult Step2(IFormFile postedFiles1, string title2, string description2)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            string projectPath = _environment.WebRootPath;
            string uploadFolderName = "RequirementFiles";
            var uploads = Path.Combine(projectPath, uploadFolderName);

            var userForm = _unit.RequirementForm.GetFirstOrDefault(u => u.UserId == userId);
            var userModel = _unit.Account.GetFirstOrDefault(x => x.Id == userId);
            var getAllForm = _unit.RequirementForm.GetAll(x => x.UserId == userId);

            string fileName2 = "";
            //-----------------------------------POSTED FILE 1
            if (postedFiles1 != null && postedFiles1.Length > 0)
            {
                string file2Id = Guid.NewGuid().ToString();

                var file2Extension = Path.GetExtension(postedFiles1.FileName);

                fileName2 = postedFiles1.FileName;

                string newFileId2 = file2Id + file2Extension;

                using (var fileStream = new FileStream(Path.Combine(uploads, file2Id + file2Extension), FileMode.Create))
                {
                    postedFiles1.CopyTo(fileStream);
                }

                TempData["fileName2"] = fileName2;
                TempData["userId2"] = userId;
                TempData["newfileId2"] = newFileId2;
                TempData["title2"] = title2;
                TempData["description2"] = description2;
                TempData["userName2"] = userModel.FullName;

                TempData["UF_filename2"] = fileName2;
                TempData["UF_isSubmitted2"] = true;
                TempData["UF_newFileId2"] = newFileId2;
            }
            string form1FileName = "";
            string form1FileName2 = "";
            string form1FileName3 = "";

            //needs foreach loop

            foreach (var i in getAllForm)
            {
                if (i.FormNumber == 2)
                {
                    form1FileName2 = userForm.FileId;
                }
            }


            requirementVM = new RequirementVM
            {
                FileName2 = form1FileName,
                IsSubmittedFile2 = true,
            };

            return View(nameof(Step2),requirementVM);
        }

        public IActionResult Step3(IFormFile postedFiles2, string title3, string description3)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string projectPath = _environment.WebRootPath;
            string uploadFolderName = "RequirementFiles";
            var uploads = Path.Combine(projectPath, uploadFolderName);

            var userId = claim.Value;
            var userModel = _unit.Account.GetFirstOrDefault(x => x.Id == userId);
            var userForm = _unit.RequirementForm.GetFirstOrDefault(u => u.UserId == userId);

            string fileName3 = "";

            //-----------------------------------POSTED FILE 2
            if (postedFiles2 != null && postedFiles2.Length > 0)
            {
                string file3Id = Guid.NewGuid().ToString();

                var file3Extension = Path.GetExtension(postedFiles2.FileName);

                fileName3 = postedFiles2.FileName;

                string newFileId3 = file3Id + file3Extension; using (var fileStream = new FileStream(Path.Combine(uploads, file3Id + file3Extension), FileMode.Create))
                {
                    postedFiles2.CopyTo(fileStream);
                }

                TempData["fileName3"] = fileName3;
                TempData["userId3"] = userId;
                TempData["newFileId3"] = newFileId3;
                TempData["title3"] = title3;
                TempData["description3"] = description3;
                TempData["userName3"] = userModel.FullName;

                TempData["UF_fileName3"] = fileName3;
                TempData["UF_isSubmitted3"] = true;
                TempData["UF_newFileId3"] = newFileId3;
            }

            return RedirectToAction("Index");
        }

        //==============================================
        [Authorize(Roles = SD.Role_Unregistered)]
        public ActionResult SubmitDocument(IFormFile postedFiles0, string title, string description)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string projectPath = _environment.WebRootPath;
            string uploadFolderName = "RequirementFiles";
            var uploads = Path.Combine(projectPath, uploadFolderName);

            var userId = claim.Value;
            var userModel = _unit.Account.GetFirstOrDefault(x => x.Id == userId);
            var getAllFile = _unit.RequirementFile.GetAll(x => x.UserId == userId);

            string fileName = "";

            //TODO add function in repository of Requirement Form for updating the properties.
            //-----------------------------------POSTED FILE 0
            if (postedFiles0 != null && postedFiles0.Length > 0)
            {
                string fileId = Guid.NewGuid().ToString();

                var fileExtension = Path.GetExtension(postedFiles0.FileName);

                fileName = postedFiles0.FileName;

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                string newFileId = fileId + fileExtension;

                using (var fileStream = new FileStream(Path.Combine(uploads, fileId + fileExtension), FileMode.Create))
                {
                    postedFiles0.CopyTo(fileStream);
                }

                TempData["fileName"] = fileName;
                TempData["userId"] = userId;
                TempData["newFileId"] = newFileId;
                TempData["title"] = title;
                TempData["description"] = description;
                TempData["userName"] = userModel.FullName;

                //----flagging the form

                TempData["UF_fileName"] = fileName;
                TempData["UF_isSubmitted"] = true;
                TempData["UF_fileId"] = newFileId;
            }

            //--------------------------------------------------------------------------

            //string form1FileName = "";
            //string form1FileName2 = "";
            //string form1FileName3 = "";

            ////needs foreach loop

            //foreach (var i in getAllFile)
            //{
            //    if (i.FormNumber == 1)
            //    {
            //        form1FileName = userForm.FileId;
            //    }
            //    if (i.FormNumber == 2)
            //    {
            //        form1FileName2 = userForm.FileId;
            //    }
            //    if (i.FormNumber == 3)
            //    {
            //        form1FileName3 = userForm.FileId;
            //    }
            //}


            //requirementVM = new RequirementVM
            //{
            //    FileName1 = form1FileName,
            //    IsSubmittedFile1 = true,
            //};
            //--------------------------------------------------------------------------

            //if (postedFiles0 != null || postedFiles1 != null || postedFiles2 != null)
            //{
            //    _unit.Save();
            //}

            //TODO submit overall file is not yet done, for submitting to admin as permission for full registration to look for document sent using this form.

            return RedirectToAction(nameof(Step2));
        }


        //------------------------------------------
        //helper METHODS ---------------------------
        public ActionResult ViewImage(string fileName)
        {
            if (fileName != null)
            {
                string path = Path.Combine(_environment.WebRootPath + "\\RequirementFiles", fileName);
                string contentType = GetContentType(fileName);

                if (System.IO.File.Exists(path))
                {
                    return File(System.IO.File.ReadAllBytes(path), contentType);
                }
                else
                {
                    TempData["failed"] = "File Not Found";
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        public ActionResult ViewPdf(string fileName)
        {
            string path = Path.Combine(_environment.WebRootPath + "\\RequirementFiles", fileName);

            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "application/pdf");
            }
            else
            {
                TempData["failed"] = "File Not Found";
                return NotFound();
            }
        }

        public ActionResult ViewTemplatePdf(string fileName)
        {
            string path = Path.Combine(_environment.WebRootPath + "\\RequirementFiles" + "\\TemplateFiles", fileName);

            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "application/pdf");
            }
            else
            {
                TempData["failed"] = "File Not Found";
                return NotFound();
            }
        }

        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                // add more cases for other image formats as needed
                default:
                    return "application/octet-stream";
            }
        }

        public IActionResult PermissionDenied()
        {
            return View();
        }
	}
}
