
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace BAExamApp.Business.Services;
public class RabbitMQService : IRabbitMQService
{
    private readonly IConfiguration _configuration;

    public RabbitMQService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    /// <summary>
    /// Bu metot, RabbitMQ bağlantısını almak için kullanılır. RabbitMQ bağlantısı, RabbitMQ sunucusuna bağlanmak için kullanılır.
    /// </summary>
    /// <returns></returns>
    public IConnection GetConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_configuration.GetSection("RabbitMQConfiguration:Uri").Value)
            };

            factory.AutomaticRecoveryEnabled = true; // Otomatik bağlantıyı etkinleştirmek için.

            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10); // Her 10 saniye de bir tekrar bağlanmaya çalışır.

            factory.TopologyRecoveryEnabled = false; // Sunucudan bağlantısı kesildikten sonra kuyruktaki mesaj tüketimini sürdürmez.

            return factory.CreateConnection();
        }
        catch (Exception)
        {
            Thread.Sleep(5000);

            return GetConnection(); // Tekrar bağlanmayı deniyeceğiz.
        }
    }
    /// <summary>
    /// Bu metot, bir bağlantıya dayalı olarak bir model almak için kullanılır. Model, belirli bir bağlantı üzerinden RabbitMQ mesajlarını göndermek ve almak için kullanılır.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns></returns>
    public IModel GetModel(IConnection connection)
    {
        return connection.CreateModel();
    }
}
