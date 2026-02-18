using System;
using Avalonia;
using Avalonia.Controls;

namespace UDTool_Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        
        // Get the current screen's DPI scaling
        var screen = Screens.ScreenFromWindow(this);
        if (screen != null)
        {
            var scaling = screen.Scaling;
            
            // Calculate the size in physical pixels (device independent pixels)
            var logicalWidth = 900 / scaling;
            var logicalHeight = 1000 / scaling;
            
            this.Width = logicalWidth;
            this.Height = logicalHeight;
            this.MinWidth = logicalWidth;
            this.MaxWidth = logicalWidth;
            this.MinHeight = logicalHeight;
            this.MaxHeight = logicalHeight;
        }
    }
}