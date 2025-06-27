namespace TodoWeb.Application.Dtos
{
    public class ToDoUpdateModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
