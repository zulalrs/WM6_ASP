using Newtonsoft.Json;
using Rabbit.BLL.RabbitMq;
using Rabbit.Models.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Consumer
{
    public class Consumer
    {
        private readonly RabbitMqService _rabbitMqService;
        public EventingBasicConsumer ConsumerEvent;

        public Consumer(string queueName)
        {
            _rabbitMqService = new RabbitMqService();
            var connection = _rabbitMqService.GetRabbitMqConnection();
            var channel = connection.CreateModel();
            ConsumerEvent = new EventingBasicConsumer(channel);

            ConsumerEvent.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                if (queueName == "MailLog")
                {
                    var data = JsonConvert.DeserializeObject<List<MailLog>>(message);
                }
                else if(queueName=="Customer")
                {
                    var data = JsonConvert.DeserializeObject<List<MailLog>>(message);
                }
            };
            channel.BasicConsume(queueName, true, ConsumerEvent);
        }
    }
}
