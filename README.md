# GUPPI

<img align="right" width="200" height="200" src="img/ackbar.png">

GUPPI (or General Unit Primary Peripheral Interface) Is a semi-sentient software
being that helps Replicants interact with the many systems they have at their
disposal. This is an early implementation of the interface and as this is not
the year 2133 in the fictional [Bobverse](https://bobiverse.fandom.com/wiki/We_Are_Legion_(We_Are_Bob)_Wiki),
GUPPI is not actually semi-sentient and is only a command line utility to provide
me with the information I need.

![Continuous Integration](https://github.com/rprouse/guppi/workflows/Continuous%20Integration/badge.svg)

## Installation

This program is a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) and requires the latest version of
the [.NET SDK](https://dotnet.microsoft.com/download) to be installed. .NET 5.0 or newer is recommended.

### Testing Locally

Build it, then package it using the **Pack** command in Visual Studio or `dotnet pack`
on the command line. Until this package is published, install it using the following
command line from the solution root;

```sh
dotnet tool install -g --add-source ./Guppi.Console/nupkg dotnet-guppi
```

To update from a previous version,

```sh
dotnet tool update -g --add-source ./Guppi.Console/nupkg dotnet-guppi
```

### Installing from GitHub Packages

Whenever the version is updated in `Guppi/Guppi.csproj`, a merge to master will publish the NuGet package
to [GitHub Packages](https://github.com/rprouse?tab=packages). You can install or update from there.

First you must update your global NuGet configuration to add the package registry and include the GitHub Personal
Access Token (PAT). This file is in `%appdata%\NuGet\NuGet.Config` on Windows and in `~/.config/NuGet/NuGet.Config`
or `~/.nuget/NuGet/NuGet.Config` on Linux/Mac.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="Local" value="C:\temp" />
    <add key="Microsoft Visual Studio Offline Packages" value="C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\" />
    <add key="github" value="https://nuget.pkg.github.com/rprouse/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="rprouse" />
      <add key="ClearTextPassword" value="GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

Once that is done, to install,

```sh
dotnet tool install -g dotnet-guppi
```

And to update from a previous version,

```sh
dotnet tool update -g dotnet-guppi
```

## Enabling Tab Completion

This program supports tab completion using `dotnet-suggest`. To enable, for each shell
you must install the `dotnet-suggest` global tool and adding a shim to your profile. This
only needs to be done once and work for all applications built using `System.CommandLine`.

Follow the [setup instructions](https://github.com/dotnet/command-line-api/blob/main/docs/dotnet-suggest.md)
for your shell.

## Configuring Actions

### Actions.Calendar

Displays your next calendar event or today's agenda.

To get the information to configure:

1. [Enable the Google Calendar API](https://developers.google.com/calendar/quickstart/dotnet)
2. Configure the API as a Desktop App
3. Download client configuration and save to `C:\Users\rob\AppData\Local\Guppi\calendar_credentials.json`
4. Run and log in using OAuth2.

To check your API information, see the [API Console](https://console.developers.google.com/).

### Actions.Weather

Displays today's weather information.

To get the information to configure;

1. Sign up to [OpenWeatherMap](https://openweathermap.org/) and get an API Key.
2. Use [Google Maps](https://google.ca/maps) to get your Latitude and Longitude.

## ToDo

Ideas of information that I would like to see every morning.

- [x] [Weather](ActionProvider.Weather/Readme.md)
- [x] [Today's calendar events](ActionProvider.Calendar/Readme.md)
- [x] Turn on office lights (Hue)
- [x] Open PRs that I'm assigned to (Use the [GitHub CLI](https://github.com/cli/cli) `gh pr list`)
- [ ] CPU temperature, load and other system statistics
- [ ] Upcoming birthdays
- [ ] Monday.com inbox
- [ ] Jira issues/epics
- [x] GitHub issues (Use the [GitHub CLI](https://github.com/cli/cli) `gh issue list -a rprouse`)
- [ ] Car maintenance reminders?
- [ ] Top news headlines
- [ ] Open my browser to specific locations?
- [x] Todos (Use my [.NET Implementation](https://github.com/rprouse/dotnet-todo) of [todo.txt CLI](http://todotxt.org/)?)
- [x] AoC - Advent of Code leaderboards
- [x] AoC - Add a new day to my Advent of Code solution
- [x] AoC - Run tests for a year/day
- [x] Git - Switch to master/main, fetch and pull
- [x] Git - Undo last commit
- [x] Git - Ammend last commit
- [ ] Windows - Lock computer with `rundll32.exe user32.dll,LockWorkStation`
- [ ] Windows - Reboot computer with `shutdown.exe /r /t 5`
- [ ] Windows - Shutdown/halt with `shutdown.exe /s /t 5`
- [ ] Windows - Update with `start ms-settings:windowsupdate-action`
- [ ] Utils - New Guid (with format?)
- [ ] Docker?
- [ ] 7-ZIP - Zip current directory
- [ ] Run a Jupyter notebook server from a given directory
