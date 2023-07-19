using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Skia;
using CPShare.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace CPShare.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
        }

        public void Test()
        {
            var image = @"namespace CPShare.Models
{
    public class CodeToken
    {
        public string Text { get; set; }
        public SKColor ForegroundColor { get; set; }
        public SKColor BackgroundColor { get; set; }
    }

    public static class CodeTokenExtend
    {
        public static SKImage ToImage(this IEnumerable<CodeToken> codeTokens)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Red,
                TextSize = 25,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName(Typeface.Default.FontFamily.Name)
            };

            var wt = 0.0f;
            var w = 0.0f;
            var h = 0.0f;
            var hn = 0;
            foreach (var token in codeTokens)
            {
                foreach (var tokenChar in token.Text)
                {
                    if (tokenChar == '\r')
                        continue;
                    if (tokenChar == '\n')
                    {
                        w = Math.Max(w, wt);
                        wt = 0;
                        hn++;
                        continue;
                    }
                    if (tokenChar == ' ')
                    {
                        wt += 10;
                        continue;
                    }
                    var tokenStr = tokenChar.ToString();

                    float textWidth = paint.MeasureText(tokenStr);
                    paint.GetFontMetrics(out var metrics);
                    float textHeight = metrics.Descent - metrics.Ascent;

                    wt += textWidth;
                    h = Convert.ToSingle(hn) * textHeight;
                }
            }

            w += 50;
            h += 50;
            var info = new SKImageInfo(Convert.ToInt32(Math.Round(w)), Convert.ToInt32(Math.Round(h)));
            var bitmap = new SKBitmap(info);
            var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Black);

            var i = 25.0f;
            var j = 1;
            foreach (var token in codeTokens)
            {
                foreach (var tokenChar in token.Text)
                {
                    if (tokenChar == '\r')
                        continue;
                    if (tokenChar == '\n')
                    {
                        i = 25.0f;
                        j++;
                        continue;
                    }
                    if (tokenChar == ' ')
                    {
                        i += 10;
                        continue;
                    }
                    var tokenStr = tokenChar.ToString();

                    float textWidth = paint.MeasureText(tokenStr);
                    paint.GetFontMetrics(out var metrics);
                    float textHeight = metrics.Descent - metrics.Ascent;

                    paint.Color = token.ForegroundColor;
                    var font = new SKFont(paint.Typeface, paint.TextSize);
                    canvas.DrawText(SKTextBlob.Create(tokenStr, font), i, j * textHeight, paint);
                    i += textWidth;
                }
            }
            return SKImage.FromBitmap(bitmap);
        }

        public static List<CodeToken> ToCodeTokens(this string code, string codeType)
        {
            var ret = new List<CodeToken>();
            var options = new RegistryOptions(ThemeName.DarkPlus);
            var registry = new Registry(options);
            var theme = registry.GetTheme();
            var grammar = registry.LoadGrammar(options.GetScopeByExtension(codeType));

            ITokenizeLineResult result = grammar.TokenizeLine(code);

            foreach (IToken token in result.Tokens)
            {
                var startIndex = (token.StartIndex > code.Length) ?
                    code.Length : token.StartIndex;
                var endIndex = (token.EndIndex > code.Length) ?
                    code.Length : token.EndIndex;

                var foreground = -1;
                var background = -1;
                var fontStyle = -1;

                foreach (var themeRule in theme.Match(token.Scopes))
                {
                    if (foreground == -1 && themeRule.foreground > 0)
                        foreground = themeRule.foreground;

                    if (background == -1 && themeRule.background > 0)
                        background = themeRule.background;

                    if (fontStyle == -1 && themeRule.fontStyle > 0)
                        fontStyle = themeRule.fontStyle;
                }

                ret.Add(new CodeToken
                {
                    Text = code[startIndex..endIndex],
                    ForegroundColor = GetColor(foreground, theme),
                    BackgroundColor = GetColor(background, theme),
                });
            }
            return ret;
        }

        private static SKColor GetColor(int colorId, Theme theme)
        {
            if (colorId == -1)
                return SKColors.White;

            return SKColor.Parse(theme.GetColor(colorId));
        }
    }
}".ToCodeTokens(".cs").ToImage();
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                using (var stream = File.OpenWrite("t.png"))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}