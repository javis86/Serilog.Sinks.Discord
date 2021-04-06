using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Discord.Webhook;

namespace Serilog.Sinks.Discord.Helpers
{
    public class EmbedBuilder
    {
        private string _title;
        private string _description;
        private string _url;
        private Image _image;
        private Thumbnail _thumbnail;
        private List<Field> _fields;
        private Footer _footer;
        private DColor _color;


        public const int MaxFieldCount = 25;
        public const int MaxTitleLength = 256;
        public const int MaxDescriptionLength = 2048;
        public const int MaxEmbedLength = 6000;


        public EmbedBuilder()
        {
            _fields = new List<Field>();
            _color = Colors.Black;
        }

        public EmbedBuilder AddField(string name, object value, bool inline = false)
        {
            _fields.Add(new Field()
            {
                name = name,
                value = value,
                inline = inline
            });

            return this;
        }
        
        public EmbedBuilder WithColor(DColor color)
        {
            _color = color;
            return this;
        }
        
        public EmbedBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }
        
        public EmbedBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public EmbedBuilder WithImage(string urlImagen)
        {
            _image = new Image()
            {
                url = urlImagen
            };
            return this;
        }
        
        public EmbedBuilder WithThumbnail(string urlThumbnail)
        {
            _thumbnail = new Thumbnail()
            {
                url = urlThumbnail
            };
            return this;
        }
        
        public EmbedBuilder WithFooter(string text, string iconUrl)
        {
            _footer = new Footer()
            {
                text = text,
                icon_url = iconUrl
            };
            return this;
        }
        
        public EmbedBuilder WithUrl(string url)
        {
            _url = url;
            return this;
        }
        
        public EmbedBuilder AddFieldsRange(IEnumerable<Field> fields)
        {
            _fields.AddRange(fields);
            return this;
        }

        public Embed Build()
        {
            return new Embed()
            {
                title = _title,
                description = _description,
                Color = _color,
                fields = _fields,
                image = _image,
                thumbnail = _thumbnail,
                footer = _footer
            };
        }

    }

}