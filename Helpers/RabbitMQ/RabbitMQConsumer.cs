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

        public async Task StartListeningAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // Ensure the queue exists
            await channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[Consumer] Listening to queue: {_settings.QueueName}");

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
                queue: _settings.QueueName,
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

                var client = new RestClient(requestDetails.Url);

                var request = new RestRequest()
                {
                    Method = requestDetails.Method
                };

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Error processing request: {ex.Message}");
            }
        }

        public async Task ProcessRequestAsync(string requestJson)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            var requestDetails = JsonSerializer.Deserialize<RequestDetails>(requestJson);
            if (requestDetails == null || string.IsNullOrEmpty(requestDetails.ReplyToQueue)) return;

            //TODO Fetch the data (simulate REST API call here)
            var patientData = "[{\"id\":1,\"name\":\"John Doe\"}]"; // Example response

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(requestDetails.ReplyToQueue, durable: true, exclusive: false, autoDelete: false);
            await channel.BasicPublishAsync(exchange: "", routingKey: requestDetails.ReplyToQueue, mandatory: false, body: Encoding.UTF8.GetBytes(patientData));
        }

        public async Task<string> WaitForResponseAsync(string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
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
