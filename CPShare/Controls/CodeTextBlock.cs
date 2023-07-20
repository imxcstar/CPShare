using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CPShare.Models;
using CPShare.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace CPShare.Controls
{
    public class CodeTextBlock : Control
    {
        public ObservableCollection<CodeToken> Tokens { get; set; }

        public CodeTextBlock()
        {
            Tokens = new ObservableCollection<CodeToken>(@"    using System;
    using System.IO;

    namespace CPShare
    {
        public class Test
        {
            public static void Test(string value)
            {
                // xxx
                /* yyyyy
                   aaaaa */
   
                Console.WriteLine(""Hello CPShare"");
                Console.WriteLine($""Hello {value}"");
            }
        }
    }
".ToCodeTokens(".cs"));
        }

        public sealed override void Render(DrawingContext context)
        {
            if (Tokens != null)
            {
                var i = 25.0;
                var j = 0;
                foreach (var token in Tokens)
                {
                    foreach (var tokenChar in token.Text)
                    {
                        if (tokenChar == '\r')
                            continue;
                        if (tokenChar == '\n')
                        {
                            i = 25.0;
                            j++;
                            var lineNumberStr = j.ToString();
                            var lineNumber = new FormattedText(lineNumberStr, CultureInfo.CurrentUICulture, FlowDirection.RightToLeft, Typeface.Default, 20, Brush.Parse(Color.FromRgb(70,78,91).ToString()));
                            context.DrawText(lineNumber, new Point(20-Math.Max(0,20-20/ lineNumberStr.Length), (j-1) * lineNumber.Height));
                            continue;
                        }
                        if (tokenChar == ' ')
                        {
                            i += 5;
                            continue;
                        }
                        var tokenStr = tokenChar.ToString();
                        var ft = new FormattedText(tokenStr, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Typeface.Default, 20, Brush.Parse(token.ForegroundColor.ToString()));
                        context.DrawText(ft, new Point(i, j * ft.Height));
                        i += ft.Width;
                    }
                }
            }

            base.Render(context);
        }
    }
}
