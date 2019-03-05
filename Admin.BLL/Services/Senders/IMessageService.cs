using Admin.Models.Enums;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.BLL.Services.Senders
{
    // Bütün mesajları bu servis üzerinden oluşturucaz.
    public interface IMessageService
    {
        MessageStates MessageState { get; } // Sadece okunur bir messagestate. İletildi, iletilmedi ifadelerini tutuyor.


        // Bazı yerlerde asenkron metodları çağıramıyoruz bunun için bir de senkron metodu yazdık. Senkron metodu içerisinde asenkron metodu çağıracagız. Message gonderecegimiz mesajı, contacts ta kimlere gonderilecegini tutuyor.
        Task SendAsync(IdentityMessage message, params string[] contacts);  //Asenkron gonder metodu
        void Send(IdentityMessage message,params string[] contacts);    // Senkron gönder metodu
    }
}
