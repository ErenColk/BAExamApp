using RabbitMQ.Client;

namespace BAExamApp.Business.Interfaces.Services;
public interface IRabbitMQService
{
    /// <summary>
    /// Bu metot, RabbitMQ bağlantısını almak için kullanılır. RabbitMQ bağlantısı, RabbitMQ sunucusuna bağlanmak için kullanılır.
    /// </summary>
    /// <returns>IConnection nesnesi döndürür.</returns>
    IConnection GetConnection();

    /// <summary>
    /// Bu metot, bir bağlantıya dayalı olarak bir model almak için kullanılır. Model, belirli bir bağlantı üzerinden RabbitMQ mesajlarını göndermek ve almak için kullanılır.
    /// </summary>
    /// <param name="connection"></param>
    /// <returns>IModel nesneni döndürür.</returns>
    IModel GetModel(IConnection connection);
}
