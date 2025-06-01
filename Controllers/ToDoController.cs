using Microsoft.AspNetCore.Mvc;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Services;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IToDoService _toDoService;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ISingletronGenerator _singletronGenerator;
        public ToDoController(
            IApplicationDbContext dbContext,
            IToDoService toDoService,
            IGuidGenerator guidGenerator,
            ISingletronGenerator singletronGenerator)
        {
            _dbContext = dbContext;
            _toDoService = toDoService;
            _guidGenerator = guidGenerator;
            _singletronGenerator = singletronGenerator;
        }

        [HttpGet("guid")]
        public Guid[] GetGuid()
        {
            return new Guid[] {
                _guidGenerator.Generate(),
                _singletronGenerator.Generator(),
            };
        }
        [HttpGet]
        public IEnumerable<ToDo> Get(bool isCompleted)
        {
            // Muốn lấy một cột thì dùng như vậy
            //var data = _dbContext.ToDos.Where(x => x.IsCompleted == isCompleted).ToList();
            // còn nếu muốn lấy nhiều cột thì phải khai báo một cái class hoặc dùng anonymous class
            var data = _dbContext.ToDos.Where(x => x.IsCompleted == isCompleted)
                .Select(x => new ToDoViewModel
                    {
                        Description = x.Description,
                        IsCompleted = x.IsCompleted
                    })
                .ToList();
            return null;
        }

        [HttpPost]
        public int Post(ToDoCreateModel toDo)
        {
            return _toDoService.Post(toDo);
        }

        [HttpPut]
        public int Put(ToDoUpdateModel toDo)
        {
            var data = _dbContext.ToDos.Find(toDo.Id);
            if (data == null)
            {
                return -1;
            }
            data.Description = toDo.Description;
            data.IsCompleted = toDo.IsCompleted;
            _dbContext.SaveChanges();
            return toDo.Id;
        }

        [HttpDelete]
        public int Delete(int id)
        {
            var data = _dbContext.ToDos.Find(id);
            if (data == null)
            {
                return -1;
            }
            _dbContext.ToDos.Remove(data);
            _dbContext.SaveChanges();
            return 0;
        }
    }
}
