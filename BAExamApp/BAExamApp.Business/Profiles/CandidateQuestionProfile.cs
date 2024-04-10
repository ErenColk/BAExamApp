using BAExamApp.Dtos.CandidateQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Profiles;
public class CandidateQuestionProfile : Profile
{
    public CandidateQuestionProfile()
    {
        CreateMap<CandidateQuestion, CandidateQuestionListDto>();
        CreateMap<CandidateQuestion, CandidateQuestionDto>();
        CreateMap<CandidateQuestionDto, CandidateQuestion>();
        CreateMap<CandidateQuestionUpdateDto, CandidateQuestion>();
        CreateMap<CandidateQuestion, CandidateQuestionDetailsDto>();

        CreateMap<CandidateQuestionCreateDto, CandidateQuestion>();

    }
}
