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
            var userID = _unit.RequirementForm.GetFirstOrDefault(u => u.UserId == claim.Value);

            string form1FileName = "";
            string form1FileName2 = "";
            string form1FileName3 = "";

            //needs foreach loop

            foreach (var i in getAllFile)
            {
                if (i.FormNumber == 1)
                {
                    form1FileName = userID.FileId;
                }
                if (i.FormNumber == 2)
                {
                    form1FileName2 = userID.FileId;
                }
                if (i.FormNumber == 3)
                {
                    form1FileName3 = userID.FileId;
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

        [Authorize(Roles = SD.Role_Unregistered)]
        public ActionResult SubmitDocument(IFormFile postedFiles0, IFormFile postedFiles1, IFormFile postedFiles2,
            string title, string title2, string title3,
            string description, string description2, string description3)
        {
            RequirementFile_Model reqFileModel = new RequirementFile_Model();
            RequirementFile_Model reqFileModel2 = new RequirementFile_Model();
            RequirementFile_Model reqFileModel3 = new RequirementFile_Model();

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string projectPath = _environment.WebRootPath;
            string uploadFolderName = "RequirementFiles";
            var uploads = Path.Combine(projectPath, uploadFolderName);

            var userId = claim.Value;
            var userModel = _unit.Account.GetFirstOrDefault(x => x.Id == userId);
            var userForm = _unit.RequirementForm.GetFirstOrDefault(u => u.UserId == userId);

            string fileName = "";
            string fileName2 = "";
            string fileName3 = "";

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


                reqFileModel.FileName = fileName;
                reqFileModel.UserId = userId;
                reqFileModel.FileId = newFileId;
                reqFileModel.Title = title;
                reqFileModel.Description = description;
                reqFileModel.UserName = userModel.FullName;

                //----flagging the form

                userForm.FileName = fileName;
                userForm.IsSubmitted = true;
                userForm.FileId = newFileId;

                _unit.RequirementFile.Add(reqFileModel);
            }

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

                reqFileModel2.FileName = fileName2;
                reqFileModel2.UserId = userId;
                reqFileModel2.FileId = newFileId2;
                reqFileModel2.Title = title2;
                reqFileModel2.Description = description2;
				reqFileModel2.UserName = userModel.FullName;

				userForm.FileName = fileName2;
                userForm.IsSubmitted = true;
                userForm.FileId = newFileId2;

                _unit.RequirementFile.Add(reqFileModel2);
            }

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

                reqFileModel2.FileName = fileName3;
                reqFileModel2.UserId = userId;
                reqFileModel2.FileId = newFileId3;
                reqFileModel2.Title = title3;
                reqFileModel2.Description = description3;
				reqFileModel3.UserName = userModel.FullName;

				userForm.FileName = fileName3;
                userForm.IsSubmitted = true;
                userForm.FileId = newFileId3;

                _unit.RequirementFile.Add(reqFileModel3);
            }

            if (postedFiles0 != null || postedFiles1 != null || postedFiles2 != null)
            {
                _unit.Save();
            }

            //--------------------------------------------------------------------------
            var getAllFile = _unit.RequirementForm.GetAll(u => u.UserId == claim.Value);
            var userID = _unit.RequirementForm.GetFirstOrDefault(u => u.UserId == claim.Value);

            string form1FileName = "";
            string form1FileName2 = "";
            string form1FileName3 = "";

            //needs foreach loop

            foreach (var i in getAllFile)
            {
                if (i.FormNumber == 1)
                {
                    form1FileName = userID.FileId;
                }
                if (i.FormNumber == 2)
                {
                    form1FileName2 = userID.FileId;
                }
                if (i.FormNumber == 3)
                {
                    form1FileName3 = userID.FileId;
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
            //--------------------------------------------------------------------------

            //TODO submit overall file is not yet done, for submitting to admin as permission for full registration to look for document sent using this form.

            return View(nameof(Index), requirementVM);
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
