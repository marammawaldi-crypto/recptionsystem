
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;
using ReceptionSystem.Models;
using ReceptionSystem.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReceptionSystem.Controllers
{
    // =====================================================
    // Authentication Required
    // =====================================================

    [Authorize]
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly JobApplicationNumberGenerator _numberGenerator;

        public JobApplicationsController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            JobApplicationNumberGenerator numberGenerator)
        {
            _context = context;
            _env = env;
            _numberGenerator = numberGenerator;
        }

        // =====================================================
        // GET: /JobApplications
        // =====================================================

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.JobApplications.ToListAsync()
            );
        }

        // =====================================================
        // GET: /JobApplications/Details/5
        // =====================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication =
                await _context.JobApplications
                    .FirstOrDefaultAsync(j => j.Id == id);

            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // =====================================================
        // GET: /JobApplications/Create
        // =====================================================

        public async Task<IActionResult> Create()
        {
            ViewBag.DrivingLicenseTypes =
                await _context.DrivingLicenseTypes
                    .OrderBy(x => x.Category)
                    .ToListAsync();

            return View(new JobApplication());
        }

        // =====================================================
        // POST: /JobApplications/Create
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            JobApplication jobApplication,
            IFormFile? CvFile)
        {
            // -------------------------------------------------
            // Remove empty Qualifications
            // -------------------------------------------------

            jobApplication.Qualifications =
                jobApplication.Qualifications?
                    .Where(q =>
                        !string.IsNullOrWhiteSpace(q.Degree)
                        || !string.IsNullOrWhiteSpace(q.Specialization)
                        || !string.IsNullOrWhiteSpace(q.University)
                        || !string.IsNullOrWhiteSpace(q.GraduationYear))
                    .ToList()
                    ?? new List<Qualification>();

            // -------------------------------------------------
            // Remove empty Courses
            // -------------------------------------------------

            jobApplication.Courses =
                jobApplication.Courses?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.CourseName)
                        || !string.IsNullOrWhiteSpace(c.Organization)
                        || !string.IsNullOrWhiteSpace(c.Duration))
                    .ToList()
                    ?? new List<Course>();

            // -------------------------------------------------
            // Remove empty Experiences
            // -------------------------------------------------

            jobApplication.Experiences =
                jobApplication.Experiences?
                    .Where(e =>
                        !string.IsNullOrWhiteSpace(e.CompanyName)
                        || !string.IsNullOrWhiteSpace(e.JobTitle)
                        || e.FromDate != default
                        || e.ToDate.HasValue
                        || !string.IsNullOrWhiteSpace(e.LastSalary))
                    .ToList()
                    ?? new List<Experience>();

            // -------------------------------------------------
            // Remove empty Computer Skills
            // -------------------------------------------------

            jobApplication.ComputerSkills =
                jobApplication.ComputerSkills?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.SkillName))
                    .ToList()
                    ?? new List<ComputerSkill>();

            // -------------------------------------------------
            // Remove empty Languages
            // -------------------------------------------------

            jobApplication.Languages =
                jobApplication.Languages?
                    .Where(l =>
                        !string.IsNullOrWhiteSpace(l.LanguageName)
                        || !string.IsNullOrWhiteSpace(l.Speaking)
                        || !string.IsNullOrWhiteSpace(l.Writing))
                    .ToList()
                    ?? new List<Language>();

            // -------------------------------------------------
            // Remove validation errors from optional sections
            // -------------------------------------------------

            var keysToRemove = ModelState.Keys
                .Where(k =>
                    k.StartsWith("Qualifications[")
                    || k.StartsWith("Courses[")
                    || k.StartsWith("Experiences[")
                    || k.StartsWith("ComputerSkills[")
                    || k.StartsWith("Languages["))
                .ToList();

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

            // -------------------------------------------------
            // Validate
            // -------------------------------------------------

            if (!ModelState.IsValid)
            {
                ViewBag.DrivingLicenseTypes =
                    await _context.DrivingLicenseTypes
                        .OrderBy(x => x.Category)
                        .ToListAsync();

                return View(jobApplication);
            }

            // -------------------------------------------------
            // Application Date
            // -------------------------------------------------

            jobApplication.ApplicationDate = DateTime.Now;

            // =================================================
            // SAVE CV
            // =================================================

            if (CvFile != null && CvFile.Length > 0)
            {
                var allowedExtensions = new[]
                {
                    ".pdf",
                    ".doc",
                    ".docx"
                };

                var extension =
                    Path.GetExtension(CvFile.FileName)
                        .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.DrivingLicenseTypes =
                        await _context.DrivingLicenseTypes
                            .OrderBy(x => x.Category)
                            .ToListAsync();

                    ModelState.AddModelError(
                        "CvFile",
                        "Only PDF, DOC and DOCX files are allowed."
                    );

                    return View(jobApplication);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await CvFile.CopyToAsync(memoryStream);

                    jobApplication.CvFileData =
                        memoryStream.ToArray();
                }

                jobApplication.CvFileName =
                    Path.GetFileName(CvFile.FileName);

                jobApplication.CvContentType =
                    CvFile.ContentType;
            }

            // -------------------------------------------------
<<<<<<< HEAD
=======
            // Assign a sequential application number (from DB sequence)
            // Format: DAMA/HR/{n}
            // -------------------------------------------------

            jobApplication.ApplicationNumber =
                await _numberGenerator.GenerateNextNumberAsync("HR");


            // -------------------------------------------------
>>>>>>> main
            // Save Job Application
            // -------------------------------------------------

            _context.JobApplications.Add(jobApplication);

            await _context.SaveChangesAsync();

<<<<<<< HEAD
            // -------------------------------------------------
            // Assign Application Number
            // Format: DAMA/HR/{Id}
            // -------------------------------------------------

            jobApplication.ApplicationNumber =
                $"DAMA/HR/{jobApplication.Id}";

            _context.JobApplications.Update(jobApplication);

            await _context.SaveChangesAsync();
=======
>>>>>>> main

            // -------------------------------------------------
            // Success
            // -------------------------------------------------

            TempData["SuccessMessage"] =
                "Job application saved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GET: Download CV
        // =====================================================

        public async Task<IActionResult> DownloadCv(int id)
        {
            var job =
                await _context.JobApplications.FindAsync(id);

            if (job == null ||
                job.CvFileData == null ||
                job.CvFileData.Length == 0)
            {
                return NotFound();
            }

            var fileName =
                string.IsNullOrEmpty(job.CvFileName)
                    ? $"cv_{job.Id}.pdf"
                    : job.CvFileName;

            var contentType =
                string.IsNullOrEmpty(job.CvContentType)
                    ? "application/octet-stream"
                    : job.CvContentType;

            return File(
                job.CvFileData,
                contentType,
                fileName);
        }

        // =====================================================
        // GET: Edit
        // =====================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

<<<<<<< HEAD
            var job =
                await _context.JobApplications
                    .FindAsync(id.Value);
=======
            var job = await _context.JobApplications
                .Include(j => j.Qualifications)
                .Include(j => j.Courses)
                .Include(j => j.Experiences)
                .Include(j => j.Languages)
                .Include(j => j.ComputerSkills)
                .FirstOrDefaultAsync(j => j.Id == id.Value);
>>>>>>> main

            if (job == null)
            {
                return NotFound();
            }

            ViewBag.DrivingLicenseTypes =
                await _context.DrivingLicenseTypes
                    .OrderBy(x => x.Category)
                    .ToListAsync();

            return View(job);
        }

        // =====================================================
        // POST: Edit
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
<<<<<<< HEAD
        public async Task<IActionResult> Edit(
            int id,
            JobApplication jobApplication,
            IFormFile? CvFile)
=======
        public async Task<IActionResult> Edit(int id, JobApplication jobApplication)
>>>>>>> main
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }

            // -------------------------------------------------
            // Remove empty Qualifications
            // -------------------------------------------------

            jobApplication.Qualifications =
                jobApplication.Qualifications?
                    .Where(q =>
                        !string.IsNullOrWhiteSpace(q.Degree)
                        || !string.IsNullOrWhiteSpace(q.Specialization)
                        || !string.IsNullOrWhiteSpace(q.University)
                        || !string.IsNullOrWhiteSpace(q.GraduationYear))
                    .ToList()
                    ?? new List<Qualification>();

            // -------------------------------------------------
            // Remove empty Courses
            // -------------------------------------------------

            jobApplication.Courses =
                jobApplication.Courses?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.CourseName)
                        || !string.IsNullOrWhiteSpace(c.Organization)
                        || !string.IsNullOrWhiteSpace(c.Duration))
                    .ToList()
                    ?? new List<Course>();

            // -------------------------------------------------
            // Remove empty Experiences
            // -------------------------------------------------

            jobApplication.Experiences =
                jobApplication.Experiences?
                    .Where(e =>
                        !string.IsNullOrWhiteSpace(e.CompanyName)
                        || !string.IsNullOrWhiteSpace(e.JobTitle)
                        || e.FromDate != default
                        || e.ToDate.HasValue
                        || !string.IsNullOrWhiteSpace(e.LastSalary))
                    .ToList()
                    ?? new List<Experience>();

            // -------------------------------------------------
            // Remove empty Computer Skills
            // -------------------------------------------------

            jobApplication.ComputerSkills =
                jobApplication.ComputerSkills?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.SkillName))
                    .ToList()
                    ?? new List<ComputerSkill>();

            // -------------------------------------------------
            // Remove empty Languages
            // -------------------------------------------------

            jobApplication.Languages =
                jobApplication.Languages?
                    .Where(l =>
                        !string.IsNullOrWhiteSpace(l.LanguageName)
                        || !string.IsNullOrWhiteSpace(l.Speaking)
                        || !string.IsNullOrWhiteSpace(l.Writing))
                    .ToList()
                    ?? new List<Language>();

            // -------------------------------------------------
            // Remove validation errors
            // -------------------------------------------------

            var keysToRemove = ModelState.Keys
                .Where(k =>
                    k.StartsWith("Qualifications[")
                    || k.StartsWith("Courses[")
                    || k.StartsWith("Experiences[")
                    || k.StartsWith("ComputerSkills[")
                    || k.StartsWith("Languages["))
                .ToList();

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

            // -------------------------------------------------
            // Validate
            // -------------------------------------------------

            if (!ModelState.IsValid)
            {
                ViewBag.DrivingLicenseTypes =
                    await _context.DrivingLicenseTypes
                        .OrderBy(x => x.Category)
                        .ToListAsync();

                return View(jobApplication);
            }

<<<<<<< HEAD
            // -------------------------------------------------
            // Handle CV upload
            // -------------------------------------------------

            if (CvFile != null && CvFile.Length > 0)
            {
                var allowedExtensions =
                    new[]
                    {
                        ".pdf",
                        ".doc",
                        ".docx"
                    };

                var extension =
                    Path.GetExtension(CvFile.FileName)
                        .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.DrivingLicenseTypes =
                        await _context.DrivingLicenseTypes
                            .OrderBy(x => x.Category)
                            .ToListAsync();

                    ModelState.AddModelError(
                        "CvFile",
                        "Only PDF, DOC and DOCX files are allowed.");

                    return View(jobApplication);
                }

                using (var ms = new MemoryStream())
                {
                    await CvFile.CopyToAsync(ms);

                    jobApplication.CvFileData =
                        ms.ToArray();
                }

                jobApplication.CvFileName =
                    Path.GetFileName(CvFile.FileName);

                jobApplication.CvContentType =
                    CvFile.ContentType;
            }

            // -------------------------------------------------
            // Update
            // -------------------------------------------------
=======
            // Preserve fields that the Edit form does not include
            // (CV data + the sequential ApplicationNumber must never change on Edit)
            var existing = await _context.JobApplications
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            jobApplication.CvFileData = existing.CvFileData;
            jobApplication.CvFileName = existing.CvFileName;
            jobApplication.CvContentType = existing.CvContentType;
            jobApplication.CvFilePath = existing.CvFilePath;
            jobApplication.ApplicationNumber = existing.ApplicationNumber;
>>>>>>> main

            try
            {
                _context.Update(jobApplication);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Job application updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
<<<<<<< HEAD
                if (!_context.JobApplications
                    .Any(e => e.Id == jobApplication.Id))
=======
                if (!_context.JobApplications.Any(e => e.Id == jobApplication.Id))
>>>>>>> main
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
<<<<<<< HEAD
        // GET: Manage
        // =====================================================

=======
        // GET: Manage (redirect to Edit)
        // =====================================================
>>>>>>> main
        public IActionResult Manage(int id)
        {
            return RedirectToAction(
                nameof(Edit),
                new { id });
        }

        // =====================================================
        // POST: Delete
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var job =
                await _context.JobApplications
                    .FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            _context.JobApplications.Remove(job);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Job application deleted.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // EXPORT TO EXCEL
        // =====================================================

        public async Task<IActionResult> ExportToExcel(int id)
        {
            var application =
                await _context.JobApplications
                    .Include(x => x.Qualifications)
                    .Include(x => x.Courses)
                    .Include(x => x.Experiences)
                    .Include(x => x.Languages)
                    .Include(x => x.ComputerSkills)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            using var workbook = new XLWorkbook();

            // =================================================
            // Main Information
            // =================================================

            var worksheet =
                workbook.Worksheets.Add("طلب التوظيف");

            worksheet.RightToLeft = true;

            worksheet.Cell(1, 1).Value = "رقم الطلب";
            worksheet.Cell(1, 2).Value =
                application.ApplicationNumber;

            worksheet.Cell(2, 1).Value = "تاريخ الطلب";
            worksheet.Cell(2, 2).Value =
                application.ApplicationDate;

            worksheet.Cell(3, 1).Value = "الاسم الكامل";
            worksheet.Cell(3, 2).Value =
                application.FullName;

            worksheet.Cell(4, 1).Value = "الجنس";
            worksheet.Cell(4, 2).Value =
                application.Gender;

            worksheet.Cell(5, 1).Value = "الجنسية";
            worksheet.Cell(5, 2).Value =
                application.Nationality;

            worksheet.Cell(6, 1).Value = "مكان الولادة";
            worksheet.Cell(6, 2).Value =
                application.PlaceOfBirth;

            worksheet.Cell(7, 1).Value = "تاريخ الميلاد";
            worksheet.Cell(7, 2).Value =
                application.DateOfBirth;

            worksheet.Cell(8, 1).Value = "الرقم الوطني";
            worksheet.Cell(8, 2).Value =
                application.NationalNumber;

            worksheet.Cell(9, 1).Value = "الحالة الاجتماعية";
            worksheet.Cell(9, 2).Value =
                application.MaritalStatus;

            worksheet.Cell(10, 1).Value = "العنوان";
            worksheet.Cell(10, 2).Value =
                application.Address;

            worksheet.Cell(11, 1).Value = "رقم الهاتف";
            worksheet.Cell(11, 2).Value =
                application.Phone;

            worksheet.Cell(12, 1).Value = "البريد الإلكتروني";
            worksheet.Cell(12, 2).Value =
                application.Email;

            worksheet.Cell(13, 1).Value = "الوظيفة المطلوبة";
            worksheet.Cell(13, 2).Value =
                application.Position;

            worksheet.Cell(14, 1).Value = "الراتب المتوقع";
            worksheet.Cell(14, 2).Value =
                application.ExpectedSalary;

            worksheet.Cell(15, 1).Value = "الحالة الصحية";
            worksheet.Cell(15, 2).Value =
                application.HealthCondition;

            worksheet.Cell(16, 1).Value = "ملاحظات";
            worksheet.Cell(16, 2).Value =
                application.Notes;

            // =================================================
            // Qualifications
            // =================================================

            var qualifications =
                workbook.Worksheets.Add("المؤهلات");

            qualifications.RightToLeft = true;

            qualifications.Cell(1, 1).Value = "الدرجة العلمية";
            qualifications.Cell(1, 2).Value = "الاختصاص";
            qualifications.Cell(1, 3).Value = "الجامعة";
            qualifications.Cell(1, 4).Value = "سنة التخرج";

            int row = 2;

            foreach (var item in application.Qualifications)
            {
                qualifications.Cell(row, 1).Value =
                    item.Degree;

                qualifications.Cell(row, 2).Value =
                    item.Specialization;

                qualifications.Cell(row, 3).Value =
                    item.University;

                qualifications.Cell(row, 4).Value =
                    item.GraduationYear;

                row++;
            }

            // =================================================
            // Courses
            // =================================================

            var courses =
                workbook.Worksheets.Add("الدورات");

            courses.RightToLeft = true;

            courses.Cell(1, 1).Value = "اسم الدورة";
            courses.Cell(1, 2).Value = "الجهة المنظمة";
            courses.Cell(1, 3).Value = "المدة";

            row = 2;

            foreach (var item in application.Courses)
            {
                courses.Cell(row, 1).Value =
                    item.CourseName;

                courses.Cell(row, 2).Value =
                    item.Organization;

                courses.Cell(row, 3).Value =
                    item.Duration;

                row++;
            }

            // =================================================
            // Experiences
            // =================================================

            var experiences =
                workbook.Worksheets.Add("الخبرات");

            experiences.RightToLeft = true;

            experiences.Cell(1, 1).Value = "الشركة";
            experiences.Cell(1, 2).Value = "المسمى الوظيفي";
            experiences.Cell(1, 3).Value = "من";
            experiences.Cell(1, 4).Value = "إلى";
            experiences.Cell(1, 5).Value = "آخر راتب";

            row = 2;

            foreach (var item in application.Experiences)
            {
                experiences.Cell(row, 1).Value =
                    item.CompanyName;

                experiences.Cell(row, 2).Value =
                    item.JobTitle;

                experiences.Cell(row, 3).Value =
                    item.FromDate;

                if (item.ToDate.HasValue)
                {
                    experiences.Cell(row, 4).Value =
                        item.ToDate.Value;
                }
                else
                {
                    experiences.Cell(row, 4).Value =
                        "حتى الآن";
                }

                experiences.Cell(row, 5).Value =
                    item.LastSalary;

                row++;
            }

            // =================================================
            // Languages
            // =================================================

            var languages =
                workbook.Worksheets.Add("اللغات");

            languages.RightToLeft = true;

            languages.Cell(1, 1).Value = "اللغة";
            languages.Cell(1, 2).Value = "المحادثة";
            languages.Cell(1, 3).Value = "الكتابة";

            row = 2;

            foreach (var item in application.Languages)
            {
                languages.Cell(row, 1).Value =
                    item.LanguageName;

                languages.Cell(row, 2).Value =
                    item.Speaking;

                languages.Cell(row, 3).Value =
                    item.Writing;

                row++;
            }

            // =================================================
            // Computer Skills
            // =================================================

            var skills =
                workbook.Worksheets.Add("مهارات الكمبيوتر");

            skills.RightToLeft = true;

            skills.Cell(1, 1).Value = "المهارة";

            row = 2;

            foreach (var item in application.ComputerSkills)
            {
                skills.Cell(row, 1).Value =
                    item.SkillName;

                row++;
            }

            // =================================================
            // Formatting
            // =================================================

            foreach (var sheet in workbook.Worksheets)
            {
                sheet.RightToLeft = true;

                var usedRange =
                    sheet.RangeUsed();

                if (usedRange != null)
                {
                    usedRange.Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Right;

                    usedRange.Style.Border.OutsideBorder =
                        XLBorderStyleValues.Thin;

                    usedRange.Style.Border.InsideBorder =
                        XLBorderStyleValues.Thin;
                }

                sheet.Row(1).Style.Font.Bold = true;

                sheet.Columns().AdjustToContents();
            }

            // =================================================
            // Create Excel File
            // =================================================

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            var fileName =
                $"طلب_توظيف_{application.ApplicationNumber.Replace("/", "-")}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // =====================================================
        // PRINT JOB APPLICATION
        // =====================================================

        public async Task<IActionResult> Print(int id)
        {
            var application =
                await _context.JobApplications
                    .Include(x => x.Qualifications)
                    .Include(x => x.Courses)
                    .Include(x => x.Experiences)
                    .Include(x => x.Languages)
                    .Include(x => x.ComputerSkills)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }
    }
<<<<<<< HEAD
}

=======
}
>>>>>>> main
