using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.Exams;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.SentMails;
using BAExamApp.Dtos.Students;
using BAExamApp.Entities.DbSets;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace BAExamApp.Business.Services
{


    public class SendMailService : ISendMailService
    {
        private readonly IOptions<EmailConfigurationDto> _configuration;
        private readonly ISentMailService _sentMailService;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly ITrainerRepository _trainerRepository;

        public SendMailService(IStudentExamRepository studentExamRepository, ITrainerRepository tarnierRepository, IStudentRepository studentRepository, IOptions<EmailConfigurationDto> configuration, ISentMailService sentMailService, IMapper mapper)
        {
            _configuration = configuration;
            _sentMailService = sentMailService;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _trainerRepository = tarnierRepository;
            _studentExamRepository = studentExamRepository;
        }

        /// <summary>
        /// E-posta'ya gönderilmek üzere 6 haneli rastgele bir sayı üretir.
        /// </summary>
        /// <returns>Gönderilen güvenlik kodu</returns>
        private int GenerateVerificationCode()
        {
            Random code = new Random();
            return code.Next(100000, 999999);
        }

        /// <summary>
        /// Gönderilecek mesajın içeriğinin oluşturulması için kullanılır.
        /// </summary>
        /// <param name="message">Gönderilecek mailin içeriği</param>
        /// <returns>Gönderilecek Mail İçeriği</returns>
        private async Task<MailMessage> CreateEmailContent(MailMessageDto message)
        {
            var emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(_configuration.Value.From);
            emailMessage.To.Add(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = message.Content;

            return emailMessage;
        }

        /// <summary>
        /// Oluşturulan mesajın gönderilmesi işlemi gerçekleştirilir.
        /// </summary>
        /// <param name="message">Gönderilecek mesaj</param>
        private async Task SendMail(MailMessageDto message)
        {
            var mailMessage = await CreateEmailContent(message);

            using (var client = new SmtpClient(_configuration.Value.SmtpServer, _configuration.Value.Port))
            {
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(_configuration.Value.From, _configuration.Value.Password);
                client.EnableSsl = true;

                client.Send(mailMessage);

            }
        }


        /// <summary>
        /// Gönderilmeyen maili id'ye göre tekrar gönderir
        /// </summary>
        /// <param name="id">Gönderilmeyen mailin id'si</param>
        /// <returns>Gönderim başarılı olursa  SentMailDto modeli tipinde değer döner</returns>
        public async Task<SentMail> ResendMail(SentMail sentMail)
        {

            MailMessageDto message = new MailMessageDto(sentMail.Email, sentMail.Subject, sentMail.Content);

            try
            {
                await SendMail(message);
                sentMail.IsSuccess = true;
                return sentMail;
            }
            catch (Exception)
            {
                return sentMail;
            }

        }

        /// <summary>
        /// Giriş yapan kullanıcının iki adımlı doğrulamadaki güvenlik kodunun gönderilmesi için kullanılır.
        /// </summary>
        /// <param name="email">Mailin gönderileceği mail adresi</param>
        /// <returns>Gönderilen güvenlik kodu</returns>
        public async Task<int> SendEmailVerificationCode(string email)
        {
            int verificationCode = GenerateVerificationCode();
            MailMessageDto message = new MailMessageDto(email, "Bilge Adam Giriş Şifresi", $"Giriş Şifreniz {verificationCode}");

            await SendMail(message);
            return verificationCode;
        }

        /// <summary>
        /// Yeni oluşturulan öğrenci hesabının bilgilerinin gönderilmesi için kullanırlır.
        /// </summary>
        /// <param name="email">Mailin gönderileceği mail adresi</param>
        /// <param name="url">Login sayfasının url adresi</param>
        /// <returns></returns>
        public async Task<SentMailCreateDto> SendEmailNewStudent(NewUserMailDto newUserMailDto)
        {
            MailMessageDto message = new MailMessageDto(newUserMailDto.Email, "Bilge Adam'a Hoşgeldiniz", $"Yeni hesabınızla giriş yapmak için aşağıdaki linke tıklayabilirsiniz.\n{newUserMailDto.Url} \n Giriş Bilgileriniz \nEmail : {newUserMailDto.Email} \nŞifre : newPassword+0");

            SentMailCreateDto sentMailCreateDto = _mapper.Map<SentMailCreateDto>(message);
            try
            {
                await SendMail(message);
                sentMailCreateDto.IsSuccess = true;
            }
            catch (Exception)
            {
                sentMailCreateDto.IsSuccess = false;
                throw;
            }
            return sentMailCreateDto;

        }
        /// <summary>
        /// Yeni oluşturulan eğitmen hesabının bilgilerinin gönderilmesi için kullanırlır.
        /// </summary>
        /// <param name="email">Mailin gönderileceği mail adresi</param>
        /// <param name="url">Login sayfasının url adresi</param>
        /// <returns></returns>
        public async Task SendEmailNewTrainer(NewUserMailDto newUserMailDto)
        {
            MailMessageDto message = new MailMessageDto(newUserMailDto.Email, "Bilge Adam'a Hoşgeldiniz", $"Yeni hesabınızla giriş yapmak için aşağıdaki linke tıklayabilirsiniz.\n{newUserMailDto.Url} \n Giriş Bilgileriniz \nEmail : {newUserMailDto.Email} \nŞifre : newPassword+0");
            await SendMail(message);
        }

        /// <summary>
        /// Yeni oluşturulan admin hesabının bilgilerinin gönderilmesi için kullanırlır.
        /// </summary>
        /// <param name="email">Mailin gönderileceği mail adresi</param>
        /// <param name="url">Login sayfasının url adresi</param>
        /// <returns></returns>
        public async Task SendEmailNewAdmin(NewUserMailDto newUserMailDto)
        {
            MailMessageDto message = new MailMessageDto(newUserMailDto.Email, "Bilge Adam'a Hoşgeldiniz", $"Yeni hesabınızla giriş yapmak için aşağıdaki linke tıklayabilirsiniz.\n{newUserMailDto.Url} \n Giriş Bilgileriniz \nEmail : {newUserMailDto.Email} \nŞifre : newPassword+0");
            await SendMail(message);
        }

        /// <summary>
        /// Belirtilen mail adresine sınav raporu göndermek için kullanılır.
        /// </summary>
        /// <param name="email">Mailin gönderileceği mail adresi</param>
        /// <returns></returns>
        public async Task SendAfterExamMail(AfterExamMailDto afterExamMailDto)
        {
            TimeSpan timeElapsed = TimeSpan.FromSeconds(afterExamMailDto.TotalTimeSpent);
            string timeElapsedFormatted = timeElapsed.ToString(@"m\:ss");

            MailMessageDto message = new MailMessageDto(afterExamMailDto.Email, $"{afterExamMailDto.ExamName} adlı öğrencinin sınavı hakkında", $"{afterExamMailDto.StudentFullName} isimli öğrenci {afterExamMailDto.ExamName} sınavını {timeElapsedFormatted} sürede tamamlamıştır. Öğrenci notu: {afterExamMailDto.StudentPoint}");
            await SendMail(message);
        }

        /// <summary>
        /// Öğrencinin oluşturulan yeni sınava girebilmesi için gerekli bilgileri ve linki gönderir. 
        /// </summary>
        /// <param name="student">Öğrenci bilgileirini içerir</param>
        /// <param name="url">Sınav sayfasının url adresi</param>
        /// <returns></returns>

        public async Task<SentMailCreateDto> SendEmailToStudentNewExam(StudentNewExamMailDto studentNewExamMailDto)
        {
            string emailSubject = $"{studentNewExamMailDto.ExamName} Sınavı Hakkında Bilgilendirme";
            string formattedExamDuration = "";
            if (studentNewExamMailDto.ExamDuration.Hours > 0 & studentNewExamMailDto.ExamDuration.Minutes > 0)
            {
                formattedExamDuration = studentNewExamMailDto.ExamDuration.Hours.ToString("00") + ":" + studentNewExamMailDto.ExamDuration.Minutes.ToString("00") + " " + "(" + studentNewExamMailDto.ExamDuration.Hours + " saat " + studentNewExamMailDto.ExamDuration.Minutes + " dakika)";
            }
            else if (studentNewExamMailDto.ExamDuration.Hours > 0 & studentNewExamMailDto.ExamDuration.Minutes == 0)
            {
                formattedExamDuration = studentNewExamMailDto.ExamDuration.Hours.ToString("00") + ":" + studentNewExamMailDto.ExamDuration.Minutes.ToString("00") + " " + "(" + studentNewExamMailDto.ExamDuration.Hours + " saat)";
            }
            else if (studentNewExamMailDto.ExamDuration.Hours == 0 & studentNewExamMailDto.ExamDuration.Minutes > 0)
            {
                formattedExamDuration = studentNewExamMailDto.ExamDuration.Hours.ToString("00") + ":" + studentNewExamMailDto.ExamDuration.Minutes.ToString("00") + " " + "(" + studentNewExamMailDto.ExamDuration.Minutes + " dakika)";
            }

            string emailBody = $"Merhaba,\n\n" +
                               $"Yaklaşan '{studentNewExamMailDto.ExamName}' sınavınız hakkında sizi bilgilendirmek isteriz. Sınav detayları aşağıda yer almaktadır:\n\n" +
                               $"Sınav Tarihi: {studentNewExamMailDto.ExamDate.ToString("dd.MM.yyyy HH:mm")}\n" +
                               $"Sınav Süresi: {formattedExamDuration}\n\n" +
                               $"Sınava giriş yapmak için lütfen aşağıdaki linke tıklayın:\n" +
                               $"{studentNewExamMailDto.Url}/{studentNewExamMailDto.StudentExamId}\n\n" +
                               $"Sınavınız hakkında herhangi bir sorunuz olursa, lütfen bizimle iletişime geçin.\n\n" +
                               $"Başarılar dileriz,\n" +
                               $"Eğitim Ekibiniz";

            MailMessageDto message = new MailMessageDto(studentNewExamMailDto.EmailAdress, emailSubject, emailBody);

            SentMailCreateDto sentMailCreateDto = _mapper.Map<SentMailCreateDto>(message);
            try
            {
                await SendMail(message);
                sentMailCreateDto.IsSuccess = true;
            }
            catch (Exception)
            {
                sentMailCreateDto.IsSuccess = false;
                throw;
            }

            return sentMailCreateDto;

        }




        public async Task<string> GetStudentEmailById(Guid studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            return student?.Email;
        }



        public async Task SendEmailToTrainerNewExam(TrainerNewExamMailDto trainerNewExamMailDto)
        {

            if (trainerNewExamMailDto.StudentContents != null && !string.IsNullOrEmpty(trainerNewExamMailDto.TrainerEmailAdress))
            {
                //var concatenatedContents = string.Join("\n", studentContents);
                //var trainerMessage = new MailMessageDto(trainerEmailAdress, "Yeni Sınav Oluşturuldu", $"Aşağıdaki öğrencilere mail gönderildi:\n{concatenatedContents}");
                var trainerMessage = new MailMessageDto(trainerNewExamMailDto.TrainerEmailAdress, trainerNewExamMailDto.StudentContents[0].Split("*?*")[0] + " " + "Sınavı Oluşturuldu", CreateHtmlBody(trainerNewExamMailDto.StudentContents));
                await SendMailWithHtml(trainerMessage);
            }
            else
            {
                throw new Exception("Öğrenci bilgisi bulunamadı veya eğitmen mail adresi eksik.");
            }
        }

        private string CreateHtmlBody(List<string> studentContents)
        {


            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<!DOCTYPE html>");
            htmlBuilder.Append("<html>");
            htmlBuilder.Append("<head>");
            htmlBuilder.Append("<title>Body Builder</title>");
            htmlBuilder.Append("<style>");
            htmlBuilder.Append("table {   border-collapse: collapse;    background-color: #fff;  }");
            htmlBuilder.Append("table tr, table td, table th {background-color:#ffffff; border: 1px solid #bbb;   padding: 10px 20px;  }");
            htmlBuilder.Append("table th {background-color:#40bfed; color:#fff; font-weight: 600;  }");
            htmlBuilder.Append("</style>");
            htmlBuilder.Append("</head>");
            htmlBuilder.Append("<body>");
            htmlBuilder.Append("<p><b>İlgili sınava ait bilgiler aşağıda verilmiştir:</b></p>");
            htmlBuilder.Append("<table border=\"1\">");
            htmlBuilder.Append("<tr><th>Sınav Adı</th><th>Sınıf Adı</th><th>Sınav Tarihi</th><th>Email</th><th>Sınav Linki</th></tr>");


            for (int i = 0; i < studentContents.Count; i++)
            {
                string[] examResult = studentContents[i].Split("*?*");
                htmlBuilder.Append("<tr>");
                htmlBuilder.Append("<td>").Append(examResult[0]).Append("</td>");
                htmlBuilder.Append("<td>").Append(examResult[1]).Append("</td>");
                htmlBuilder.Append("<td>").Append(examResult[2]).Append("</td>");
                htmlBuilder.Append("<td>").Append(examResult[3]).Append("</td>");
                htmlBuilder.Append("<td>").Append(examResult[4]).Append("</td>");

                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append("</table>");
            htmlBuilder.Append("</body>");
            htmlBuilder.Append("</html>");

            string htmlOutput = htmlBuilder.ToString();

            return htmlOutput;
        }

        private async Task SendMailWithHtml(MailMessageDto message)
        {
            var mailMessage = await CreateEmailContentWithHtml(message);

            using (var client = new SmtpClient(_configuration.Value.SmtpServer, _configuration.Value.Port))
            {
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(_configuration.Value.From, _configuration.Value.Password);
                client.EnableSsl = true;

                client.Send(mailMessage);

            }
        }

        private async Task<MailMessage> CreateEmailContentWithHtml(MailMessageDto message)
        {
            var emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(_configuration.Value.From);
            emailMessage.To.Add(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.IsBodyHtml = true;
            emailMessage.Body = message.Content;

            return emailMessage;
        }

        /// <summary>
        /// Eğitmen tarafından öğrencinin sınav sonucu değerlendirmesini göndermek için kullanılır.
        /// </summary>
        /// <param name="assessment">Yapılan değerlendirme</param>
        /// <param name="studentEmailAddress">Öğrencinin mail adresi</param>
        /// <param name="examName">Değerlendirme yapılan sınav</param>
        /// <param name="trainerName">Değerlendirme yapan eğitmenin adı</param>
        /// <returns></returns>
        public async Task<SentMailCreateDto> SendEmailToStudentAssessment(StudentAssesmentMailDto studentAssesmentMailDto)
        {
            MailMessageDto message = new MailMessageDto(studentAssesmentMailDto.StudentEmailAddress, $"{studentAssesmentMailDto.ExamName} Sınav Sonucu Değerlendirmesi", $"{studentAssesmentMailDto.Assessment} \n\n{studentAssesmentMailDto.TrainerName}");
            SentMailCreateDto sentMailCreateDto = _mapper.Map<SentMailCreateDto>(message);
            try
            {
                await SendMail(message);
                sentMailCreateDto.IsSuccess = true;
            }
            catch (Exception)
            {

                sentMailCreateDto.IsSuccess = false;
                throw;
            }
            return sentMailCreateDto;
        }

    }
}