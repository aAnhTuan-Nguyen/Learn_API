using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using TodoWeb.Application.ActionFillter;
using TodoWeb.Application.Dtos.SchoolDTO;
using TodoWeb.Application.Dtos.UserDTO;
using TodoWeb.Application.Services;
using TodoWeb.Infrastructures;

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
        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
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
    }
}
