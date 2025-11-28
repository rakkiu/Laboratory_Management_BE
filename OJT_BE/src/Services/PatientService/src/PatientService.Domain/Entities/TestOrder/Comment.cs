using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Entities.TestOrder
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public Guid TestOrderId { get; set; }
        public Guid? ResultId { get; set; }
        public string UserName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public TestOrder TestOrder { get; set; } = null!;
        public TestResult? TestResult { get; set; }
    }
}
