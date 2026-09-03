using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using ReceptionSystem.Authorization;
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
using System.Security.Claims;
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
        private readonly PermissionService _permissionService;

        public JobApplicationsController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            JobApplicationNumberGenerator numberGenerator,
            PermissionService permissionService)
        {
            _context = context;
            _env = env;
            _numberGenerator = numberGenerator;
            _permissionService = permissionService;
        }

        // =====================================================
        // GET: /JobApplications
        // =====================================================

        [Permission("JobApplications.View")]
        public async Task<IActionResult> Index()
        {
            var applications =
                await _context.JobApplications
                    .Include(j => j.Qualifications)
                    .Include(j => j.Experiences)
                    .Include(j => j.Courses)
                    .Include(j => j.Languages)
                    .Include(j => j.ComputerSkills)
                    .ToListAsync();

            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var canDelete = false;

            if (!string.IsNullOrEmpty(userId))
            {
                canDelete =
                    await _permissionService.HasPermissionAsync(
                        userId,
                        "JobApplications.Delete");
            }

            ViewBag.CanDelete = canDelete;

            return View(applications);
        }

        // =====================================================
        // GET: /JobApplications/Details/5
        // =====================================================

        [Permission("JobApplications.Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication =
                await _context.JobApplications
                    .Include(j => j.Qualifications)
                    .Include(j => j.Experiences)
                    .Include(j => j.Courses)
                    .Include(j => j.Languages)
                    .Include(j => j.ComputerSkills)
                    .FirstOrDefaultAsync(j => j.Id == id);

            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // =====================================================
        // GET: Create
        // =====================================================

        [Permission("JobApplications.Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.DrivingLicenseTypes =
                await _context.DrivingLicenseTypes
                    .OrderBy(x => x.Category)
                    .ToListAsync();

            return View(new JobApplication());
        }

        // =====================================================
        // POST: Create
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("JobApplications.Create")]
        public async Task<IActionResult> Create(
            JobApplication jobApplication,
            IFormFile? CvFile)
        {
            // =================================================
            // Remove empty Qualifications
            // =================================================

            jobApplication.Qualifications =
                jobApplication.Qualifications?
                    .Where(q =>
                        !string.IsNullOrWhiteSpace(q.Degree)
                        || !string.IsNullOrWhiteSpace(q.Specialization)
                        || !string.IsNullOrWhiteSpace(q.University)
                        || !string.IsNullOrWhiteSpace(q.GraduationYear))
                    .ToList()
                    ?? new List<Qualification>();

            // =================================================
            // Remove empty Courses
            // =================================================

            jobApplication.Courses =
                jobApplication.Courses?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.CourseName)
                        || !string.IsNullOrWhiteSpace(c.Organization)
                        || !string.IsNullOrWhiteSpace(c.Duration))
                    .ToList()
                    ?? new List<Course>();

            // =================================================
            // Remove empty Experiences
            // =================================================

            jobApplication.Experiences =
                jobApplication.Experiences?
                    .Where(e =>
                        !string.IsNullOrWhiteSpace(e.CompanyName)
                        || !string.IsNullOrWhiteSpace(e.JobTitle)
                        || e.FromDate.HasValue
                        || e.ToDate.HasValue
                        || !string.IsNullOrWhiteSpace(e.LastSalary))
                    .ToList()
                    ?? new List<Experience>();

            // =================================================
            // Remove empty Computer Skills
            // =================================================

            jobApplication.ComputerSkills =
                jobApplication.ComputerSkills?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.SkillName))
                    .ToList()
                    ?? new List<ComputerSkill>();

            // =================================================
            // Remove empty Languages
            // =================================================

            jobApplication.Languages =
                jobApplication.Languages?
                    .Where(l =>
                        !string.IsNullOrWhiteSpace(l.LanguageName)
                        || !string.IsNullOrWhiteSpace(l.Speaking)
                        || !string.IsNullOrWhiteSpace(l.Writing))
                    .ToList()
                    ?? new List<Language>();

            // =================================================
            // Remove validation errors from optional sections
            // =================================================

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

            // =================================================
            // Validate
            // =================================================

            if (!ModelState.IsValid)
            {
                ViewBag.DrivingLicenseTypes =
                    await _context.DrivingLicenseTypes
                        .OrderBy(x => x.Category)
                        .ToListAsync();

                return View(jobApplication);
            }

            // =================================================
            // Application Date
            // =================================================

            jobApplication.ApplicationDate = DateTime.Now;

            // =================================================
            // Save CV
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
                        "Only PDF, DOC and DOCX files are allowed.");

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

            // =================================================
            // Generate Application Number
            // =================================================

            jobApplication.ApplicationNumber =
                await _numberGenerator.GenerateNextNumberAsync("HR");

            // =================================================
            // Save
            // =================================================

            _context.JobApplications.Add(jobApplication);

            await _context.SaveChangesAsync();

            // =================================================
            // Final Application Number
            // =================================================

            jobApplication.ApplicationNumber =
                $"DAMA/HR/{jobApplication.Id}";

            _context.JobApplications.Update(jobApplication);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Job application saved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GET: Download CV
        // =====================================================

        [Permission("JobApplications.DownloadCv")]
        public async Task<IActionResult> DownloadCv(int id)
        {
            var job =
                await _context.JobApplications
                    .FindAsync(id);

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

        [Permission("JobApplications.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job =
                await _context.JobApplications
                    .Include(j => j.Qualifications)
                    .Include(j => j.Courses)
                    .Include(j => j.Experiences)
                    .Include(j => j.Languages)
                    .Include(j => j.ComputerSkills)
                    .FirstOrDefaultAsync(j => j.Id == id.Value);

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
        [Permission("JobApplications.Edit")]
        public async Task<IActionResult> Edit(
            int id,
            JobApplication jobApplication,
            IFormFile? CvFile)
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }

            // =================================================
            // Remove empty Qualifications
            // =================================================

            jobApplication.Qualifications =
                jobApplication.Qualifications?
                    .Where(q =>
                        !string.IsNullOrWhiteSpace(q.Degree)
                        || !string.IsNullOrWhiteSpace(q.Specialization)
                        || !string.IsNullOrWhiteSpace(q.University)
                        || !string.IsNullOrWhiteSpace(q.GraduationYear))
                    .ToList()
                    ?? new List<Qualification>();

            // =================================================
            // Remove empty Courses
            // =================================================

            jobApplication.Courses =
                jobApplication.Courses?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.CourseName)
                        || !string.IsNullOrWhiteSpace(c.Organization)
                        || !string.IsNullOrWhiteSpace(c.Duration))
                    .ToList()
                    ?? new List<Course>();

            // =================================================
            // Remove empty Experiences
            // =================================================

            jobApplication.Experiences =
                jobApplication.Experiences?
                    .Where(e =>
                        !string.IsNullOrWhiteSpace(e.CompanyName)
                        || !string.IsNullOrWhiteSpace(e.JobTitle)
                        || e.FromDate.HasValue
                        || e.ToDate.HasValue
                        || !string.IsNullOrWhiteSpace(e.LastSalary))
                    .ToList()
                    ?? new List<Experience>();

            // =================================================
            // Remove empty Computer Skills
            // =================================================

            jobApplication.ComputerSkills =
                jobApplication.ComputerSkills?
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.SkillName))
                    .ToList()
                    ?? new List<ComputerSkill>();

            // =================================================
            // Remove empty Languages
            // =================================================

            jobApplication.Languages =
                jobApplication.Languages?
                    .Where(l =>
                        !string.IsNullOrWhiteSpace(l.LanguageName)
                        || !string.IsNullOrWhiteSpace(l.Speaking)
                        || !string.IsNullOrWhiteSpace(l.Writing))
                    .ToList()
                    ?? new List<Language>();

            // =================================================
            // Remove validation errors
            // =================================================

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

            // =================================================
            // Get existing application
            // =================================================

            var existing =
                await _context.JobApplications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            // =================================================
            // Validate
            // =================================================

            if (!ModelState.IsValid)
            {
                ViewBag.DrivingLicenseTypes =
                    await _context.DrivingLicenseTypes
                        .OrderBy(x => x.Category)
                        .ToListAsync();

                return View(jobApplication);
            }

            // =================================================
            // Handle CV
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
            else
            {
                jobApplication.CvFileData =
                    existing.CvFileData;

                jobApplication.CvFileName =
                    existing.CvFileName;

                jobApplication.CvContentType =
                    existing.CvContentType;

                jobApplication.CvFilePath =
                    existing.CvFilePath;
            }

            // =================================================
            // Preserve Application Number
            // =================================================

            jobApplication.ApplicationNumber =
                existing.ApplicationNumber;

            // =================================================
            // Preserve Application Date
            // =================================================

            jobApplication.ApplicationDate =
                existing.ApplicationDate;

            // =================================================
            // Update
            // =================================================

            try
            {
                _context.Update(jobApplication);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Job application updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.JobApplications
                    .Any(e => e.Id == jobApplication.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GET: Manage
        // =====================================================

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
        [Permission("JobApplications.Delete")]
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

        [Permission("JobApplications.Export")]
        public async Task<IActionResult> ExportToExcel(int id)
        {
            // =================================================
            // Load Job Application + ALL Related Data
            // =================================================

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

            // =================================================
            // Create Workbook
            // =================================================

            using var workbook = new XLWorkbook();

            // =================================================
            // Sheet 1 - Main Information
            // =================================================

            var worksheet = workbook.Worksheets.Add("طلب التوظيف");

            worksheet.RightToLeft = true;

            // Header row (horizontal)
            worksheet.Cell(1, 1).Value = "رقم الطلب";
            worksheet.Cell(1, 2).Value = "تاريخ الطلب";
            worksheet.Cell(1, 3).Value = "الاسم الكامل";
            worksheet.Cell(1, 4).Value = "الجنس";
            worksheet.Cell(1, 5).Value = "الجنسية";
            worksheet.Cell(1, 6).Value = "مكان الولادة";
            worksheet.Cell(1, 7).Value = "تاريخ الميلاد";
            worksheet.Cell(1, 8).Value = "الرقم الوطني";
            worksheet.Cell(1, 9).Value = "الحالة الاجتماعية";
            worksheet.Cell(1, 10).Value = "العنوان";
            worksheet.Cell(1, 11).Value = "رقم الهاتف";
            worksheet.Cell(1, 12).Value = "البريد الإلكتروني";
            worksheet.Cell(1, 13).Value = "رخصة القيادة";
            worksheet.Cell(1, 14).Value = "الوظيفة المطلوبة";
            worksheet.Cell(1, 15).Value = "الراتب المتوقع";
            worksheet.Cell(1, 16).Value = "تاريخ مباشرة العمل";
            worksheet.Cell(1, 17).Value = "يعمل حالياً";
            worksheet.Cell(1, 18).Value = "الحالة الصحية";
            worksheet.Cell(1, 19).Value = "سوابق قضائية";
            worksheet.Cell(1, 20).Value = "ملاحظات";

            // Values row
            worksheet.Cell(2, 1).Value = application.ApplicationNumber ?? "";

            worksheet.Cell(2, 2).Value = application.ApplicationDate;
            worksheet.Cell(2, 2).Style.DateFormat.Format = "yyyy-MM-dd";

            worksheet.Cell(2, 3).Value = application.FullName ?? "";
            worksheet.Cell(2, 4).Value = application.Gender ?? "";
            worksheet.Cell(2, 5).Value = application.Nationality ?? "";
            worksheet.Cell(2, 6).Value = application.PlaceOfBirth ?? "";

            if (application.DateOfBirth.HasValue)
            {
                worksheet.Cell(2, 7).Value = application.DateOfBirth.Value;
                worksheet.Cell(2, 7).Style.DateFormat.Format = "yyyy-MM-dd";
            }
            else
            {
                worksheet.Cell(2, 7).Value = "";
            }

            worksheet.Cell(2, 8).Value = application.NationalNumber ?? "";
            worksheet.Cell(2, 9).Value = application.MaritalStatus ?? "";
            worksheet.Cell(2, 10).Value = application.Address ?? "";
            worksheet.Cell(2, 11).Value = application.Phone ?? "";
            worksheet.Cell(2, 12).Value = application.Email ?? "";
            worksheet.Cell(2, 13).Value = application.DrivingLicense ?? "";
            worksheet.Cell(2, 14).Value = application.Position ?? "";
            worksheet.Cell(2, 15).Value = application.ExpectedSalary ?? "";

            if (application.AvailableStartDate.HasValue)
            {
                worksheet.Cell(2, 16).Value = application.AvailableStartDate.Value;
                worksheet.Cell(2, 16).Style.DateFormat.Format = "yyyy-MM-dd";
            }
            else
            {
                worksheet.Cell(2, 16).Value = "";
            }

            worksheet.Cell(2, 17).Value = application.CurrentlyWorking ? "نعم" : "لا";
            worksheet.Cell(2, 18).Value = application.HealthCondition ?? "";
            worksheet.Cell(2, 19).Value = application.CriminalRecord ? "نعم" : "لا";
            worksheet.Cell(2, 20).Value = application.Notes ?? "";

            // =================================================
            // Sheet 2 - Qualifications
            // =================================================

            var qualifications =
                workbook.Worksheets.Add("المؤهلات");

            qualifications.RightToLeft = true;

            qualifications.Cell(1, 1).Value =
                "الدرجة العلمية";

            qualifications.Cell(1, 2).Value =
                "الاختصاص";

            qualifications.Cell(1, 3).Value =
                "الجامعة / المعهد";

            qualifications.Cell(1, 4).Value =
                "سنة التخرج";

            int qualificationRow = 2;

            if (application.Qualifications != null &&
                application.Qualifications.Any())
            {
                foreach (var qualification
                         in application.Qualifications)
                {
                    qualifications.Cell(
                        qualificationRow, 1).Value =
                        qualification.Degree ?? "";

                    qualifications.Cell(
                        qualificationRow, 2).Value =
                        qualification.Specialization ?? "";

                    qualifications.Cell(
                        qualificationRow, 3).Value =
                        qualification.University ?? "";

                    qualifications.Cell(
                        qualificationRow, 4).Value =
                        qualification.GraduationYear ?? "";

                    qualificationRow++;
                }
            }
            else
            {
                qualifications.Cell(2, 1).Value =
                    "لا يوجد مؤهلات علمية";
            }

            // =================================================
            // Sheet 3 - Courses
            // =================================================

            var courses =
                workbook.Worksheets.Add("الدورات");

            courses.RightToLeft = true;

            courses.Cell(1, 1).Value =
                "اسم الدورة";

            courses.Cell(1, 2).Value =
                "الجهة المنظمة";

            courses.Cell(1, 3).Value =
                "المدة";

            int courseRow = 2;

            if (application.Courses != null &&
                application.Courses.Any())
            {
                foreach (var course in application.Courses)
                {
                    courses.Cell(
                        courseRow, 1).Value =
                        course.CourseName ?? "";

                    courses.Cell(
                        courseRow, 2).Value =
                        course.Organization ?? "";

                    courses.Cell(
                        courseRow, 3).Value =
                        course.Duration ?? "";

                    courseRow++;
                }
            }
            else
            {
                courses.Cell(2, 1).Value =
                    "لا توجد دورات";
            }

            // =================================================
            // Sheet 4 - Experiences
            // =================================================

            var experiences =
                workbook.Worksheets.Add("الخبرات");

            experiences.RightToLeft = true;

            experiences.Cell(1, 1).Value =
                "الشركة";

            experiences.Cell(1, 2).Value =
                "المسمى الوظيفي";

            experiences.Cell(1, 3).Value =
                "من تاريخ";

            experiences.Cell(1, 4).Value =
                "إلى تاريخ";

            experiences.Cell(1, 5).Value =
                "آخر راتب";

            int experienceRow = 2;

            if (application.Experiences != null &&
                application.Experiences.Any())
            {
                foreach (var experience
                         in application.Experiences)
                {
                    experiences.Cell(
                        experienceRow, 1).Value =
                        experience.CompanyName ?? "";

                    experiences.Cell(
                        experienceRow, 2).Value =
                        experience.JobTitle ?? "";

                    // From Date
                    if (experience.FromDate.HasValue)
                    {
                        experiences.Cell(
                            experienceRow, 3).Value =
                            experience.FromDate.Value;

                        experiences.Cell(
                            experienceRow, 3)
                            .Style.DateFormat.Format =
                            "yyyy-MM-dd";
                    }
                    else
                    {
                        experiences.Cell(
                            experienceRow, 3).Value =
                            "";
                    }

                    // To Date
                    if (experience.ToDate.HasValue)
                    {
                        experiences.Cell(
                            experienceRow, 4).Value =
                            experience.ToDate.Value;

                        experiences.Cell(
                            experienceRow, 4)
                            .Style.DateFormat.Format =
                            "yyyy-MM-dd";
                    }
                    else
                    {
                        experiences.Cell(
                            experienceRow, 4).Value =
                            "حتى الآن";
                    }

                    experiences.Cell(
                        experienceRow, 5).Value =
                        experience.LastSalary ?? "";

                    experienceRow++;
                }
            }
            else
            {
                experiences.Cell(2, 1).Value =
                    "لا توجد خبرات عملية";
            }

            // =================================================
            // Sheet 5 - Languages
            // =================================================

            var languages =
                workbook.Worksheets.Add("اللغات");

            languages.RightToLeft = true;

            languages.Cell(1, 1).Value =
                "اللغة";

            languages.Cell(1, 2).Value =
                "التحدث";

            languages.Cell(1, 3).Value =
                "الكتابة";

            int languageRow = 2;

            if (application.Languages != null &&
                application.Languages.Any())
            {
                foreach (var language
                         in application.Languages)
                {
                    languages.Cell(
                        languageRow, 1).Value =
                        language.LanguageName ?? "";

                    languages.Cell(
                        languageRow, 2).Value =
                        language.Speaking ?? "";

                    languages.Cell(
                        languageRow, 3).Value =
                        language.Writing ?? "";

                    languageRow++;
                }
            }
            else
            {
                languages.Cell(2, 1).Value =
                    "لا توجد لغات";
            }

            // =================================================
            // Sheet 6 - Computer Skills
            // =================================================

            var skills =
                workbook.Worksheets.Add("مهارات الكمبيوتر");

            skills.RightToLeft = true;

            skills.Cell(1, 1).Value =
                "المهارة";

            int skillRow = 2;

            if (application.ComputerSkills != null &&
                application.ComputerSkills.Any())
            {
                foreach (var skill
                         in application.ComputerSkills)
                {
                    skills.Cell(
                        skillRow, 1).Value =
                        skill.SkillName ?? "";

                    skillRow++;
                }
            }
            else
            {
                skills.Cell(2, 1).Value =
                    "لا توجد مهارات";
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
                    // Header
                    var headerRow =
                        sheet.Row(1);

                    headerRow.Style.Font.Bold =
                        true;

                    // Alignment
                    usedRange.Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Right;

                    usedRange.Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    // Borders
                    usedRange.Style.Border.OutsideBorder =
                        XLBorderStyleValues.Thin;

                    usedRange.Style.Border.InsideBorder =
                        XLBorderStyleValues.Thin;

                    // Wrap text
                    usedRange.Style.Alignment.WrapText =
                        true;

                    // Auto width
                    sheet.Columns()
                        .AdjustToContents();
                }

                // Minimum width
                foreach (var column in sheet.ColumnsUsed())
                {
                    if (column.Width < 15)
                    {
                        column.Width = 15;
                    }
                }
            }

            // =================================================
            // Create Excel File
            // =================================================

            using var stream =
                new MemoryStream();

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

        [Permission("JobApplications.Print")]
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
}