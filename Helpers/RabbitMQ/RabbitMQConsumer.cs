using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.RabbitMQ
{
    public class RabbitMQConsumer
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> options)
        {
            _settings = options.Value;
        }

        public async Task StartListeningAsync(string queueName = "")
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            string whichQueue = queueName != "" ? queueName : _settings.QueueName;
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // Ensure the queue exists
            await channel.QueueDeclareAsync(
                queue: whichQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[Consumer] Listening to queue: {whichQueue}");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    //var request = JsonSerializer.Deserialize<RestRequest>(message);

                    Console.WriteLine($"[Consumer] Received: {message}");
                    await ProcessMessageAsync(message);

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Consumer] Error processing message: {ex.Message}");
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            // Start consuming messages from the queue
            await channel.BasicConsumeAsync(
                queue: whichQueue,
                autoAck: false, // Set to false to manually acknowledge messages
                consumer: consumer);

        }

        public async Task ProcessMessageAsync(string requestJson)
        {
            try
            {
                var requestDetails = JsonSerializer.Deserialize<RequestDetails>(requestJson);

                if (requestDetails == null)
                {
                    Console.WriteLine("[Consumer] Invalid request details.");
                    return;
                }

                var client = new RestClient();
                var request = new RestRequest(requestDetails.Url, requestDetails.Method);

                if (requestDetails.Headers != null)
                {
                    foreach (var header in requestDetails.Headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }

                if (!string.IsNullOrEmpty(requestDetails.Body))
                {
                    request.AddStringBody(requestDetails.Body, ContentType.Json);
                }

                var response = await client.ExecuteAsync(request);

                Console.WriteLine($"[Consumer] Response: {response.StatusCode} - {response.Content}");

                if (response.IsSuccessful && !string.IsNullOrEmpty(requestDetails.ReplyToQueue))
                {
                    var responseBody = response.Content ?? string.Empty;
                    await PublishAsync(responseBody, requestDetails.ReplyToQueue);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Error processing request: {ex.Message}");
            }
        }

        public async Task<string> WaitForResponseAsync(string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            var tcs = new TaskCompletionSource<string>();

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                tcs.SetResult(message);
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));
            if (completedTask == tcs.Task)
            {
                return tcs.Task.Result;
            }
            else
            {
                throw new TimeoutException("No response received within the timeout period.");
            }
        }

        private async Task PublishAsync(string message, string queueName)
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

            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _settings.QueueName,
                mandatory: false,
                body: body);

            Console.WriteLine($"[Producer] Sent: {message}");
        }
    }

    public class RequestDetails
    {
        public Method Method { get; set; }
        public string Url { get; set; }
        public string? Body { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? ReplyToQueue { get; set; }
    }
}
