using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BAExamApp.Business.Services;
public class RabbitMQPublishService : IRabbitMQPublishService
{
    private readonly IRabbitMQService _rabbitMQService;

    public RabbitMQPublishService(IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public void EnqueueModels<T>(IEnumerable<T> queueDataModels, string queueName) where T : class, new()
    {
        try
        {
            using (var connection = _rabbitMQService.GetConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false, 
                                     autoDelete: false, 
                                     arguments: null); 

                foreach (var queueDataModel in queueDataModels)
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queueDataModel));
                    channel.BasicPublish(exchange: "",
                        routingKey: queueName,
                        basicProperties: null,
                        body: body);
                }
            }
        }
        catch (Exception ex)
        {

            throw new Exception(ex.InnerException.Message);
        }
    }

    public void EnqueueModel<T>(T queueDataModel, string queueName) where T : class, new()
    {
        try
        {
            using (var connection = _rabbitMQService.GetConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true, // durable: ile in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir.
                                     exclusive: false, // exclusive: Yalnızca bir bağlantı tarafından kullanılır ve bu bağlantı                                     kapandığında sıra silinir — özel olarak işaretlenirse silinmez
                                     autoDelete: false, // autoDelete: En son bir abonelik iptal edildiğinde en az bir müşteriye sahip                              olan kuyruk silinir
                                     arguments: null); // arguments: İsteğe bağlı; eklentiler tarafından kullanılır ve TTL mesajı,                                 kuyruk uzunluğu sınırı, vb. özellikler tanımlanır.


                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(queueDataModel));
                channel.BasicPublish(exchange: "",
                    routingKey: queueName,
                    basicProperties: null,
                    body: body);

            }
        }
        catch (Exception ex)
        {

            throw new Exception(ex.InnerException.Message);
        }
    }
}
