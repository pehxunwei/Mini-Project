using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using Mini_Project.Models;

namespace Mini_Project.Controllers
{
    public class SmsController : TwilioController
    {
        public ActionResult SendSms(string msg, string phoneNumber)
        {
            var accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
            TwilioClient.Init(accountSid, authToken);

            //var to = new PhoneNumber(ConfigurationManager.AppSettings["MyPhoneNumber"]);

            var to = new PhoneNumber(phoneNumber);
            var from = new PhoneNumber(ConfigurationManager.AppSettings["TwilioPhoneNumber"]);

            var message = MessageResource.Create(
                to: to,
                from: from,
                body: msg
                );

            return View();
        }

        //public ActionResult ReceiveSms()
        //{
        //    var response = new MessagingResponse();
        //    response.Message("The Robots are coming! Head for the hills!");

        //    return TwiML(response);
        //}
    }
}