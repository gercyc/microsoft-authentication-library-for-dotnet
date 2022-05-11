﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Client;
using Microsoft.Identity.Test.Common;
using Microsoft.Identity.Test.Integration.Infrastructure;
using Microsoft.Identity.Test.Integration.net45.Infrastructure;
using Microsoft.Identity.Test.LabInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.UI;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Test.Common.Core.Mocks;
using Microsoft.Identity.Client.OAuth2;
using System.Runtime.InteropServices;
using System;
using NSubstitute;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Identity.Test.Integration.Broker
{
    
    [TestClass]
    public class RuntimeBrokerTests
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// Initialized by MSTest (do not make private or readonly)
        /// </summary>
        //public TestContext TestContext { get; set; }
        private CoreUIParent _coreUIParent;
        private ICoreLogger _logger;
        private RuntimeBroker _wamBroker;
        IntPtr _parentHandle = GetForegroundWindow();
        //private SynchronizationContext _synchronizationContext;

        [TestInitialize]
        public void Init()
        {
            //_synchronizationContext = new DedicatedThreadSynchronizationContext();
            _coreUIParent = new CoreUIParent() { OwnerWindow = _parentHandle };
            ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
            _logger = Substitute.For<ICoreLogger>();
            _wamBroker = new RuntimeBroker(_coreUIParent, applicationConfiguration, _logger);
        }

        [TestMethod]
        public async Task WamSilentAuthAsync()
        {
            string[] scopes = new[]
                {
                    "https://management.core.windows.net//.default"
                };

            PublicClientApplicationBuilder pcaBuilder = PublicClientApplicationBuilder
               .Create("04f0c124-f2bc-4f59-8241-bf6df9866bbd")
               .WithAuthority("https://login.microsoftonline.com/organizations");

            IPublicClientApplication pca = pcaBuilder.WithBroker2().Build();

            // Act
            try
            {
                var result = await pca.AcquireTokenSilent(scopes, PublicClientApplication.OperatingSystemAccount).ExecuteAsync().ConfigureAwait(false);

                Assert.IsNotNull(result.AccessToken);
            }
            catch (MsalUiRequiredException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Need user interaction to continue"));
            }

        }

        [TestMethod]
        public async Task WamInteractiveAuthNoWindowHandleAsync()
        {
            string[] scopes = new[]
                {
                    "https://management.core.windows.net//.default"
                };

            IAccount accountToLogin = PublicClientApplication.OperatingSystemAccount;

            PublicClientApplicationBuilder pcaBuilder = PublicClientApplicationBuilder
               .Create("04f0c124-f2bc-4f59-8241-bf6df9866bbd")
               .WithAuthority("https://login.microsoftonline.com/organizations");

            IPublicClientApplication pca = pcaBuilder.WithBroker2().Build();

            // Act
            try
            {
                var result = await pca.AcquireTokenInteractive(scopes)
                    .WithAccount(accountToLogin)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                Assert.IsNotNull(result.AccessToken);
            }
            catch (MsalClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Public Client applications wanting to use WAM need to provide their window handle. " +
                    "Console applications can use GetConsoleWindow Windows API for this"));
            }

        }

        [TestMethod]
        public async Task WamInteractiveAuthOperatingSystemAccountAsync()
        {
            string[] scopes = new[]
                {
                    "https://management.core.windows.net//.default"
                };

            IAccount accountToLogin = PublicClientApplication.OperatingSystemAccount;

            PublicClientApplicationBuilder pcaBuilder = PublicClientApplicationBuilder
               .Create("04f0c124-f2bc-4f59-8241-bf6df9866bbd")
               .WithAuthority("https://login.microsoftonline.com/organizations")
               .WithParentActivityOrWindow(() => _parentHandle);

            IPublicClientApplication pca = pcaBuilder.WithBroker2().Build();

            // Act
            try
            {
                var result = await pca.AcquireTokenInteractive(scopes)
                    .WithAccount(accountToLogin)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                Assert.IsNotNull(result.AccessToken);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }

        }
    }
}
