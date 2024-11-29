using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Helpers.RabbitMQ
{
    public class RabbitMQProducer
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQProducer(IOptions<RabbitMQSettings> options)
        {
            _settings = options.Value;
        }

        //Messages should match this format, but are currently not
        /*
        {
            "Method": "POST",
            "Url": "https://example.com/api/resource",
            "Headers": {
                "Authorization": "Bearer some_token",
                "Custom-Header": "value"
            },
            "Body": "{\"key\":\"value\"}"
        }*/
        public async Task PublishAsync(string message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _settings.QueueName,
                mandatory: false,
                body: body);

            Console.WriteLine($"[Producer] Sent: {message}");
        }
    }
}
