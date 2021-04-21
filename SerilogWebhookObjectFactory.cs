using System;
using System.Collections.Generic;
using Discord.Webhook;
using Serilog.Events;

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
            message = message?.Length > 1024 ? message.Substring(0, 1024) : message;

            var obj = new WebhookObject();
            obj.AddEmbed(builder =>
                builder.WithTitle(GetLevelTitle(logEvent.Level))
                    .WithDescription(message)
                    .WithColor(GetColorFromLevel(logEvent.Level))
            );

            return obj;
        }

        private static WebhookObject BuildExceptionObject(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var stackTrace = logEvent.Exception.StackTrace;
            stackTrace = FormatStackTrace(stackTrace);

            var obj = new WebhookObject();

            obj.AddEmbed(builder =>
                builder.WithTitle("Error")
                    .WithDescription(logEvent.Exception.Message)
                    .WithColor(GetColorFromLevel(logEvent.Level))
                    .WithThumbnail("https://raw.githubusercontent.com/javis86/Serilog.Sinks.Discord/master/Resources/error.png")
                    .AddField("Type", logEvent.Exception.GetType().Name, true)
                    .AddField("TimeStamp", logEvent.Timestamp.ToString(), true)
                    .AddField("Message", logEvent.Exception.Message)
                    .AddField("StackTrace", stackTrace)
                    .AddFieldsRange(GetFieldsFromLogEventProperties(logEvent.Properties))
            );

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

        private static DColor GetColorFromLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                    return Colors.Purple;
                case LogEventLevel.Error:
                    return Colors.Red;
                case LogEventLevel.Fatal:
                    return Colors.DarkRed;
                case LogEventLevel.Information:
                    return Colors.Green;
                case LogEventLevel.Verbose:
                    return Colors.Grey;
                case LogEventLevel.Warning:
                    return Colors.Orange;
                default:
                    return Colors.Black;
            }
        }

        private static string GetLevelTitle(LogEventLevel level)
        {
            return level.ToString();
        }

        private static IEnumerable<Field> GetFieldsFromLogEventProperties(IReadOnlyDictionary<string, LogEventPropertyValue> logEventProperties)
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