using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.BLL.RabbitMq
{
    public class RabbitMqService
    {
        private readonly string _hostName = "localhost",
            _userName = "zulal",
            _password = "123456";

        public IConnection GetRabbitMqConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                // RabbitMQ'nun bağlantı kuracağı host'u tanımlıyoruz. Herhangi bir güvenlik önlemi koymak istersek, Management ekranından password adımlarını tanımlayıp factory içerisindeki "UserName" ve "Password" property'lerini set etmemiz yeterlidir.
                HostName = _hostName,
                VirtualHost="/",
                UserName= _userName,
                Password=_password,
                Uri= new Uri($"amqp://{_userName}:{_password}@{_hostName}")
            };
            return connectionFactory.CreateConnection();
        }
    }
}
