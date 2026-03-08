using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Controls;
using FluentAvalonia.UI.Controls;
using System;
using System.Threading.Tasks;
using SystemTools.Triggers;

namespace SystemTools.Settings;

public class FloatingWindowTriggerSettings : TriggerSettingsControlBase<FloatingWindowTriggerConfig>
{
    private const int IconCodeStart = 0xE000;
    private const int IconCodeEnd = 0xF4D3;

    private readonly TextBox _iconTextBox;
    private readonly TextBox _nameTextBox;

    private readonly WrapPanel _iconWrapPanel = new()
    {
        Orientation = Orientation.Horizontal,
        ItemWidth = 36,
        ItemHeight = 36,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Top
    };

    private bool _iconsLoaded;
    private ContentDialog? _iconPickerDialog;

    public FloatingWindowTriggerSettings()
    {
        var panel = new StackPanel { Spacing = 10, Margin = new Thickness(10) };

        panel.Children.Add(new TextBlock
        {
            Text = "悬浮窗按钮图标（示例：/uE7C3）",
            TextWrapping = TextWrapping.Wrap
        });

        _iconTextBox = new TextBox { Watermark = "/uE7C3", HorizontalAlignment = HorizontalAlignment.Stretch };
        _iconTextBox.TextChanged += (_, _) => { Settings.Icon = _iconTextBox.Text ?? string.Empty; };

        var iconRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8
        };
        iconRow.Children.Add(_iconTextBox);

        var pickIconButton = new Button
        {
            Content = "选择图标",
            VerticalAlignment = VerticalAlignment.Center
        };
        pickIconButton.Click += async (_, _) => await OpenIconPickerDialogAsync();
        Grid.SetColumn(pickIconButton, 1);
        iconRow.Children.Add(pickIconButton);
        panel.Children.Add(iconRow);

        panel.Children.Add(new TextBlock
        {
            Text = "悬浮窗按钮名称（显示在图标下方）",
            TextWrapping = TextWrapping.Wrap
        });

        _nameTextBox = new TextBox { Watermark = "例如：快捷抽取" };
        _nameTextBox.TextChanged += (_, _) => { Settings.ButtonName = _nameTextBox.Text ?? string.Empty; };
        panel.Children.Add(_nameTextBox);

        panel.Children.Add(new TextBlock
        {
            Text = "每个“从悬浮窗触发”触发器会在浮窗里生成一个按钮。",
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.Gray
        });

        Content = panel;
    }

    private async Task OpenIconPickerDialogAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return;
        }

        if (!_iconsLoaded)
        {
            LoadIcons();
            _iconsLoaded = true;
        }

        _iconPickerDialog = new ContentDialog
        {
            Title = "选择悬浮窗图标",
            PrimaryButtonText = "关闭",
            DefaultButton = ContentDialogButton.Primary,
            Content = new Border
            {
                Padding = new Thickness(8),
                Child = new ScrollViewer
                {
                    Height = 520,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Content = _iconWrapPanel
                }
            }
        };

        await _iconPickerDialog.ShowAsync(topLevel);
        _iconPickerDialog = null;
    }

    private void LoadIcons()
    {
        for (var code = IconCodeStart; code <= IconCodeEnd; code++)
        {
            var token = $"/u{code:X4}";
            var glyph = char.ConvertFromUtf32(code);

            var iconButton = new Button
            {
                Width = 34,
                Height = 34,
                Margin = new Thickness(1),
                ToolTip = token,
                Padding = new Thickness(0),
                Content = new FluentIcon
                {
                    Glyph = glyph,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            iconButton.Click += (_, _) => SelectIcon(token);
            _iconWrapPanel.Children.Add(iconButton);
        }
    }

    private void SelectIcon(string token)
    {
        Settings.Icon = token;
        _iconTextBox.Text = token;
        _iconPickerDialog?.Hide();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _iconTextBox.Text = Settings.Icon;
        _nameTextBox.Text = Settings.ButtonName;
    }
}
