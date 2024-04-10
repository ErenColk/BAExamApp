namespace BAExamApp.Business.Interfaces.Services;
public interface IRabbitMQPublishService
{
    void EnqueueModels<T>(IEnumerable<T> queueDataModels, string queueName) where T : class, new();
    void EnqueueModel<T>(T queueDataModel, string queueName) where T : class, new();
}
