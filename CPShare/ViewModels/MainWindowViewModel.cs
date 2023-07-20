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
            var image = @"<Window xmlns=""https://github.com/avaloniaui""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        xmlns:vm=""using:CPShare.ViewModels""
        xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
		xmlns:cc=""using:CPShare.Controls""
        xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
        mc:Ignorable=""d"" d:DesignWidth=""800"" d:DesignHeight=""450""
        x:Class=""CPShare.Views.MainWindow""
        x:DataType=""vm:MainWindowViewModel""
        Icon=""/Assets/avalonia-logo.ico""
        Title=""CPShare"">

    <Design.DataContext>
        <!-- This only sets the DataContext for the previewer in an IDE,
             to set the actual DataContext for runtime, set the DataContext property in code (look at App.axaml.cs) -->
        <vm:MainWindowViewModel/>
    </Design.DataContext>

	<DockPanel>
		<cc:CodeTextBlock></cc:CodeTextBlock>
		<Button Command=""{Binding Test}"">Test</Button>
	</DockPanel>

</Window>
".ToCodeTokens(".axaml").ToImage();
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