﻿using System;
using Discord.Webhook;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Discord
{
    public partial class DiscordSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly UInt64 _webhookId;
        private readonly string _webhookToken;

        public DiscordSink(IFormatProvider formatProvider,
            UInt64 webhookId,
            string webhookToken)
        {
            _formatProvider = formatProvider;
            _webhookId = webhookId;
            _webhookToken = webhookToken;
        }

        public void Emit(LogEvent logEvent)
        {
            SendMessage(logEvent);
        }

        private void SendMessage(LogEvent logEvent)
        {
            var webhookUrl = $"https://discord.com/api/webhooks/{_webhookId}/{_webhookToken}";
            var webHook = new Webhook(webhookUrl);

            try
            {
                var obj = SerilogWebhookObjectFactory.Create(logEvent, _formatProvider);
                webHook.Send(obj);
            }
            catch (Exception ex)
            {
                webHook.Send(new WebhookObject() {content = $"ooo snap, {ex.Message}"});
            }
        }
    }
}