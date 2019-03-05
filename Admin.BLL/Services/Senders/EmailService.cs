using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Admin.Models.Enums;
using Microsoft.AspNet.Identity;

namespace Admin.BLL.Services.Senders
{
    // IMessageState interface i genel olarak tüm mesajlarda kullanılacak propertyleri içeriyor.
    public class EmailService : IMessageService
    {
        // EmailService sınıfı email işlemlerine ait propertyleri içeriyor.
        private string _userId = HttpContext.Current.User.Identity.GetUserId();
        public string[] Cc { get; set; }    // Bilgi alanı
        public string[] Bcc { get; set; }   // Gizli bilgi alanı
        public string FilePath { get; set; }    // Eger attach ile bir dosya gonderilecekse filepath propertysini kullanıyoruz.
        public MessageStates MessageState { get; set; }
        public string SenderMail { get; set; }  // Kim gonderecek
        public string Password { get; set; }    // Gonderecek olanın şifresi
        public string Smtp { get; set; }    // Smtp adresi giden posta sunucusu
        public int SmtpPort { get; set; }   // Smtp portu


        public EmailService()
        {
            // Sabit kullanılacak mail
            this.SenderMail = "wissenakademie.wm6@gmail.com";
            this.Password = "qweqweasdasd123";
            this.Smtp = "smtp.gmail.com";   // Giden posta sunucusu
            this.SmtpPort = 587;    // Bu port üzerinden mail gonderilecek
        }
        // Birden fazla mail adresi kullandığımız durumlarda gerekli olacak constructor
        public EmailService(string senderMail,string password,string smtp,int smtpPort)
        {
            this.SenderMail = senderMail;
            this.Password = password;
            this.Smtp = smtp; 
            this.SmtpPort =smtpPort;
        }


        public async Task SendAsync(IdentityMessage message, params string[] contacts)
        {
            var userID = _userId ?? "system";
            try
            {
                var mail = new MailMessage { From = new MailAddress(this.SenderMail) }; // Gonderen mailini , maildeki from kısmına oluşturuyoruz.

                if (!string.IsNullOrEmpty(FilePath))    // Eger mail oluşturulurken bir attach file verildiyse o attach file ı maile ek olarak ekliyoruz.
                {
                    mail.Attachments.Add(new Attachment(FilePath));
                }

                foreach (var c in contacts) // Birden fazla contact bilgisi aldıgımzda her birini alıcı kısmına ekliyoruz.
                {
                    mail.To.Add(c);
                }
                if (Cc != null && Cc.Length > 0)    // Cc kısmına mail adresi ekleme işlemi
                {
                    foreach (var cc in Cc)
                    {
                        mail.CC.Add(new MailAddress(cc));
                    }
                }

                if (Bcc != null && Bcc.Length > 0)  // Bcc kısmına mail adresi ekleme işlemi
                {
                    foreach (var bcc in Bcc)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                }

                mail.Subject = message.Subject; // Mailin konusu
                mail.Body = message.Body;   // Mailin içeriği

                mail.IsBodyHtml = true; // Bu maili gonderirken html taglarının render edilip edilmeyecegini ayarlama işlemi

                // Body,Subject ve Header kısımlarında türkçe karakter sıkıntılarıyla uğraşmamak için Utf8 yaptık.
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.HeadersEncoding = Encoding.UTF8;

                var smptClient = new SmtpClient(this.Smtp, this.SmtpPort)   // Client oluşturduk.
                {
                    Credentials = new NetworkCredential(this.SenderMail, this.Password),    // Hesap oluşturma
                    EnableSsl = true    // Güvenli bağlantı
                };
                await smptClient.SendMailAsync(mail);   // Client sayesınd maili gonderecegiz.
                MessageState = MessageStates.Delivered; // Bir hata çıkmazsa Message state imizi gonderildi yapıyoruz
            }
            catch (Exception ex)
            {
                MessageState = MessageStates.NotDelivered;  // Hata durumunda gonderilmedi yapıyoruz.
            }
        }
        public void Send(IdentityMessage message, params string[] contacts)
        {
            // Task nesnesinin run metodunu çalıştırıyoruz. Asnkron bir metod çalıştıracagımızı soyluyoruz
            Task.Run(async () =>    // Burada bir delege çalışıyor
            {
                await this.SendAsync(message, contacts);    // Burada Senkron olanı çağırıp, Asenkron olan Send metodunun çalışmasını saglıyoruz. 
            });
        }

       
    }
}
