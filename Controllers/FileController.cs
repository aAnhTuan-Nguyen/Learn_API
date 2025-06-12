using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TodoWeb.Domain.AppsetingsConfigurations;

namespace TodoWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly FileInformation _fileInformation;
        public FileController(IOptions<FileInformation> fileInformation)
        {
            _fileInformation = fileInformation.Value;
        }


        //public async Task<ActionResult> ReadFileAsync(string path)
        //{
        //    if (!System.IO.File.Exists(path))
        //    {
        //        return NotFound("File not found.");
        //    }

        //    string content = await System.IO.File.ReadAllTextAsync(path);
        //    //string[] content = await System.IO.File.ReadAllLineAsync(path);

        //    return Ok(content);
        //}
        [HttpGet("{fileName}/read")]
        public async Task<ActionResult> ReadFileAsync(string fileName)
        {
            var path = Path.Combine(_fileInformation.RootDirectory, fileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound($"File '{fileName}' not found.");
            }
            // làm việc với stream nhớ dùng using để giải phóng tài nguyên
            using StreamReader reader = new StreamReader(path);

            string? line = null;
            while ((line = reader.ReadLine()) != null)
            {

                Console.WriteLine(line);
            }


            return Ok(line);
        }

        [HttpPost("{fileName}/write")]
        public async Task<ActionResult> WriteFileAsync(string fileName, string content)
        {
            //await System.IO.File.WriteAllTextAsync(path, content);
            //await System.IO.File.AppendAllTextAsync(path, content); // hàm này giúp ko bị ghi đè

            //
            var path = Path.Combine(_fileInformation.RootDirectory, fileName);
            // kiểm tra xem file có tồn tại hay không, nếu không thì tạo mới
            if (!System.IO.File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            using var writer = new StreamWriter(path, append: true);
            await writer.WriteLineAsync(content);

            return Ok("File written successfully.");
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var path = Path.Combine(_fileInformation.RootDirectory, file.FileName);
            using var stream = new FileStream(path, FileMode.Create);
            try
            {
                await file.CopyToAsync(stream);
                
                return Ok($"File '{file.FileName}' uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<ActionResult> DownloadFileAsync(string fileName)
        {
            var path = Path.Combine(_fileInformation.RootDirectory, fileName);
            if (!System.IO.File.Exists(path))
            {
                return NotFound($"File '{fileName}' not found.");
            }
            try
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(path);
                //var contentType = "application/octet-stream"; // hoặc xác định loại MIME phù hợp
                var contentType = "application/pdf"; // nếu là file pdf

                // inline; filename= "fileName" sẽ hiển thị file trong trình duyệt
                // attachment; filename= "fileName" sẽ tải file về
                Response.Headers["Content-Disposition"] = $"inline; filename= \"{fileName}\"";


                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
