# IcomMediaDisplay

## Description
The IcomMediaDisplay (IMD) plugin for SCP: Secret Laboratory enhances the Intercom system by allowing the playback of exported frames (images) in a specified folder or container. This feature enables server administrators to create immersive and dynamic communication experiences using visual media.

## Features
- Automatically downscale large images if they are too large.
- Bitmap quantizer, to compress code size by not a lot.
- Keeping track of previous color to not repeat same color tag.
- Media containers, as folders.
- Highly optimized, uses Async, conversion does not run in main thread.

## Commands
- `imd play <folderID>`: Initiates playback of frames from the specified directory or container.
- `imd pause`: Pauses the current playback.
- `imd stop`: Aborts the ongoing playback.
- `imd help`: Displays information about the available commands and their usage.
- `imd mod <value>`: Modifies the values of certain configuration fields.

## Permissions
- `imd.command`: Grants permission to use all IMD commands.

## Usage
- Open a terminal or command prompt on your computer.
- Use the cd command to navigate to the directory where your video file is located.
- Use the following FFmpeg command to export frames from the video, scaling them down by a factor of 10 or your desired one (please do this, do not use absolute size, do not torture your server), and using the %d notation for the output filename `ffmpeg -i input.mp4 -vf "scale=iw/10:ih/10" output/%d.png` (create output folder before executing the command!)
- Change the output folder to desired one, for example `Billy_Herrington`
- Place it in `.config/EXILED/Plugins/IcomMediaDisplay/`
- Use command play, in this example `imd play Billy_Herrington`

## Credits
- `ChatGPT` for the README, because I am lazy to write my own one.
- `kyutkoharu` for keeping me awake while coding this at night, wakey wakey.
- `tsukisashamiko` moving to async.

