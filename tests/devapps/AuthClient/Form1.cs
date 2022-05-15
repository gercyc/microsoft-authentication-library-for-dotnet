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
        private IAccount _account;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string clientId = "e3a243fd-ee77-496b-8647-78c45294fb24";
                string environmentId = "c5f9c259-c9e3-4b7e-be07-41536b1b843c";
                _client = PublicClientApplicationBuilder.Create(clientId)
                    .WithTenantId(environmentId)
                    .WithDesktopFeatures()
                    .WithPingIdAuthority(new Uri("https://auth.pingone.com/c5f9c259-c9e3-4b7e-be07-41536b1b843c/as/authorize"), false)
                    .WithRedirectUri("http://localhost:8059") //addin sonda teste
                    .Build();

                var auth = await _client.AcquireTokenInteractive(new string[] { "profile" })
                    .WithPrompt(Prompt.NoPrompt)
                    .WithUseEmbeddedWebView(false)
                    //.WithCustomWebUi(new CustomWebUi(this))
                    .WithParentActivityOrWindow(this)
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
