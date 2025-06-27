using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using RazorLight;
using TodoWeb.Application.ActionFillter;
using TodoWeb.Application.Dtos.SchoolDTO;
using TodoWeb.Application.Dtos.UserDTO;
using TodoWeb.Application.Services;
using TodoWeb.Domain.AppsetingsConfigurations;


namespace TodoWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[TypeFilter(typeof(AuthorizationFilter), Arguments = ["Admin,Teacher"])]
    //[TypeFilter(typeof(AuthorizationFilter), Arguments = [$"{nameof(Role.Admin)},{nameof(Role.Teacher)}"])]

    //[Authorize(Roles= "Admin,Stud")] // Cái này nó như cái filter

    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        private readonly FileInformation _fileInformation;
        public SchoolController(ISchoolService schoolService, IOptions<FileInformation> fileInformationOptions)
        {
            _schoolService = schoolService;
            _fileInformation = fileInformationOptions.Value;
        }

        [HttpGet]
        public IEnumerable<SchoolViewModel> Get(int? id)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //var role = HttpContext.Session.GetString("Role");
            //if (userId == null || role != "Admin")
            //{
            //    return null;
            //}

            return _schoolService.Get(id);
        }
        [HttpGet("{id}/detail")]
        public SchoolStudentModel GetSchoolDetail(int id)
        {
            return _schoolService.GetSchoolDetail(id);
        }

        [HttpPost]
        public int Post(SchoolCreateModel school)
        {
            return _schoolService.Post(school);
        }

        [HttpPut]
        public bool Put(int id, SchoolUpdateModel school)
        {
            return _schoolService.Put(id, school);
        }

        [HttpDelete]
        public bool Delete(int id)
        {
            return _schoolService.Delete(id);
        }

        [HttpGet("excel")]
        public async Task<IActionResult> ExportSchools()
        {
            var schools = _schoolService.Get(null);
            using var stream = new MemoryStream(); // 
            using var excelFile = new ExcelPackage(stream);

            var worksheet = excelFile.Workbook.Worksheets.Add("Schools");
            worksheet.Cells[1, 1].LoadFromCollection(schools, true, TableStyles.Light1);

            await excelFile.SaveAsAsync(stream);
            return File(stream.ToArray(), "application/octet-stream", "Schools.xlsx");

        }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            try
            {
                var result = await _schoolService.ImportExcelAsync(file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> ExportSchoolPdf()
        {
            var htmlText= await System.IO.File.ReadAllTextAsync("Template/SchoolReport.html");
            htmlText.Replace("{{SchoolName}}", "FPT University");

            var renderEngine = new ChromePdfRenderer();

            var pdf = await renderEngine.RenderHtmlAsPdfAsync(htmlText);

            var path = Path.Combine(_fileInformation.RootDirectory, "SchoolReport.pdf");
            pdf.SaveAs(path);

            return Ok();
        }

        [HttpGet("pdf-dynamic")]
        public async Task<IActionResult> ExportSchoolPdfDynamic()
        {
            var schools = _schoolService.Get(null);

            var model = new SchoolReportModel
            {
                Author = "nat",
                DateCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                Schools = schools.ToList()
            };


            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Template"))
                .UseMemoryCachingProvider()
                .Build();

            string htmlText = await engine.CompileRenderAsync("SchoolReportDynamic.cshtml", model);
            var renderEngine = new ChromePdfRenderer();
            var pdf = await renderEngine.RenderHtmlAsPdfAsync(htmlText);
            var path = Path.Combine(_fileInformation.RootDirectory, "SchoolReport.pdf");
            pdf.SaveAs(path);

            return File(pdf.BinaryData, "application/pdf", "SchoolReport.pdf");
        }

        [HttpGet("test-hangfire")]
        public async Task<IActionResult> TestHangFire()
        {
            // Hàm Enqueue được sử dụng để thêm một công việc vào hàng đợi Hangfire.
            string jobId1 = BackgroundJob.Enqueue(() => Console.WriteLine("Hello, Hangfire!"));

            // Hàm Schedule được sử dụng để thêm một công việc vào hàng đợi Hangfire với thời gian trì hoãn.
            string jobId2 = BackgroundJob.Schedule(() => Console.WriteLine("This is a delayed job!"), TimeSpan.FromSeconds(10));

            // hàm ContinueJobWith được sử dụng để tạo một công việc tiếp theo sẽ chạy sau khi công việc trước đó hoàn thành.
            string jobId3 = BackgroundJob.ContinueJobWith(jobId2, () => Console.WriteLine("This is a continuos job after run job 2"));

            BackgroundJob.ContinueJobWith(jobId3, () => Console.WriteLine("This is a continuos job after run job 3"));

            return Ok("Hangfire job has been enqueued successfully.");
        }
        
    }
}
