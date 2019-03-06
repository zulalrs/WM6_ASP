using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client.Events;

namespace Rabbit.Consumer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static Consumer _consumer;
        public static long logCustomer = 0, logMailLog = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            _consumer = new Consumer("Customer");
            _consumer.ConsumerEvent.Received += ConsumerEvent_Received;
            ConsumerEvent_Received(sender, new BasicDeliverEventArgs());
        }

        private void ConsumerEvent_Received(object sender, BasicDeliverEventArgs e)
        {
        }
    }
}
