using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageClient
{
    public static class MessageWaiter
    {
        public static async Task<T?>? WaitForMessage<T>(MessageClient<T> messageClient, string clientId, int timeout = 5000)
        {
            var tcs = new TaskCompletionSource<T?>();
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            cancellationTokenSource.Token.Register(() => tcs.TrySetResult(default));

            using (
                var connection = messageClient.ListenUsingTopic<T>(message =>
                {
                    tcs.TrySetResult(message);
                }, "User" + clientId, clientId)
            )
            {
            }

            return await tcs.Task;
        }
    }
}
