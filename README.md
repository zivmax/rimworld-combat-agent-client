# RimWorld Combat Agent Client

A RimWorld mod that enables AI-controlled combat training scenarios. This mod creates a client that connects to an external AI agent server for automated combat simulation and training.

## Features

- Creates controlled combat training environments
- Connects to external AI agent server via TCP/IP
- Supports configurable team sizes and map generation
- Provides automated reset and state management
- Includes headless mode for training

## Installation

1. Copy this mod to your RimWorld Mods folder
2. Enable the mod in RimWorld's mod menu
3. Start the AI agent server (sold separately)

## Configuration

Launch arguments for customization:

- `--agent-control`: Enable/disable agent control (default: true)
- `--team-size`: Number of pawns per team (default: 1)  
- `--map-size`: Size of training map (default: 15)
- `--gen-trees`: Generate trees on map (default: true)
- `--gen-ruins`: Generate ruins on map (default: true)
- `--seed`: Random seed for map generation
- `--server-addr`: AI agent server address (default: localhost)
- `--server-port`: AI agent server port (default: 10086)
- `--interval`: Time between agent actions in seconds (default: 1.0)
- `--speed`: Game speed (default: 1)
- `--headless`: Run in headless mode (default: false)

## Launch

After build, install and activate the mod in RimWorld, run a Devquick Test to use the mode is recommended.

To run a quick test every time when the game starts, launch RimWorld with the following command:

```sh
./RimWorldBinary -quicktest <other arguments>
```

## Development

The mod is written in C# targeting .NET Framework 4.8. Visual Studio Code with C# extension is recommended for development.

Build scripts are provided for both Windows and Linux:

Windows:
```sh
.vscode/win/build.bat
```

Linux:

## Requirements
- RimWorld 1.5
- .NET Framework 4.8 (.NET SDK 8.0 is recommended to install)
