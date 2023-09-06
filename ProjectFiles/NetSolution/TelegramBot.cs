#region Using directives
using System;
using FTOptix.Core;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.OPCUAServer;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using System.Net;
using System.IO;
using System.Text;
using FTOptix.Recipe;
#endregion

public class TelegramBot : BaseNetLogic
{
    public override void Start()
    {
        resultMessage = LogicObject.GetVariable("ResultMessage");
        if (resultMessage == null)
            throw new CoreConfigurationException("ResultMessage variable not found");

        botTokenVariable = LogicObject.GetVariable("BotToken");
        if (botTokenVariable == null)
            throw new CoreConfigurationException("BotToken variable not found");

        channelIdVariable = LogicObject.GetVariable("ChannelId");
        if (channelIdVariable == null)
            throw new CoreConfigurationException("ChannelId variable not found");

        botChatIdVariable = LogicObject.GetVariable("BotChatId");
        if (botChatIdVariable == null)
            throw new CoreConfigurationException("BotChatId variable not found");
    }

    [ExportMethod]
    public void SendChannelMessage(string message)
    {
        botToken = botTokenVariable.Value;
        channelId = channelIdVariable.Value;
        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var urlString = GetUrlString(botToken, message, channelId);

            var web = new System.Net.WebClient();
            var response = web.DownloadString(urlString);

            Log.Info("TelegramBot", response);
            resultMessage.Value = "Message sent successfully";
        }
        catch (Exception e)
        {
            Log.Error("TelegramBot", $"An exception occurred while sending message to the channel '{channelId}': {e.Message}");
            resultMessage.Value = "Error Sending Message";
        }
    }

    [ExportMethod]
    public void SendBotMessage(string message)
    {
        botToken = botTokenVariable.Value;
        botChatId = botChatIdVariable.Value;
        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var urlString = GetUrlString(botToken, message, botChatId);

            var web = new System.Net.WebClient();
            var response = web.DownloadString(urlString);

            Log.Info("TelegramBot", response);
            resultMessage.Value = "Message sent successfully";
        }
        catch (Exception e)
        {
            Log.Error("TelegramBot", $"An exception occurred while sending message to the bot '{botChatId}' : {e.Message}");
            resultMessage.Value = "Error Sending Message";
        }
    }

    private string GetUrlString(string botToken, string message, string chatId)
    {
        return $"https://api.telegram.org/bot{botToken}/sendMessage?text=\"{message}\"&chat_id={chatId}";
    }

    private IUAVariable botTokenVariable;
    private IUAVariable channelIdVariable;
    private IUAVariable botChatIdVariable;
    private IUAVariable resultMessage;
    private string botToken;
    private string channelId;
    private string botChatId;
}
