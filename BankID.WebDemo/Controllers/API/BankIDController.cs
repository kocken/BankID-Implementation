﻿using BankID.Client;
using BankID.Client.Models;
using BankID.WebDemo.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BankID.WebDemo.Controllers.API
{
    [RoutePrefix("api/bankid")]
    public class BankIDController : ApiController
    {
        private readonly IBankIdClient _bankIdClient;

        public BankIDController(IBankIdClient bankIdClient)
        {
            _bankIdClient = bankIdClient;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<AuthorizeResponseDTO> AuthenticateAsync(AuthenticateModel contract)
        {
            try
            {
                if (contract.EndUserIp == null)
                {
                    contract.EndUserIp = await Request.GetClientIpStringAsync();
                }

                return await _bankIdClient.AuthenticateAsync(contract.EndUserIp, contract.PersonalNumber, contract.Requirement);
            }
            catch (Exception e)
            {
                var errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(_bankIdClient.GetUserMessage(e))
                };

                throw new HttpResponseException(errorMessage);
            }
        }

        [HttpPost]
        [Route("sign")]
        public async Task<AuthorizeResponseDTO> SignAsync(SignModel contract)
        {
            try
            {
                if (contract.EndUserIp == null)
                {
                    contract.EndUserIp = await Request.GetClientIpStringAsync();
                }

                return await _bankIdClient.SignAsync(contract.EndUserIp, Client.Types.EncodeType.Undecoded,
                    contract.UserVisibleData, contract.UserNonVisibleData, contract.PersonalNumber, contract.Requirement);
            }
            catch (Exception e)
            {
                var errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(_bankIdClient.GetUserMessage(e))
                };

                throw new HttpResponseException(errorMessage);
            }
        }

        [HttpPost]
        [Route("cancel")]
        public async Task<bool> CancelAsync(string orderRef)
        {
            try
            {
                return await _bankIdClient.CancelAsync(orderRef);
            }
            catch (Exception e)
            {
                var errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(_bankIdClient.GetUserMessage(e))
                };

                throw new HttpResponseException(errorMessage);
            }
        }

        [HttpGet]
        [Route("collect")]
        public async Task<StatusModel> CollectAsync(string orderRef, bool isAutomaticStart, bool isQrCodeUsed)
        {
            try
            {
                var collectResponse = await _bankIdClient.CollectAsync(orderRef);

                var status = _bankIdClient.GetStatus(collectResponse);
                var userMessage = _bankIdClient.GetUserMessage(collectResponse, isAutomaticStart, isQrCodeUsed);

                return new StatusModel(collectResponse, status.ToString(), userMessage);
            }
            catch (Exception e)
            {
                var errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(_bankIdClient.GetUserMessage(e))
                };

                throw new HttpResponseException(errorMessage);
            }
        }
    }

    // https://stackoverflow.com/a/38096572
    static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        private static string LocalIp { get; set; }

        public static async Task<string> GetClientIpStringAsync(this HttpRequestMessage request)
        {
            //Local
            if (request.IsLocal())
            {
                if (LocalIp == null)
                {
                    var httpClient = new HttpClient();
                    LocalIp = await httpClient.GetStringAsync("https://api.ipify.org");
                }
                return LocalIp;
            }

            //Web-hosting
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            //Self-hosting
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            //Owin-hosting
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic ctx = request.Properties[OwinContext];
                if (ctx != null)
                {
                    return ctx.Request.RemoteIpAddress;
                }
            }

            if (System.Web.HttpContext.Current != null)
            {
                return System.Web.HttpContext.Current.Request.UserHostAddress;
            }

            // Always return all zeroes for any failure
            return "0.0.0.0";
        }
    }
}
