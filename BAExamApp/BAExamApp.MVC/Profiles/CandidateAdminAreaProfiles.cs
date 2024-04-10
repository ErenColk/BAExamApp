
using AutoMapper;
using BAExamApp.Dtos.CandidateQuestions;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionVMs;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.CandidateAdmins;
using BAExamApp.MVC.Areas.Admin.Models.AdminVMs;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.CandidateAdminVMs;
using BAExamApp.Dtos.Candidates;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.StudentVMs;
using BAExamApp.Dtos.CandidateBranches;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.Branch;
using BAExamApp.MVC.Areas.CandidateAdmin.Models.QuestionAnswerVMs;
using BAExamApp.Dtos.CandidateQuestionAnswers;

namespace BAExamApp.MVC.Profiles;

public class CandidateAdminAreaProfiles : Profile
{
    public CandidateAdminAreaProfiles()
    {
        //CandidateAdminController
        CreateMap<CandidateAdminListDto, CandidateAdminListVM>();
        CreateMap<CandidateAdminDto, CandidateAdminUpdateVM>();
        CreateMap<CandidateAdminCreateVM, CandidateAdminCreateDto>();
        CreateMap<CandidateAdminDetailsDto, CandidateAdminDetailsVM>();
        CreateMap<CandidateAdminUpdateVM, CandidateAdminUpdateDto>();
        CreateMap<CandidateQuestionListDto, CandidateQuestionListVM>();

        //CandidateController
        CreateMap<CandidateListDto, CandidateAdminCandidateListVM>();
        CreateMap<CandidateAdminCandidateCreateVM,CandidateCreateDto>();
        CreateMap<CandidateAdminCandidateUpdateVM,CandidateUpdateDto>();
        CreateMap<CandidateDto, CandidateAdminCandidateUpdateVM>();
        CreateMap<CandidateDetailsDto, CandidateAdminCandidateDetailVM>();

        //BranchController
        CreateMap<CandidateBranchCreateVM, CandidateBranchCreateDto>();
        CreateMap<CandidateBranchListDto, CandidateBranchListVM>();
        CreateMap<CandidateBranchUpdateVM, CandidateBranchUpdateDto>();

        //QuestionController
        CreateMap<CandidateQuestionCreateVM,CandidateQuestionCreateDto>();  
        CreateMap<CandidateQuestionAnswerCreateVM, CandidateQuestionAnswerCreateDto>();  
        CreateMap< CandidateQuestionAnswerDto, CandidateQuestionAnswerUpdateVM>();  
        CreateMap<CandidateQuestionAnswerUpdateVM,CandidateQuestionAnswerUpdateDto >();  
        CreateMap<CandidateQuestionAnswerDto,CandidateQuestionUpdateVM>();  
        CreateMap<CandidateQuestionDto,CandidateQuestionUpdateVM>();
        CreateMap< CandidateQuestionUpdateVM, CandidateQuestionUpdateDto>();
        CreateMap<CandidateQuestionAnswerCreateVM, CandidateQuestionAnswerDto>();
        CreateMap< CandidateQuestionDetailsDto, CandidateQuestionDetailsVM>();


    }
}
