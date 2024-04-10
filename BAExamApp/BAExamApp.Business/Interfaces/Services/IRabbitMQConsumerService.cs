namespace BAExamApp.Business.Interfaces.Services;
public interface IRabbitMQConsumerService
{
    void StartSendMailNewStudent();
    void StartSendMailNewTrainer();
    void StartSendMailNewAdmin();
    void StartSendAfterExamMail();
    void StartResendStudentEmail();
    void StartSendEmailToStudentNewExam();
    void StartSendEmailToTrainerNewExam();
    void StartSendEmailToStudentAssessment();
}
