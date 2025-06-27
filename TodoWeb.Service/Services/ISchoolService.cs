using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Dtos.SchoolDTO;
using TodoWeb.Application.Dtos.StudentDTO;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services
{
    public interface ISchoolService
    {
        public IEnumerable<SchoolViewModel> Get(int? id);
        public SchoolStudentModel GetSchoolDetail(int schoolId);
        public int Post(SchoolCreateModel school);
        public bool Put(int id, SchoolUpdateModel school);
        public bool Delete(int id);
        public Task<string> ImportExcelAsync(IFormFile file);
    }

        public class SchoolService : ISchoolService
    {
        private readonly IApplicationDbContext _dbContext;

        public SchoolService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Delete(int id)
        {
            var data = _dbContext.School.Find(id);
            if (data == null) return false;
            _dbContext.School.Remove(data);
            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<SchoolViewModel> Get(int? id)
        {
            var query = _dbContext.School.Where(school => school.Id == id);
            if (id != null)
            {
                return query
                    .Select(school => new SchoolViewModel
                    {
                        Id = school.Id,
                        Name = school.Name,
                        Address = school.Address,
                    });
            }
            return _dbContext.School
                .Select(school => new SchoolViewModel
                {
                    Id = school.Id,
                    Name = school.Name,
                    Address = school.Address,
                });
        }

        

        public int Post(SchoolCreateModel school)
        {
            if (school == null)
            {
                return -1;
            }
            var data = new School
            {
                Name = school.Name,
                Address = school.Address,
            };
            _dbContext.School.Add(data);
            var state = _dbContext.Entry(data).State; // detached
            

            return data.Id;
        }

        public bool Put(int id, SchoolUpdateModel school)
        {
            var data = _dbContext.School.Find(id);
            if (data == null) return false;
            data.Name = school.Name;
            data.Address = school.Address;
            return true;
        }

        public SchoolStudentModel GetSchoolDetail(int schoolId)
        {
            // 1 school => 10 student
            // 1 students => 3 course
            var school = _dbContext.School.Find(schoolId); //1
            if (school == null)
            {
                return null;
            }

             _dbContext.Entry(school).Collection(x => x.Students).Load();
            var students = school.Students;
            return new SchoolStudentModel
            {
                Id = school.Id,
                Name = school.Name,
                Address = school.Address,
                Students = students.Select(student => new StudentViewModel
                {
                    Id = student.Id,
                    FullName = $"{student.FirstName} {student.LastName}",
                    Age = student.Age,
                    SchoolName = student.School.Name
                }).ToList(),
            };
        }

        public Task<string> ImportExcelAsync(IFormFile file)
        {
            using var excelFile = new ExcelPackage(file.OpenReadStream());
            var workSheet = excelFile.Workbook.Worksheets.FirstOrDefault();

            if (workSheet == null)
            {
                throw new Exception("No worksheet found in the uploaded file.");
            }

            if (workSheet.Dimension == null || workSheet.Dimension.Rows < 2)
            {
                throw new Exception("The uploaded file is empty or does not contain any data.");
            }

            // Validate headers
            var headers = new[] { "Id", "Name", "Address" };
            for (int col = 1; col <= headers.Length; col++)
            {
                var headerCell = workSheet.Cells[1, col];
                if (headerCell.Text != headers[col - 1])
                {
                    throw new Exception($"Invalid header '{headerCell.Text}' at column {col}. Expected '{headers[col - 1]}'.");
                }
            }

            // Process rows
            for (int row = 2; row <= workSheet.Dimension.Rows; row++)
            {
                var schoolIdCell = workSheet.Cells[row, 1];
                var nameCell = workSheet.Cells[row, 2];
                var addressCell = workSheet.Cells[row, 3];

                if (string.IsNullOrWhiteSpace(schoolIdCell.Text))
                {
                    var newSchool = new SchoolCreateModel
                    {
                        Name = nameCell.Text,
                        Address = addressCell.Text
                    };
                    Post(newSchool);
                }
                else if (int.TryParse(schoolIdCell.Text, out int schoolId))
                {
                    var updateSchool = new SchoolUpdateModel
                    {
                        Name = nameCell.Text,
                        Address = addressCell.Text
                    };
                    Put(schoolId, updateSchool);
                }
                else
                {
                    throw new Exception($"Invalid school ID at row {row}");
                }
            }
            return Task.FromResult(file.FileName + " imported successfully.");
        }

    }
}
