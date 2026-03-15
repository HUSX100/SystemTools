using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SystemTools.Actions;

[ActionInfo("SystemTools.SetVolumeAndHold", "调整系统音量并保持", "\uF013", false)]
public class SetVolumeAndHoldAction(ILogger<SetVolumeAndHoldAction> logger) : ActionBase<SetVolumeAndHoldSettings>
{
    private readonly ILogger<SetVolumeAndHoldAction> _logger = logger;

    protected override Task OnInvoke()
    {
        var targetVolume = Math.Clamp(Settings.VolumePercent, 0f, 100f) / 100f;
        var holdSeconds = Math.Max(0f, Settings.HoldSeconds);

        _ = Task.Run(() => HoldVolumeAsync(targetVolume, holdSeconds));

        _logger.LogInformation("已启动音量保持任务，目标音量 {VolumePercent}%，保持 {HoldSeconds} 秒", targetVolume * 100f,
            holdSeconds);

        return Task.CompletedTask;
    }

    private async Task HoldVolumeAsync(float targetVolume, float holdSeconds)
    {
        try
        {
            var deviceEnumerator = new MMDeviceEnumeratorWrapper();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            var endTime = DateTime.UtcNow.AddSeconds(holdSeconds);

            device.SetMasterVolumeLevelScalar(targetVolume, Guid.Empty);

            while (DateTime.UtcNow < endTime)
            {
                var currentVolume = device.GetMasterVolumeLevelScalar();
                if (Math.Abs(currentVolume - targetVolume) > 0.001f)
                {
                    device.SetMasterVolumeLevelScalar(targetVolume, Guid.Empty);
                    _logger.LogDebug("检测到音量变化，已恢复到目标音量 {VolumePercent}%", targetVolume * 100f);
                }

                await Task.Delay(100);
            }

            _logger.LogInformation("音量保持任务结束，目标音量 {VolumePercent}%", targetVolume * 100f);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "音量保持任务执行失败");
        }
    }
}
