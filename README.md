# Display Capture Tool

A Blazor Server application for capturing portions of the screen with mouse interactions, designed for easy sharing via Teams.

## Features

- **Screen Area Selection**: Define rectangular capture area with customizable position and size
- **5-Second Countdown**: Get ready before recording starts with visual countdown
- **Live Recording Status**: Clear indication when recording is active
- **Automatic Stop**: Recording stops automatically after specified duration
- **Manual Stop**: Option to stop recording early
- **File Management**: View recent recordings and access output folder
- **Cross-Platform**: Works on Windows, Linux, and macOS

## Quick Start

1. **Run the Application**:
   ```bash
   cd DisplayCapture
   dotnet run
   ```

2. **Access the Tool**: Open your browser to `https://localhost:5001` (or the URL shown in console)

3. **Configure Capture**:
   - Set X, Y position for the top-left corner of capture area
   - Set Width and Height for the capture dimensions  
   - Set Recording Duration (5-300 seconds)
   - Use quick presets for common sizes

4. **Start Recording**:
   - Click "Start Recording" 
   - 5-second countdown begins
   - Position your mouse and get ready
   - Recording starts automatically

5. **Stop Recording**:
   - Recording stops automatically after duration
   - Or click "Stop Recording" to end early
   - Files are saved to Documents/DisplayCapture

## Requirements

- **.NET 8.0** (ready for upgrade to .NET 9.0)
- **FFmpeg** installed and available in system PATH
- **Web Browser** (Chrome, Firefox, Edge, Safari)

## FFmpeg Installation

### Windows
- Download from https://ffmpeg.org/download.html
- Add to system PATH

### Linux (Ubuntu/Debian)
```bash
sudo apt update
sudo apt install ffmpeg
```

### macOS
```bash
brew install ffmpeg
```

## Output

- **Format**: MP4 video files
- **Location**: `~/Documents/DisplayCapture/`
- **Naming**: `screen_capture_YYYYMMDD_HHMMSS.mp4`
- **Quality**: Optimized for Teams sharing (H.264, 30fps)

## Usage Tips

- Test your capture area first with a short recording
- Ensure sufficient disk space for longer recordings  
- Close unnecessary applications for better performance
- Use preset sizes for consistent recordings
- The tool captures everything visible in the specified rectangle

## Troubleshooting

- **FFmpeg not found**: Install FFmpeg and ensure it's in your system PATH
- **Permission issues**: Run with appropriate permissions for screen capture
- **Performance issues**: Reduce capture area size or recording duration
- **File not found**: Check the output folder path in the application

## Teams Integration

The generated MP4 files are optimized for Teams sharing:
- Upload directly to Teams chat or channel
- Share via OneDrive link  
- Attach to email or meeting invite
- Compatible with Teams recording playback

## Development

Built with:
- **Blazor Server** for real-time UI updates
- **Bootstrap 5** for responsive design  
- **Font Awesome** for icons
- **FFmpeg** for cross-platform screen recording
