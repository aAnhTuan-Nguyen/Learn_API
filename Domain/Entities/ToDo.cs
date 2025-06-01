using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Domain.Entities
{
    public class ToDo
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
