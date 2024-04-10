using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.CandidateAdmins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Profiles;
public class CandidateAdminProfile : Profile
{
    public CandidateAdminProfile()
    {
        CreateMap<CandidateAdmin, CandidateAdminDto>();
        CreateMap<CandidateAdmin, CandidateAdminListDto>();
        CreateMap<CandidateAdminCreateDto, CandidateAdmin>();
        CreateMap<CandidateAdminUpdateDto, CandidateAdmin>();
        CreateMap<CandidateAdmin, CandidateAdminDetailsDto>();
    }
}
