//#region Copyright (C) 2014 Sascha Simon

////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 3 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see http://www.gnu.org/licenses/.
////
////  Visit the official homepage at http://www.sascha-simon.com

//#endregion

//using System;
//using System.Diagnostics;
//using System.Threading.Tasks;
//using Strava.Common;

//namespace Strava.Authentication
//{
//    /// <summary>
//    /// This class gives access to the user's access token through different methods.
//    /// </summary>
//    public class WebAuthentication : IAuthentication
//    {
//        /// <summary>
//        /// AccessTokenReceived is raised when an access token is received from the Strava server.
//        /// </summary>
//        public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;

//        /// <summary>
//        /// AuthCodeReceived is raised when an auth token is received from the Strava server.
//        /// </summary>
//        public event EventHandler<AuthCodeReceivedEventArgs> AuthCodeReceived;

//        /// <summary>
//        /// The access token that was received from the Strava server.
//        /// </summary>
//        public string AccessToken { get; set; }

//        /// <summary>
//        /// the auth token that was received from the Strava server.
//        /// </summary>
//        public String AuthCode { get; set; }

//        /// <summary>
//        /// Loads an access token asynchronously from the Strava servers. Invoking this method opens a web browser.
//        /// This method is used to start a local web server to receive a callback message from Strava.
//        /// Using this method requires admin privileges.
//        /// </summary>
//        /// <param name="clientId">The client id from your application (provided by Strava).</param>
//        /// <param name="clientSecret">The client secret (provided by Strava).</param>
//        /// <param name="scope">Define what your application is allowed to do.</param>
//        /// <param name="callbackPort">Define the callback port (optional, default value is 1895). Only change this, 
//        /// if the default port 1895 is already used on your machine.</param>
//        public void GetTokenAsync(String clientId, String clientSecret, Scope scope, int callbackPort = 1895)
//        {
//            LocalWebServer server = new LocalWebServer(String.Format("http://*:{0}/", callbackPort));
//            server.ClientId = clientId;
//            server.ClientSecret = clientSecret;

//            server.AccessTokenReceived += delegate (object sender, TokenReceivedEventArgs args)
//            {
//                if (AccessTokenReceived != null)
//                {
//                    AccessTokenReceived(this, args);
//                    AccessToken = args.Token;
//                }
//            };

//            server.AuthCodeReceived += delegate (object sender, AuthCodeReceivedEventArgs args)
//            {
//                if (AuthCodeReceived != null)
//                {
//                    AuthCodeReceived(this, args);
//                    AuthCode = args.AuthCode;
//                }
//            };

//            server.Start();

//            String url = "https://www.strava.com/oauth/authorize";
//            String scopeLevel = String.Empty;

//            switch (scope)
//            {
//                case Scope.Full:
//                    scopeLevel = "view_private,write";
//                    break;
//                case Scope.Public:
//                    scopeLevel = "public";
//                    break;
//                case Scope.ViewPrivate:
//                    scopeLevel = "view_private";
//                    break;
//                case Scope.Write:
//                    scopeLevel = "write";
//                    break;
//            }

//            Process process = new Process();
//            process.StartInfo = new ProcessStartInfo(String.Format("{0}?client_id={1}&response_type=code&redirect_uri=http://localhost:{2}&scope={3}&approval_prompt=auto", url, clientId, callbackPort, scopeLevel));
//            process.Start();
//        }

//        /// <summary>
//        /// Loads an access token asynchronously from the code returned by the Strava server.
//        /// </summary>
//        /// <param name="clientId">The client id from your application (provided by Strava).</param>
//        /// <param name="clientSecret">The client secret (provided by Strava).</param>
//        /// <param name="code">The code returned by the Strava server (after calling the https://www.strava.com/oauth/authorize url)</param>
//        public async Task GetTokenAsync(String clientId, String clientSecret, String code)
//        {
//            String url = String.Format("https://www.strava.com/oauth/token?client_id={0}&client_secret={1}&code={2}", clientId, clientSecret, code);
//            String json = await Http.WebRequest.SendPostAsync(new Uri(url));

//            AccessToken auth = Unmarshaller<AccessToken>.Unmarshal(json);

//            if (!String.IsNullOrEmpty(auth.Token))
//            {
//                AuthCode = code;
//                AccessToken = auth.Token;
//            }
//        }
//    }
//}