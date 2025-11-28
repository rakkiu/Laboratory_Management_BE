

namespace PatientService.Application.Models.TestOrderDto
{
    public class TestOrderCommentDto
    {
        public Guid CommentId { get; set; }
        public Guid? ResultId { get; set; }
        public string UserName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}





