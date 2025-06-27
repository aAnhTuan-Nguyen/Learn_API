using TodoWeb.Application.Dtos;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services
{
    public interface IToDoService
    {
        int Post(ToDoCreateModel toDo);

        Guid Genrate();
    }

    public class ToDoService : IToDoService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IGuidGenerator _guidGenerator;
        public ToDoService(IApplicationDbContext dbContext, IGuidGenerator guidGenerator)
        {
            _dbContext = dbContext;
            _guidGenerator = guidGenerator;
        }

        public Guid Genrate()
        {
            return _guidGenerator.Generate();
        }

        public int Post(ToDoCreateModel toDo)
        {
            var data = new ToDo
            {
                Description = toDo.Description,
            };
            _dbContext.ToDos.Add(data);
            _dbContext.SaveChanges();
            return data.Id;
        }
    }
}
