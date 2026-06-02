using EMS.Models;
using EMS.Services.Interfaces;
using EMS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Controllers
{
    public class EmployeeDocumentsController : Controller
    {
        private readonly IEmployeeDocumentService _employeeDocumentService;

        public EmployeeDocumentsController(IEmployeeDocumentService employeeDocumentService)
        {
            _employeeDocumentService = employeeDocumentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int employeeId, CancellationToken ct)
        {
            employeeId = 1;
            var documents = await _employeeDocumentService.GetDocumentsAsync(employeeId, ct);
            return View(documents);
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            ViewBag.DocumentTypes = Enum.GetValues<DocumentType>().Select(dt => new SelectListItem
            {
                Value = dt.ToString(),
                Text = dt.ToString()
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadDocumentViewModel viewModel, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DocumentTypes = Enum.GetValues<DocumentType>().Select(dt => new SelectListItem
                {
                    Value = dt.ToString(),
                    Text = dt.ToString()
                }).ToList();
                return View(viewModel);
            }
            await _employeeDocumentService.UploadDocumentAsync(viewModel, ct);
            return RedirectToAction(nameof(Upload));
        }
    }
}
