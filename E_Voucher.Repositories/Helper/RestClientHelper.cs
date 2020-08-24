using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Voucher.Repositories.Helper
{
    public class RestAuthenticator<T> : IAuthenticator
    {
        private string AccessToken;
        private T BodyObject;
        public RestAuthenticator(string token,T bodyObject )
        {
            AccessToken = "Bearer "+token;
            BodyObject = bodyObject;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            client.AddDefaultHeader("Authorization", AccessToken);
            request.Method = Method.POST;
            
            request.AddJsonBody(BodyObject);
        }
    }
}
