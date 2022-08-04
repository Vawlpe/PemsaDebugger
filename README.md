# PemsaDebugger
Pico-8 / Pemsa Debugger and dev env
___
## Installation:
As this is highly ***experimental*** there is no pre-built packages for the debugger, however you can still try it out:

### CI Artifacts
Every time a code change is made, github builds the debugger for Windows, Linux, and MacOS, then uploads the builds as "artifacts" you can easily download and run by following these steps:
- Install the [.NET 6 ***Runtime***](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 
- Go to the [Actions tab](https://github.com/Vawlpe/PemsaDebugger/actions)
- Click on the first workflow run with a green checkmark (✅)
- Scroll to the "Artifacts" section at the bottom
- (Log in to github if you haven't already)
- Download the artifact for your system
___
### Build from source
Alternatively, you can build the debugger from source as follows:
  #### Pre-requisites:
  - [Install CMake](https://cmake.org/install/)
  - [Install .NET 6 ***SDK***](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  - [Install .NET Core 3.1 ***SDK***](https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
  - Clone this repo
  - Go to the directory you cloned the repo to (make sure it contains a file called `build.sh` or `build.ps1` (or both))

  #### Linux/MacOS
  - Open a terminal and run `sudo chmod +x build.sh` to make the build script runnable
  - Then run `build.sh` and wait

  #### Windows
  - Open a powershell instance ***as an admin***
  - Run `Get-ExecutionPolicy` in said instance
    - If this prints out anything other then `Bypass`, write it down, then run `Set-ExecutionPolicy Bypass` and confirm all prompts
  - Run `build.ps1` and wait

The finished build will be found under `src/PemsaDebugger/bin/Debug/net6.0/`