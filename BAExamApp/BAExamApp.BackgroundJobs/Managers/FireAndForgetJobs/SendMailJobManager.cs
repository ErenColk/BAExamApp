using BAExamApp.Business.Interfaces.Services;

namespace BAExamApp.BackgroundJobs.Managers.FireAndForgetJobs;
public class SendMailJobManager
{
    private readonly IRabbitMQConsumerService _rabbitMQConsumerService;

    public SendMailJobManager(IRabbitMQConsumerService rabbitMQConsumerService)
    {
        _rabbitMQConsumerService = rabbitMQConsumerService;
    }

    public void Process()
    {
        _rabbitMQConsumerService.StartSendMailNewTrainer();
        _rabbitMQConsumerService.StartSendMailNewAdmin();
        _rabbitMQConsumerService.StartSendEmailToStudentAssessment();
        _rabbitMQConsumerService.StartResendStudentEmail();
        _rabbitMQConsumerService.StartSendEmailToStudentNewExam();
        _rabbitMQConsumerService.StartSendEmailToTrainerNewExam();
    }
}
