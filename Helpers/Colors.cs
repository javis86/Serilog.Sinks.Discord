using Discord.Webhook;

namespace Serilog.Sinks.Discord.Helpers
{
    public static class Colors
    {
        public static readonly DColor Red = new DColor(231, 76, 70); //https://www.spycolor.com/e74c3c#
        public static readonly DColor DarkRed = new DColor(153,45,34); //https://www.spycolor.com/992d22#
        public static readonly DColor Purple = new DColor(155,89,182); //https://www.spycolor.com/9b59b6#
        public static readonly DColor Green = new DColor(46,204,113); //https://www.spycolor.com/2ecc71#
        public static readonly DColor Orange = new DColor(230,126,34); //https://www.spycolor.com/e67e22#
        public static readonly DColor Grey = new DColor(179,177,175); //https://www.spycolor.com/b3b1af#
        public static readonly DColor Black = new DColor(255,255,255);
    }
}