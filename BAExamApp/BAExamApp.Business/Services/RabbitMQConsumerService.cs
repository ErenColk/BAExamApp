using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using BAExamApp.Dtos.SendMails;
using BAExamApp.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using BAExamApp.Dtos.SentMails;
using Microsoft.Extensions.Configuration;

namespace BAExamApp.Business.Services;
public class RabbitMQConsumerService : IRabbitMQConsumerService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IObjectConvertFormatService _objectConvertFormatService;
    private readonly ISendMailService _sendMailService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IModel _channel;
    private EventingBasicConsumer _consumer;
    public RabbitMQConsumerService(IRabbitMQService rabbitMQService, IObjectConvertFormatService objectConvertFormatService, ISendMailService sendMailService, IMapper mapper, IConfiguration configuration)
    {
        _rabbitMQService = rabbitMQService;
        _objectConvertFormatService = objectConvertFormatService;
        _sendMailService = sendMailService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public void StartSendAfterExamMail()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.AfterExamMail,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedAfterExam;
            _channel.BasicConsume(queue: RabbitMQQueueNames.AfterExamMail,
                                       autoAck: true,
                                       /* autoAck: bir mesajı aldıktan sonra bunu anladığına       
                                          dair(acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda 
                                          mesajı geri çevirmek(Discard) veya yeniden kuyruğa aldırmak(Re-Queue) için dönüşler yapar*/
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedAfterExam(object sender, BasicDeliverEventArgs ea)
    {
        AfterExamMailDto afterExamMailDto = _objectConvertFormatService.JsonToObject<AfterExamMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
         _sendMailService.SendAfterExamMail(afterExamMailDto);
        Console.WriteLine(afterExamMailDto.StudentFullName + " " + afterExamMailDto.Email + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartSendEmailToStudentAssessment()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.EmailToStudentAssessment,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += Consumer_ReceivedEmailToStudentAssesment;
            _channel.BasicConsume(queue: RabbitMQQueueNames.EmailToStudentAssessment,
                                       autoAck: true,
                                       /* autoAck: bir mesajı aldıktan sonra bunu anladığına       
                                          dair(acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda 
                                          mesajı geri çevirmek(Discard) veya yeniden kuyruğa aldırmak(Re-Queue) için dönüşler yapar*/
                                       consumer: _consumer);
        }
        catch (Exception ex)
        {

            throw new Exception(ex.InnerException.Message);
        }
    }
   
    private void Consumer_ReceivedEmailToStudentAssesment(object sender, BasicDeliverEventArgs ea)
    {
        StudentAssesmentMailDto studentAssesmentMailDto = _objectConvertFormatService.JsonToObject<StudentAssesmentMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        SentMailCreateDto sentMailCreateDto = _sendMailService.SendEmailToStudentAssessment(studentAssesmentMailDto).Result;
        AddSentMail(sentMailCreateDto);
        Console.WriteLine(studentAssesmentMailDto.StudentEmailAddress + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartSendEmailToStudentNewExam()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.EmailToStudentNewExam,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedEmailToStudentNewExam;
            _channel.BasicConsume(queue: RabbitMQQueueNames.EmailToStudentNewExam,
                                       autoAck: true,
                                       /* autoAck: bir mesajı aldıktan sonra bunu anladığına       
                                          dair(acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda 
                                          mesajı geri çevirmek(Discard) veya yeniden kuyruğa aldırmak(Re-Queue) için dönüşler yapar*/
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedEmailToStudentNewExam(object sender, BasicDeliverEventArgs ea)
    {
        StudentNewExamMailDto studentNewExamMailDto = _objectConvertFormatService.JsonToObject<StudentNewExamMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        SentMailCreateDto sentMailCreateDto = _sendMailService.SendEmailToStudentNewExam(studentNewExamMailDto).Result;
        AddSentMail(sentMailCreateDto);
        Console.WriteLine(studentNewExamMailDto.EmailAdress + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartSendEmailToTrainerNewExam()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.EmailToTrainerNewExam,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedEmailToTrainerNewExam;
            _channel.BasicConsume(queue: RabbitMQQueueNames.EmailToTrainerNewExam,
                                       autoAck: true,
                                       /* autoAck: bir mesajı aldıktan sonra bunu anladığına       
                                          dair(acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda 
                                          mesajı geri çevirmek(Discard) veya yeniden kuyruğa aldırmak(Re-Queue) için dönüşler yapar*/
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedEmailToTrainerNewExam(object sender, BasicDeliverEventArgs ea)
    {
        TrainerNewExamMailDto trainerNewExamMailDto = _objectConvertFormatService.JsonToObject<TrainerNewExamMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        _sendMailService.SendEmailToTrainerNewExam(trainerNewExamMailDto);
        Console.WriteLine(trainerNewExamMailDto.TrainerEmailAdress + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartSendMailNewAdmin()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.EmailNewAdmin,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedMailNewAdmin;
            _channel.BasicConsume(queue: RabbitMQQueueNames.EmailNewAdmin,
                                       autoAck: true,
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedMailNewAdmin(object sender, BasicDeliverEventArgs ea)
    {
        NewUserMailDto newUserMailDto = _objectConvertFormatService.JsonToObject<NewUserMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        _sendMailService.SendEmailNewAdmin(newUserMailDto);
        Console.WriteLine("Admin -" + newUserMailDto.Email + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartSendMailNewStudent()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.EmailNewStudent,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedMailNewStudent;
            _channel.BasicConsume(queue: RabbitMQQueueNames.EmailNewStudent,
                                       autoAck: true,
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedMailNewStudent(object sender, BasicDeliverEventArgs ea)
    {
        NewUserMailDto emailNewUserDto = _objectConvertFormatService.JsonToObject<NewUserMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        SentMailCreateDto sentMailCreateDto = _sendMailService.SendEmailNewStudent(emailNewUserDto).Result;
        AddSentMail(sentMailCreateDto);
        Console.WriteLine("Öğrenci - " + emailNewUserDto.Email + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartSendMailNewTrainer()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.EmailNewTrainer,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedMailNewTrainer;
            _channel.BasicConsume(queue: RabbitMQQueueNames.EmailNewTrainer,
                                       autoAck: true,
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedMailNewTrainer(object sender, BasicDeliverEventArgs ea)
    {
        NewUserMailDto emailNewUserDto = _objectConvertFormatService.JsonToObject<NewUserMailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        _sendMailService.SendEmailNewTrainer(emailNewUserDto);
        Console.WriteLine("Trainer - " + emailNewUserDto.Email + " adresine mail gönderildi." + $" {DateTime.Now}");
    }

    public void StartResendStudentEmail()
    {
        try
        {
            _connection = _rabbitMQService.GetConnection();
            _channel = _rabbitMQService.GetModel(_connection);
            _channel.QueueDeclare(queue: RabbitMQQueueNames.ResendStudentEmail,
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += ConsumerReceivedResendStudent;
            _channel.BasicConsume(queue: RabbitMQQueueNames.ResendStudentEmail,
                                       autoAck: true,
                                       consumer: _consumer);
        }
        catch (Exception)
        {

            throw;
        }
    }
    private void ConsumerReceivedResendStudent(object sender, BasicDeliverEventArgs ea)
    {
        ResendStudentEmailDto resendStudentEmailDto = _objectConvertFormatService.JsonToObject<ResendStudentEmailDto>(Encoding.UTF8.GetString(ea.Body.ToArray()));
        SentMail sentMail = GetSentMail(resendStudentEmailDto.SentMailId);
        SentMail updateSentMail = _sendMailService.ResendMail(sentMail).Result;
        UpdateSentMail(updateSentMail);
        Console.WriteLine(updateSentMail.Email + " adresine mail gönderildi." + $" {DateTime.Now}");
    }
    private void AddSentMail(SentMailCreateDto sentMailCreateDto)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<BAExamAppDbContext>();

        dbContextBuilder.UseSqlServer(_configuration.GetConnectionString(BAExamAppDbContext.ConnectionName));
        var sentMail = _mapper.Map<SentMail>(sentMailCreateDto);
        using (BAExamAppDbContext context = new(dbContextBuilder.Options))
        {
            context.SentMails.Add(sentMail);
            context.SaveChanges();
        }
    }

    private void UpdateSentMail(SentMail sentMail)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<BAExamAppDbContext>();

        dbContextBuilder.UseSqlServer(_configuration.GetConnectionString(BAExamAppDbContext.ConnectionName));

        using (BAExamAppDbContext context = new(dbContextBuilder.Options))
        {
            context.SentMails.Update(sentMail);
            context.SaveChanges();
        }
    }

    private SentMail GetSentMail(Guid sentMailId)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<BAExamAppDbContext>();

        dbContextBuilder.UseSqlServer(_configuration.GetConnectionString(BAExamAppDbContext.ConnectionName));      
        using (BAExamAppDbContext context = new(dbContextBuilder.Options))
        {
            return context.SentMails.Find(sentMailId);
        }
    }
}
