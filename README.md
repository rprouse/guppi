# GUPPI

<img align="right" width="200" height="200" src="img/ackbar.png">

GUPPI (or General Unit Primary Peripheral Interface) Is a semi-sentient software
being that helps Replicants interact with the many systems they have at their
disposal. This is an early implementation of the interface and as this is not
the year 2133 in the fictional [Bobverse](https://bobiverse.fandom.com/wiki/We_Are_Legion_(We_Are_Bob)_Wiki),
GUPPI is not actually semi-sentient and is only a command line utility to provide
me with the information I need.

![Continuous Integration](https://github.com/rprouse/guppi/workflows/Continuous%20Integration/badge.svg)

## Skills

### Advent of Code

Views the AoC leaderboard, adds a new day to my AoC Visual Studio solution and runs the tests.

To configure:

1. Log in to [Advent of Code](https://adventofcode.com/)
2. Go to the private leaderboard, click on API and view the JSON for the board
3. Take the leaderboard number from the JSON filename
4. In the browser dev tools, copy the session cookie minus the `session=`
5. Set the `src` directory for the Visual Studio AoC solution

### Calendar

Displays your next calendar event or today's agenda from Google Calendar and Office 365. Right
now it gets both and both must be configured.

#### Google Calendar

To get the information to configure:

1. Sign in to the [Azure portal](https://portal.azure.com/)
2. In the left-hand navigation pane, select the `Azure Active Directory` service, and then select `Add application registration`
3. Set a name and appropriate account types, then set the redirect URL to native with the url `http://localhost:39428`
3. Download client configuration and save to `C:\Users\rob\AppData\Local\Guppi\calendar_credentials.json`
4. Run and log in using OAuth2.

To check your API information, see the [API Console](https://console.developers.google.com/).

#### Office 365

To get the information to configure, follow the instructions at:

1. [Enable the Google Calendar API](https://developers.google.com/calendar/quickstart/dotnet)
2. Configure the API as a Desktop App
3. Download client configuration and save to `C:\Users\rob\AppData\Local\Guppi\calendar_credentials.json`
4. Run and log in using OAuth2.

To check your API information, see the [API Console](https://console.developers.google.com/).

### Dictionary and Thesaurus

Displays the definition of a word or the synonyms of a word. You must supply a
dictionary and thesaurus API key from [Merriam-Webster](https://dictionaryapi.com/).

### Git

Useful git aliases like `ammend`, `undo`, `unstage` and `update` which switches to the
master branch, does a fetch and a pull.

### Hue Lights

Control Philip's Hue lights. On first run, it will search for and connect to the first Hue Bridge
it finds. If there are more than one bridge, list the bridges and register using the IP.

You can have one default light which is set using the configure command.

### Notes

I keep all of my notes as Markdown files. I used to use VS Code, but recently switched to 
[Obsidian](https://obsidian.md/) for better linking and formatting. This opens my notes 
in Obsidian, adds new notes and optionally opens the notes directory in VS Code.

### Strava

Displays Strava fitness activities.

1. Get the [client id and secret](https://www.strava.com/settings/api) for the api.

### Utilities

Displays the date, the time or creates a new Guid.

### Weather

Displays today's weather information.

To get the information to configure;

1. Sign up to [OpenWeatherMap](https://openweathermap.org/) and get an API Key.
2. Use [Google Maps](https://google.ca/maps) to get your Latitude and Longitude.

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
