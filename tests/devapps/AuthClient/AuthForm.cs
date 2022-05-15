// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Microsoft.Identity.Client;
using Microsoft.Web.WebView2.WinForms;

namespace AuthClient
{
    /// <summary>
    /// Customform for use webview2.
    /// See: https://techcommunity.microsoft.com/t5/windows-dev-appconsult/how-to-use-embedded-web-ui-of-msal-net-on-wpf-on-net-core/ba-p/1315024
    /// </summary>
    public partial class AuthForm : Form
    {
        private readonly Uri _authorizationUri;
        private readonly Uri _redirectUri;
        private readonly TaskCompletionSource<Uri> _taskCompletionSource;
        private readonly CancellationToken _cancellationToken;
        private CancellationTokenRegistration _token;

        public AuthForm()
        {
            InitializeComponent();
        }
        public AuthForm(
            Uri authorizationUri,
            Uri redirectUri,
            TaskCompletionSource<Uri> taskCompletionSource,
            CancellationToken cancellationToken)
        {
            InitializeComponent();
            _authorizationUri = authorizationUri;
            _redirectUri = redirectUri;
            _taskCompletionSource = taskCompletionSource;
            _cancellationToken = cancellationToken;
        }

        private void webView2_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            Uri uri = new Uri(e.Uri);
            if (!e.Uri.ToString().StartsWith(_redirectUri.ToString()))
            {
                // not redirect uri case
                return;
            }

            // parse query string
            var query = HttpUtility.ParseQueryString(uri.Query);
            if (query.AllKeys.Any(x => x == "code"))
            {
                // It has a code parameter.
                _taskCompletionSource.SetResult(uri);
            }
            else
            {
                // error.
                _taskCompletionSource.SetException(
                    new MsalException($"An error occurred, error: {query.Get("error")}, error_description: {query.Get("error_description")}"));
            }

            Close();
        }

        private void Navigate(string url)
        {
            webView2.CoreWebView2.Navigate(url);
            Uri uri = _authorizationUri;

            if (!uri.ToString().StartsWith(_redirectUri.ToString()))
            {
                // not redirect uri case
                return;
            }

            // parse query string
            var query = HttpUtility.ParseQueryString(uri.Query);
            if (query.AllKeys.Any(x => x == "code"))
            {
                // It has a code parameter.
                _taskCompletionSource.SetResult(uri);
            }
            else
            {
                // error.
                _taskCompletionSource.SetException(
                    new MsalException($"An error occurred, error: {query.Get("error")}, error_description: {query.Get("error_description")}"));
            }

            Close();

        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            webView2.EnsureCoreWebView2Async();
            _token = _cancellationToken.Register(() => _taskCompletionSource.SetCanceled());
            // navigating to an uri that is entry point to authorization flow.
            Navigate(_authorizationUri.AbsoluteUri);
        }

        private void AuthForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _taskCompletionSource.TrySetCanceled();
            _token.Dispose();
        }

        private void webView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Uri uri = ((WebView2)sender).Source;

            if (!uri.ToString().StartsWith(_redirectUri.ToString()))
            {
                // not redirect uri case
                return;
            }

            // parse query string
            var query = HttpUtility.ParseQueryString(uri.Query);
            if (query.AllKeys.Any(x => x == "code"))
            {
                // It has a code parameter.
                _taskCompletionSource.SetResult(uri);
            }
            else
            {
                // error.
                _taskCompletionSource.SetException(
                    new MsalException($"An error occurred, error: {query.Get("error")}, error_description: {query.Get("error_description")}"));
            }

            Close();
        }
    }
}
