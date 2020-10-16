# GUPPI

<img align="right" width="200" height="200" src="img/ackbar.png">

GUPPI (or General Unit Primary Peripheral Interface) Is a semi-sentient software
being that helps Replicants interact with the many systems they have at their
disposal. This is an early implementation of the interface and as this is not
the year 2133 in the fictional [Bobverse](https://bobiverse.fandom.com/wiki/We_Are_Legion_(We_Are_Bob)_Wiki),
GUPPI is not actually semi-sentient and is only a command line utility to provide
me with the information I need.

## Installation

This program is a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools). 
Build it, then package it using the **Pack** command in Visual Studio or `dotnet pack` 
on the command line. Until this package is published, install it using the following
command line from the solution root;

```sh
dotnet tool install -g --add-source .\nupkg\ alteridem.guppi
```

## Enabling Tab Completion

This program supports tab completion using `dotnet-suggest`. To enable, for each shell
you must install the `dotnet-suggest` global tool and adding a shim to your profile. This 
only needs to be done once and work for all applications built using `System.CommandLine`.

Follow the [setup instructions](https://github.com/dotnet/command-line-api/blob/main/docs/dotnet-suggest.md)
for your shell.

## ToDo

Ideas of information that I would like to see every morning.

- [x] [Weather](DataProvider.Weather/Readme.md)
- [x] [Today's calendar events](DataProvider.Calendar/Readme.md)
- [ ] Turn on office lights (Hue)
- [x] Open PRs that I'm assigned to (Use the [GitHub CLI](https://github.com/cli/cli) `gh pr list`)
- [ ] Upcoming birthdays
- [ ] Monday.com inbox
- [ ] Jira issues/epics
- [x] GitHub issues (Use the [GitHub CLI](https://github.com/cli/cli) `gh issue list -a rprouse`)
- [ ] Car maintenance reminders?
- [ ] Top news headlines
- [ ] Open my browser to specific locations?
- [ ] Todos (Use the [todo.txt CLI](http://todotxt.org/)?)
