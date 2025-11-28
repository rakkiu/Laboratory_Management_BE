using PatientService.Application.Models.PatientMedicalRecordDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Queries.ViewMedicalRecordDetail
{
    public class ViewMedicalRecordDetailQuery : IRequest<ViewMedicalRecordDetailResponse> 
    {
        public Guid PatientId { get; set; }
    }


}
