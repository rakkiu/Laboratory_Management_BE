using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Models.PatientMedicalRecordDto
{
    public class ViewMedicalRecordDetailResponse
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public List<TestOrderReponse> TestOrders { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }
        public string IdentifyNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? LastTestDate { get; set; }

    }
    public class TestOrderReponse
    {
        public Guid TestOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
