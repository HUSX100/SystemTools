using Avalonia.Controls;
using Avalonia.Data;
using ClassIsland.Core.Abstractions.Controls;
using SystemTools.Actions;

namespace SystemTools.Controls;

public class SetVolumeAndHoldSettingsControl : ActionSettingsControlBase<SetVolumeAndHoldSettings>
{
    private NumericUpDown _volumeInput;
    private NumericUpDown _holdSecondsInput;

    public SetVolumeAndHoldSettingsControl()
    {
        var panel = new StackPanel { Spacing = 10, Margin = new(10) };

        panel.Children.Add(new TextBlock
        {
            Text = "目标音量百分比",
            Margin = new(0, 0, 0, 5)
        });

        _volumeInput = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 100,
            Increment = 1,
            FormatString = "0",
            Watermark = "输入 0-100 的整数"
        };
        panel.Children.Add(_volumeInput);

        panel.Children.Add(new TextBlock
        {
            Text = "保持时间（秒）",
            Margin = new(0, 5, 0, 5)
        });

        _holdSecondsInput = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 3600,
            Increment = 0.5,
            FormatString = "0.##",
            Watermark = "输入保持秒数"
        };
        panel.Children.Add(_holdSecondsInput);

        panel.Children.Add(new TextBlock
        {
            Text = "执行后将立即返回，并在后台每 100ms 检测音量变化后自动恢复",
            Foreground = Avalonia.Media.Brushes.Gray,
            FontSize = 12,
            Margin = new(0, 5, 0, 0)
        });

        Content = panel;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _volumeInput[!NumericUpDown.ValueProperty] = new Binding(nameof(Settings.VolumePercent))
        {
            Source = Settings,
            Mode = BindingMode.TwoWay
        };

        _holdSecondsInput[!NumericUpDown.ValueProperty] = new Binding(nameof(Settings.HoldSeconds))
        {
            Source = Settings,
            Mode = BindingMode.TwoWay
        };
    }
}
