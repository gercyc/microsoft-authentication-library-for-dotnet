using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using Microsoft.Identity.Client.Extensibility;

namespace AuthClient
{
    public partial class Form1 : Form
    {
        private IPublicClientApplication _client;
        private IConfidentialClientApplication _confidentialClient;
        private IAccount _account;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string clientId = "f498bff5-d99a-43e6-8c80-17804b326300";
                string environmentId = "c5f9c259-c9e3-4b7e-be07-41536b1b843c";

                //_confidentialClient = ConfidentialClientApplicationBuilder
                //    .Create(clientId)
                //    .WithClientAssertion(clientId)
                //    .WithTenantId(environmentId)
                //    .WithAuthority("https://auth.pingone.com/c5f9c259-c9e3-4b7e-be07-41536b1b843c/saml20/idp/sso", false)
                //    .WithRedirectUri("http://localhost:9502/Comply")
                //    .Build();

                //var auth = await _confidentialClient
                //    .AcquireTokenForClient(new string[] { "profile" })
                //    //.AcquireTokenByAuthorizationCode(new string[] { "profile" }, "ComplySaml")
                //    .ExecuteAsync()
                //    .ConfigureAwait(true);

                _client = PublicClientApplicationBuilder.Create(clientId)
                    .WithTenantId(environmentId)
                    .WithDesktopFeatures()
                    .WithPingIdAuthority(new Uri("https://auth.pingone.com/c5f9c259-c9e3-4b7e-be07-41536b1b843c/saml20/idp/startsso?spEntityId=ComplySaml"), false)
                    .WithRedirectUri("http://localhost:8059") //addin sonda teste
                    .Build();

                var auth = await _client
                    //.AcquireTokenByIntegratedWindowsAuth(new string[] { "profile" })
                    
                    .AcquireTokenInteractive(new string[] { "profile" })

                    //.WithPrompt(Prompt.NoPrompt)
                    //.WithParentActivityOrWindow(this)

                    .ExecuteAsync()
                    .ConfigureAwait(true);

                if (auth != null)
                {
                    lbLogin.Text = auth.Account.Username;
                    _account = auth.Account;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{exception.Message} \n {exception.StackTrace}");
                Console.WriteLine(exception);
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string clientId = "c34609d2-4e3f-478b-8b97-0fb3551be336";
                string environmentId = "7023fac1-375c-4554-a207-b5359b9209e5";
                _client = PublicClientApplicationBuilder.Create(clientId)
                    .WithRedirectUri("http://localhost:8059") //addin sonda teste
                            .WithTenantId(environmentId)
                            .Build();
                var auth = await _client.AcquireTokenInteractive(new string[] { "User.read" }).WithPrompt(Prompt.NoPrompt).WithUseEmbeddedWebView(false).ExecuteAsync().ConfigureAwait(true);
                if (auth != null)
                {
                    lbLogin.Text = auth.Account.Username;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($@"{exception.Message} \n {exception.StackTrace}");
                Console.WriteLine(exception);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var account = await _client.GetAccountAsync(lbLogin.Text).ConfigureAwait(true);
            await _client.RemoveAsync(_account).ConfigureAwait(true);
        }
    }
}
