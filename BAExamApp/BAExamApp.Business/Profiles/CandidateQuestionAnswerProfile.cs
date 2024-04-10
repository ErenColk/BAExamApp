using BAExamApp.Dtos.CandidateQuestionAnswers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Profiles;
public class CandidateQuestionAnswerProfile :Profile
{

    public CandidateQuestionAnswerProfile()
    {
        CreateMap<CandidateQuestionAnswerCreateDto, CandidateQuestionAnswer>();
        CreateMap<CandidateQuestionAnswer, CandidateQuestionAnswerDto>();
        CreateMap<CandidateQuestionAnswerDto, CandidateQuestionAnswer>();

    }
}
