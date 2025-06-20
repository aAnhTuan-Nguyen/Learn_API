namespace TodoWeb.Application.Dtos.SchoolDTO
{
    public class SchoolReportModel
    {
        public string Author { get; set; } = string.Empty;
        public string DateCreated { get; set; } = string.Empty;
        public List<SchoolViewModel> Schools { get; set; } = new List<SchoolViewModel>();
    }
}
