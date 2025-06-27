using Microsoft.Extensions.Options;
using RazorLight;
using TodoWeb.Application.Dtos.SchoolDTO;
using TodoWeb.Application.Services;
using TodoWeb.Domain.AppsetingsConfigurations;

namespace TodoWeb.Application.BackGroundJobs
{
    public class GenerateSchoolReportJob
    {
        private readonly ISchoolService _schoolService;
        private readonly FileInformation _fileInformation;
        public GenerateSchoolReportJob(ISchoolService schoolService, IOptions<FileInformation> fileInformation)
        {
            _schoolService = schoolService;
            _fileInformation = fileInformation.Value;
        }
        public async Task ExcuteAsync()
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
            var path = Path.Combine(_fileInformation.RootDirectory, $"SchoolReport-{DateTime.Now.ToString("dd-MM-yyyy")}.pdf");
            pdf.SaveAs(path);
        }
    }
}
