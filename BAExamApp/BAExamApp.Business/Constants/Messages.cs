using BAExamApp.Entities.DbSets;

namespace BAExamApp.Business.Constants;

public class Messages
{
    public const string ListedSuccess = "Listed_Success";
    public const string ListNotFound = "List_Not_Found";
    public const string ListReceived = "List_Received";
    public const string ListNotReceived = "List_Not_Received";
    public const string InvalidParameter = "Invalid_Parameter";

    public const string EmailDuplicate = "Email_Duplicate";
    public const string UserNotFound = "User_Not_Found";
    public const string FoundSuccess = "Found_Successfully";

    public const string AddSuccess = "Add_Success";
    public const string AddError = "Add_Error";
    public const string AddFail = "Add_Fail";
    public const string AddUserFail = "Add_User_Fail";
    public const string AddUserRoleFail = "Add_User_Role_Fail";
    public const string AddFailAlreadyExists = "Add_Fail_Already_Exists";

    public const string ApproveSuccess = "Approve_Success";
    public const string RejectSuccess = "Reject_Success";
    public const string ReviewSuccess = "Review_Success";
    public const string TestSuccess = "Test_Success";

    public const string UpdateSuccess = "Update_Success";
    public const string UpdateFail = "Update_Fail";

    public const string DeleteSuccess = "Silme İşlemi Başarılı";
    public const string DeleteFail = "Delete_Fail";
    public const string DeleteSuccessRedirect = "Delete_Success_Redirect";

    public const string StudentFoundSuccess = "Student_Found_Success";
    public const string StudentNotFound = "Student_Not_Found";
    public const string NoStudentFoundInClassroom = "No_Student_Found_In_Classroom";

    public const string StudentExamNotFound = "StudentExam_Found_Success";
    public const string StudentQuestionNotFound = "StudentQuestion_Found_Success";

    public const string CandidateFoundSuccess = "Candidate_Found_Success";
    public const string CandidateNotFound = "Candidate_Not_Found";
    public const string NoCandidateFoundInClassroom = "No_Candidate_Found_In_Classroom";

    public const string CandidateExamNotFound = "CandidateExam_Found_Success";
    public const string CandidateQuestionNotFound = "CandidateQuestion_Found_Success";

    public const string SubjectAlreadyExist = "Subject_Already_Exist";
    public const string SubjectNotFound = "Subject_Not_Found";

    public const string ClassroomFoundSuccess = "Classroom_Found_Success";
    public const string ClassroomNotFound = "Classroom_Not_Found";
    public const string ClassroomProductNotFound = "ClassroomProduct_Not_Found";

    public const string TrainerHasNoClassroom = "Trainer_Has_No_Classroom";
    public const string TrainerFoundSuccess = "Trainer_Found_Success";
    public const string TrainerNotFound = "Trainer_Not_Found";
    public const string NoTrainerFoundInClassroom = "No_Trainer_Found_In_Classroom";
    public const string EvaluatorNotAssigned = "Evaluator_Not_Assigned";

    public const string TechnicalUnitNotFound = "Technical_Unit_Not_Found";
    public const string TechnicalUnitFoundSuccess = "Technical_Unit_Found_Success";
    public const string NoSubjectToList = "No_Subject_To_List";
    public const string NoAvailableStudent = "Student_Not_Found";
    public const string NoAvailableCandidate = "Candidate_Not_Found";
    public const string NoAvailableTrainer = "No_Available_Trainer";
    public const string NoExamRuleToList = "No_Exam_Rule_To_List";
    public const string PleaseAddExamRuleBefore = "Please_Add_Exam_Rule_Before";
    public const string PleaseAddQuestionsBefore = "Please_Add_Questions_Before";

    public const string GroupTypeNotFound = "Group_Type_Not_Found";

    public const string ExamRuleAlreadyExist = "Exam_Rule_Already_Exist";
    public const string DuplicateExamRule = "Do_Not_Enter_Same_Exam_Rule";
    public const string ExamRuleNotFound = "Exam_Rule_Not_Found";
    public const string ExamRuleSubtopicNotFound = "Exam_Rule_Subject_Not_Found";
    public const string PleaseAddProductBefore = "Please_Add_Product_Before";
    public const string PleaseAddExamRuleSubject = "Add_Exam_Rule_Subject";

    public const string QuestionNotFound = "Question_Not_Found";
    public const string QuestionFoundSuccess = "Question_Found_Success";

    public const string QuestionAnswerNotFound = "Question_Answer_Not_Found";

    public const string AddAtLeastOneAnswer = "Add_At_Least_One_Answer";

    public const string BranchNotFound = "Branch_Not_Found";

    public const string ProductNotFound = "Product_Not_Found";

    public const string ExamNotFound = "Exam_Not_Found";
    public const string ExamFoundSuccess = "Exam_Found_Success";
    public const string ExamStartedSuccessfully = "Exam_Started_Successfully";
    public const string ExamStartedMailError = "Exam_Started_Mail_Error";


    public const string ExamClassroomNotFound = "Exam_Classroom_Not_Found";
    public const string ExamClassroomsRetrievedSuccessfully = "Exam_Classrooms_Retrieved_Successfully";
    public const string ExamClassroomRetrievedSuccessfully = "Exam_Classroom_Retrieved_Successfully";
    public const string ExamClassroomAlreadyExist = "Exam_Classroom_Already_Exist";
    public const string ExamClassroomUpdatedSuccessfully = "Exam_Classroom_Updated_Successfully"; 

    public const string ExamCreatedSuccessfully = "Exam_Created_Successfully";

    public const string ExamEvaluatorNotFound = "ExamEvaluator_Not_Found";

    public const string EmailNotFound = "Email_Not_Found";
    public const string EmailFoundSuccess = "Email_Found_Success";
    public const string SentEmailFoundSuccess = "Sent_Email_Found_Success";

    public const string TalentNotFound = "Talent_Not_Found";
    public const string DuplicateTalent = "Do_Not_Enter_Same_Talent";
    public const string TheExamTimeIsUp = "The_Exam_Time_Is_Up";
    public const string YouDontHaveAnExamYet = "You_Dont_Have_An_Exam_Yet";

    public const string AnExamRelatedToThisRuleHasBeenCreatedForThisClassBefore = "Bu sınıfa daha önce bu kuralla ilgili sınav oluşturulmuş.";

    //--
    public const string SetIsActiveTrue = "Set_Is_Active_True";
    public const string SetIsActiveFalse = "Set_Is_Active_False";

    public const string NullData = "Data_Not_Found";

    public const string TheQuestionTypeOfTheApprovedQuestionCannotBeChanged = "The_Question_Type_Of_The_Approved_Question_Cannot_Be_Changed";

    public const string VersionTypeNotFound = "Version_Type_Not_Found";

    public const string QuestionTypeCannotBeChanged = "Question_Type_Cannot_Be_Changed";

    public const string CityNotFound = "City_Not_Found";

    public const string SubtopicAlreadyExist = "Subtopic_Already_Exist";
    public const string SubtopicNotFound = "Subtopic_Not_Found";

    public const string StudentExamsNotFound = "The_Student_Does_Not_Have_Any_Exams";
    public const string CandidateExamsNotFound = "The_Candidate_Does_Not_Have_Any_Exams";

    public const string TrainerAssessmentStudentFail = "The_Trainer_Failed_To_Assess_Student";
    public const string TrainerAssessmentCandidateFail = "The_Trainer_Failed_To_Assess_Candidate";

    public const string EmailSendSuccess = "Email_Send_Success";
    public const string EmailSendNotFound = "Email_Send_Not_Found";
    public const string EmailSendError= "Email_Send_Error";


}