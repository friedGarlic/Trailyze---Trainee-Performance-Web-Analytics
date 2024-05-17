using Microsoft.AspNetCore.Mvc;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.Models.Models;
using System.Security.Claims;

namespace ML_ASP.Controllers
{
    public class RequirementFileController : Controller
    {
        public readonly IUnitOfWork _unit;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;

        public RequirementFileController(IUnitOfWork unit, IWebHostEnvironment environment)
        {
            _unit = unit;
            _environment = environment;

        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult SubmitDocument(IFormFile postedFiles0, IFormFile postedFiles1, IFormFile postedFiles2,
            string title, string title2, string title3,
            string description, string description2, string description3)
        {
            RequirementFile_Model reqFileModel = new RequirementFile_Model();
            RequirementFile_Model reqFileModel2 = new RequirementFile_Model();
            RequirementFile_Model reqFileModel3 = new RequirementFile_Model();

            string projectPath = _environment.WebRootPath;
            string uploadFolderName = "RequirementFiles";
            var uploads = Path.Combine(projectPath, uploadFolderName);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            //-----------------------------------POSTED FILE 0
            if (postedFiles0 != null && postedFiles0.Length > 0)
            {

                string fileId = Guid.NewGuid().ToString();

                var fileExtension = Path.GetExtension(postedFiles0.FileName);

                var fileName = postedFiles0.FileName;

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

                _unit.RequirementFile.Add(reqFileModel);
            }

            //-----------------------------------POSTED FILE 1
            if (postedFiles1 != null && postedFiles1.Length > 0)
            {
                string file2Id = Guid.NewGuid().ToString();

                var file2Extension = Path.GetExtension(postedFiles1.FileName);

                var fileName2 = postedFiles1.FileName;

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

                _unit.RequirementFile.Add(reqFileModel2);
            }

            //-----------------------------------POSTED FILE 2
            if (postedFiles2 != null && postedFiles2.Length > 0)
            {
                string file3Id = Guid.NewGuid().ToString();

                var file3Extension = Path.GetExtension(postedFiles2.FileName);

                var fileName3 = postedFiles2.FileName;

                string newFileId3 = file3Id + file3Extension; using (var fileStream = new FileStream(Path.Combine(uploads, file3Id + file3Extension), FileMode.Create))
                {
                    postedFiles2.CopyTo(fileStream);
                }

                reqFileModel2.FileName = fileName3;
                reqFileModel2.UserId = userId;
                reqFileModel2.FileId = newFileId3;
                reqFileModel2.Title = title3;
                reqFileModel2.Description = description3;

                _unit.RequirementFile.Add(reqFileModel3);
            }

            if (postedFiles0 != null || postedFiles1 != null || postedFiles2 != null)
            {
                _unit.Save();
            }
            return View(nameof(Index));
        }

        public IActionResult PermissionDenied()
        {
            return View();
        }
    }
}
