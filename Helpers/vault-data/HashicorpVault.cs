using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vault.Client;
using Vault.Model;
using Vault;

namespace Helpers.vault_data
{
    public class HashicorpVault
    {
        public static void Main()
        {
            string address = "http://127.0.0.1:8200";
            VaultConfiguration config = new VaultConfiguration(address);

            VaultClient vaultClient = new VaultClient(config);
            vaultClient.SetToken("my-token");

            try
            {
                var secretData = new Dictionary<string, string> { { "mypass", "pass" } };

                // Write a secret
                var kvRequestData = new KvV2WriteRequest(secretData);

                vaultClient.Secrets.KvV2Write("mypath", kvRequestData);

                // Read a secret
                VaultResponse<KvV2ReadResponse> resp = vaultClient.Secrets.KvV2Read("mypath");
                Console.WriteLine(resp.Data.Data);
            }
            catch (VaultApiException e)
            {
                Console.WriteLine("Failed to read secret with message {0}", e.Message);
            }
        }
    }
}
