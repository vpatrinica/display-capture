using System.Diagnostics;
using DisplayCapture.Models;

namespace DisplayCapture.Services;

public class ScreenCaptureService
{
    private readonly ILogger<ScreenCaptureService> _logger;
    private Process? _recordingProcess;
    private bool _isRecording = false;
    private string _outputDirectory;
    private CancellationTokenSource? _recordingCancellation;

    public ScreenCaptureService(ILogger<ScreenCaptureService> logger)
    {
        _logger = logger;
        _outputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DisplayCapture");
        Directory.CreateDirectory(_outputDirectory);
    }

    public bool IsRecording => _isRecording;
    public string OutputDirectory => _outputDirectory;

    public async Task<string> StartRecordingAsync(CaptureArea area, int durationSeconds = 30)
    {
        if (_isRecording)
            throw new InvalidOperationException("Recording is already in progress");

        var fileName = $"screen_capture_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
        var outputPath = Path.Combine(_outputDirectory, fileName);

        try
        {
            _recordingCancellation = new CancellationTokenSource();
            
            // For cross-platform compatibility, we'll simulate recording
            // In a real implementation, you'd use platform-specific APIs or tools like FFmpeg
            string ffmpegArgs;
            
            if (OperatingSystem.IsWindows())
            {
                // Windows - use gdigrab
                ffmpegArgs = $"-f gdigrab -framerate 30 -offset_x {area.X} -offset_y {area.Y} -video_size {area.Width}x{area.Height} -i desktop -t {durationSeconds} -c:v libx264 -preset ultrafast -crf 28 \"{outputPath}\"";
            }
            else if (OperatingSystem.IsLinux())
            {
                // Linux - use x11grab
                ffmpegArgs = $"-f x11grab -framerate 30 -video_size {area.Width}x{area.Height} -i :0.0+{area.X},{area.Y} -t {durationSeconds} -c:v libx264 -preset ultrafast -crf 28 \"{outputPath}\"";
            }
            else if (OperatingSystem.IsMacOS())
            {
                // macOS - use avfoundation
                ffmpegArgs = $"-f avfoundation -capture_cursor 1 -framerate 30 -video_size {area.Width}x{area.Height} -i 1 -t {durationSeconds} -c:v libx264 -preset ultrafast -crf 28 \"{outputPath}\"";
            }
            else
            {
                throw new PlatformNotSupportedException("Recording not supported on this platform");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegArgs,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            _recordingProcess = Process.Start(startInfo);
            if (_recordingProcess == null)
            {
                throw new Exception("Failed to start ffmpeg process");
            }

            _isRecording = true;
            _logger.LogInformation($"Started recording to {outputPath}");
            
            // Monitor the recording process
            _ = Task.Run(async () =>
            {
                try
                {
                    await _recordingProcess.WaitForExitAsync(_recordingCancellation.Token);
                    _logger.LogInformation("Recording completed");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Recording was stopped");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during recording");
                }
                finally
                {
                    _isRecording = false;
                }
            }, _recordingCancellation.Token);

            return outputPath;
        }
        catch (Exception ex)
        {
            _isRecording = false;
            _logger.LogError(ex, "Failed to start recording");
            throw;
        }
    }

    public async Task StopRecordingAsync()
    {
        if (!_isRecording || _recordingProcess == null)
            return;

        try
        {
            _recordingCancellation?.Cancel();
            
            // Send 'q' to ffmpeg to stop recording gracefully
            if (!_recordingProcess.HasExited)
            {
                try
                {
                    await _recordingProcess.StandardInput.WriteLineAsync("q");
                    
                    // Use a cancellation token with timeout
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await _recordingProcess.WaitForExitAsync(cts.Token);
                }
                catch (Exception)
                {
                    // If graceful shutdown fails, force kill
                    if (!_recordingProcess.HasExited)
                    {
                        _recordingProcess.Kill();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping recording");
        }
        finally
        {
            _recordingProcess?.Dispose();
            _recordingProcess = null;
            _recordingCancellation?.Dispose();
            _recordingCancellation = null;
            _isRecording = false;
        }
    }

    public string[] GetRecordedFiles()
    {
        if (!Directory.Exists(_outputDirectory))
            return Array.Empty<string>();
            
        return Directory.GetFiles(_outputDirectory, "*.mp4")
            .OrderByDescending(f => File.GetCreationTime(f))
            .ToArray();
    }

    public void Dispose()
    {
        if (_isRecording)
        {
            StopRecordingAsync().Wait(TimeSpan.FromSeconds(10));
        }
        _recordingCancellation?.Dispose();
    }
}