using System;
using System.Collections.Generic;
using Discord.Webhook;
using Serilog.Events;
using Serilog.Sinks.Discord.Helpers;

namespace Serilog.Sinks.Discord
{
    public static class SerilogWebhookObjectFactory
    {
        public static WebhookObject Create(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var obj = logEvent.Exception != null
                ? BuildExceptionObject(logEvent, formatProvider)
                : BuildBasicObject(logEvent, formatProvider);

            return obj;
        }

        private static WebhookObject BuildBasicObject(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var message = logEvent.RenderMessage(formatProvider);
            message = message?.Length > 256 ? message.Substring(0, 256) : message;

            var embed = new Embed()
            {
                title = GetLevelTitle(logEvent.Level),
                description = message,
                Color = GetDColor(logEvent.Level),
            };

            var obj = new WebhookObject {embeds = new List<Embed> {embed}};
            return obj;
        }

        private static WebhookObject BuildExceptionObject(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var stackTrace = logEvent.Exception.StackTrace;
            stackTrace = FormatStackTrace(stackTrace);

            var embed = new Embed()
            {
                title = "Error",
                description = logEvent.Exception.Message,
                Color = GetDColor(logEvent.Level),
                thumbnail = new Thumbnail() {url = "https://raw.githubusercontent.com/javis86/Serilog.Sinks.Discord/master/Resources/error.png"},
            };

            embed.fields = new List<Field>()
            {
                new Field() {name = "Type", value = logEvent.Exception.GetType().Name, inline = true},
                new Field() {name = "TimeStamp", value = logEvent.Timestamp.ToString(), inline = true},
                new Field() {name = "Message", value = logEvent.Exception.Message, inline = false},
                new Field() {name = "StackTrace", value = stackTrace, inline = false}
            };
            embed.fields.AddRange(GetFieldsFromEventLogProperties(logEvent.Properties));

            var obj = new WebhookObject {embeds = new List<Embed> {embed}};
            return obj;
        }

        private static string FormatStackTrace(string stackTrace)
        {
            if (!string.IsNullOrEmpty(stackTrace)
                && stackTrace.Length >= 1024)
                stackTrace = stackTrace.Substring(0, 1015) + "...";

            stackTrace = $"```{stackTrace ?? "NA"}```";
            return stackTrace;
        }

        private static DColor GetDColor(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                    return ColorsTemplate.Purple;
                case LogEventLevel.Error:
                    return ColorsTemplate.Red;
                case LogEventLevel.Fatal:
                    return ColorsTemplate.DarkRed;
                case LogEventLevel.Information:
                    return ColorsTemplate.Green;
                case LogEventLevel.Verbose:
                    return ColorsTemplate.Grey;
                case LogEventLevel.Warning:
                    return ColorsTemplate.Orange;
                default:
                    return ColorsTemplate.Black;
            }
        }
        
        private static string GetLevelTitle(LogEventLevel level)
        {
            return level.ToString();
        }

        public static IEnumerable<Field> GetFieldsFromEventLogProperties(IReadOnlyDictionary<string, LogEventPropertyValue> logEventProperties)
        {
            var properties = new List<Field>();

            foreach (var property in logEventProperties)
            {
                var value = property.Value.ToString().Length < 1024 
                    ? property.Value.ToString() 
                    : property.Value.ToString().Substring(0, 1020) + "...";

                properties.Add(new Field()
                {
                    name = property.Key,
                    value = value,
                    inline = true
                });
            }
            
            return properties.ToArray();
        }
    }
}