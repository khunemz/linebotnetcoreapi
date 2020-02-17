using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using LineMessageApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace LineMessageApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LineBotController : Controller
    {
        private Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        AppSettings _appsettings;


        public LineBotController(AppSettings appConfig)
        {
            _appsettings = appConfig;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.Info("In get: api/linebot");
            return new string[] {"value1", "value2"};
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        public void Post([FromBody] dynamic req)
        {
            try
            {
                string responseTextJson = "";
                string lineAccessToken = _appsettings.AppConfig["LineChannelAccessToken"];
                var input = JsonConvert.SerializeObject(req);
                _logger.Info(input.ToString());
                Events events = JsonConvert.DeserializeObject<Events>(input) as Events;
                string userId = events.EventList[0].Source.UserId;
                string roomId = events.EventList[0].Source.RoomId;
                string groupId = events.EventList[0].Source.GroupId;
                string replyToken = events.EventList[0].ReplyToken;
                string message = events.EventList[0].Message.Text;


               

                var chatId = string.Empty;
                if (groupId != null)
                {
                    chatId = groupId;
                }
                else if (roomId != null)
                {
                    chatId = roomId;
                }
                else if (userId != null)
                {
                    chatId = userId;
                }

                _logger.Info(chatId.ToString());

                var replyMessage = string.Empty;
                
                _logger.Info(string.Format("UserId: {0}", chatId.ToString()));
                _logger.Info(string.Format("ReplyToken: {0}", replyToken.ToString()));
                _logger.Info(string.Format("Message: {0}", message.ToString()));

                List<string> res = _messageHandler(message);
                _logger.Info(string.Format("replyMessage {0}", replyMessage));

                var messagesJson = string.Empty;

                var jsonresult = string.Empty;

                for (int i = 0; i < res.Count; i++)
                {
                    var msg = res[i];
                    messagesJson += msg;
                    jsonresult += "{\"type\":\"text\",\"text\":\"" + msg + "\"}";
                    var str = i == res.Count - 1 ? "" : ",";
                    jsonresult += str;
                }

                jsonresult = string.Empty;
                jsonresult += "{\"type\":\"text\",\"text\":\"" + messagesJson + "\"}";


           
                responseTextJson = "{\"replyToken\":\"" + replyToken +
                                   "\",\"messages\":[" + jsonresult +"]}";


                var httpWebRequest = (HttpWebRequest) WebRequest.Create(_appsettings.AppConfig["LineApiEndPoint"]);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + lineAccessToken);
                _logger.Info(string.Format("{0}:{1}", replyToken, responseTextJson));
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(responseTextJson);
                }

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
            }
        }

     
        private List<string> _messageHandler(string message)
        {
            string returnMessage;
            var listMessage = new List<string>();
            message = message.ToLower();
            
            if (message.Contains("Hello"))
            {

                returnMessage = "Hi boss.";
                listMessage.Add(returnMessage);
            }

            return listMessage;
        }
    }
}