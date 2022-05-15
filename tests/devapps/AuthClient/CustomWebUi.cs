// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Identity.Client.Extensibility;

namespace AuthClient
{
    public class CustomWebUi : ICustomWebUi
    {
        public const int DefaultWindowWidth = 600;
        public const int DefaultWindowHeight = 800;

        private readonly Form _owner;
        private readonly string _title;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly FormStartPosition _windowStartupLocation;

        public CustomWebUi(Form owner,
            string title = "Sign in",
            int windowWidth = DefaultWindowWidth,
            int windowHeight = DefaultWindowHeight,
            FormStartPosition windowStartupLocation = FormStartPosition.CenterScreen)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _title = title;
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
            _windowStartupLocation = windowStartupLocation;
        }

        public Task<Uri> AcquireAuthorizationCodeAsync(Uri authorizationUri, Uri redirectUri, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Uri>();
            _owner.Invoke(new Action(() =>
            {
                new AuthForm(authorizationUri,
                    redirectUri,
                    tcs,
                    cancellationToken)
                {
                    Owner = _owner,
                    Text = _title,
                    Width = _windowWidth,
                    Height = _windowHeight,
                    StartPosition = _windowStartupLocation,
                }.ShowDialog();
            }));

            return tcs.Task;
        }
    }
}
