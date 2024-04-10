using BAExamApp.Dtos.Branches;
using BAExamApp.Dtos.CandidateBranches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Profiles;
public class CandidateBranchProfile : Profile
{
    public CandidateBranchProfile()
    {
        CreateMap<CandidateBranch, CandidateBranchDto>();
        CreateMap<CandidateBranchCreateDto, CandidateBranch>();
        CreateMap<CandidateBranch, CandidateBranchListDto>();
        CreateMap<CandidateBranchUpdateDto, CandidateBranch>();
    }
}
