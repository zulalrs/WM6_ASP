using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRApp
{
    public class ChatHub : Hub
    {
        public static List<User> UserList = new List<User>();   // Kullanıcıları tutacak liste
        public void HerkeseGonder(string gonderen,string mesaj) // Bu metod viewden çağrılırken baş harfi küçük olarak yazılmalı.
        {
            Clients.All.herkeseGonder(gonderen, mesaj, $"{DateTime.Now:g}");    // All nesnesi dinamik bir nesne olduğundan . dediklten sonra istediğimiz şeyi yazabiliyoruz ve view tarafında o isimle çağırıyoruz. (Client tarafındaki metod)
        }

        public void OzelMesaj(string gonderenId, string aliciId, string mesaj) // Parametre değerlerimiz view tarfından gelecek.
        {
            Clients.Client(aliciId).mesajGeldi(gonderenId, mesaj); // Bu metodun sonucu viewda gosterilecek.
        }

        public void Login(string kullaniciAdi, string id) // Login işlemi için viewden gelen kullanıcı adı ve idyi alacak
        {
            var giris = UserList.FirstOrDefault(x => x.Id == id);    // Kişinin idsi listede varsa girilen kullanıcı adını o kişinin kullanıcı adı olarak atıyor.
            giris.UserName = kullaniciAdi;

            Clients.All.users(UserList); // Listeyi tekrar view a gonderiyor.
        }

        public override Task OnConnected()  // Sisteme biri bağlandığı zaman o kişiyi bağlantı id sini kullanarak ekle
        {
            UserList.Add(new User()
            {
                Id = Context.ConnectionId // User ın id sine bağlantının idsini ver
            });
            return base.OnConnected(); // Base yazılabiliyorsa abstract değildir.
        }

        public override Task OnDisconnected(bool stopCalled)    // Biri bağlantıdan çıktığında yani sayfayı kapattığında
        {
            var silinecek = UserList.FirstOrDefault(x => x.Id == Context.ConnectionId); // User listesi içerisinde idsi bağlantı idsine eşit olan kullanıcıyı bul
            UserList.Remove(silinecek); // Kullanıcı listesinden sil
            Clients.All.users(UserList); // View tarafına listeyi tekrar gonder.
            return base.OnDisconnected(stopCalled);
        }
    }

    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}