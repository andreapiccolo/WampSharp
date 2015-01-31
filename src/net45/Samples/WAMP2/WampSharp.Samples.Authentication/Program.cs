﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WampSharp.V2;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;
using WampSharp.V2.Rpc;

namespace WampSharp.Samples.Authentication
{
    public interface ITimeService
    {
        [WampProcedure("com.timeservice.now")]
        Task<string> Now();
    }

    public class TicketAuthenticator : IWampClientAuthenticator
    {
        private static readonly string[] mAuthenticationMethods = { "ticket" };

        private readonly IDictionary<string, string> mTickets = new Dictionary<string, string>()
        {
            { "peter", "magic_secret_1" },
            { "joe", "magic_secret_2" }
        };

        private const string User = "peter";

        public AuthenticationResponse Authenticate(string authmethod, ChallengeDetails extra)
        {
            if (authmethod == "ticket")
            {
                Console.WriteLine("authenticating via '" + authmethod + "'");
                
                AuthenticationResponse result = 
                    new AuthenticationResponse {Signature = mTickets[User]};
                
                return result;
            }
            else
            {
                throw new WampAuthenticationException("don't know how to authenticate using '" + authmethod + "'");
            }
        }

        public string[] AuthenticationMethods
        {
            get
            {
                return mAuthenticationMethods;
            }
        }

        public string AuthenticationId
        {
            get
            {
                return User;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Run();
            Console.ReadLine();
        }

        private async static Task Run()
        {
            const string url = "ws://127.0.0.1:8080/ws";
            const string realm = "realm1";

            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            IWampClientAuthenticator authenticator = new TicketAuthenticator();
            IWampChannel channel = channelFactory.CreateJsonChannel(url, realm, authenticator);
            
            IWampRealmProxy realmProxy = channel.RealmProxy;

            IWampClientConnectionMonitor monitor = realmProxy.Monitor;
            monitor.ConnectionEstablished += ConnectionEstablished;
            monitor.ConnectionBroken += ConnectionBroken;

            await channel.Open();
            ITimeService proxy = realmProxy.Services.GetCalleeProxy<ITimeService>();
            
            try
            {
                string now = await proxy.Now();
                Console.WriteLine("call result {0}", now);
            }
            catch (Exception e)
            {
                Console.WriteLine("call error {0}", e);
            }

            Console.ReadLine();
        }

        private static void ConnectionEstablished(object sender, WampSessionEventArgs e)
        {
            IDictionary<string, object> details = 
                e.Details.Deserialize<IDictionary<string, object>>();
            
            Console.WriteLine("connected session with ID " + e.SessionId);
            Console.WriteLine("authenticated using method '" + details["authmethod"] + "' and provider '" + details["authprovider"] + "'");
            Console.WriteLine("authenticated with authid '" + details["authid"] + "' and authrole '" + details["authrole"] + "'");
        }

        private static void ConnectionBroken(object sender, WampSessionCloseEventArgs e)
        {
            Console.WriteLine("disconnected {0}", e.Reason);
        }
    }
}