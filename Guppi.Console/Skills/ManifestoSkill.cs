using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class ManifestoSkill : ISkill
{
    public IEnumerable<Command> GetCommands() =>
        new[]
        {
            new Command("agile", "Displays the Agile Manifesto.")
            {
                Handler = CommandHandler.Create(() => ViewManifesto(AgileManifesto))
            },
            new Command("hacker", "Displays the Hacker Manifesto.")
            {
                Handler = CommandHandler.Create(() => ViewManifesto(HackerManifesto))
            }
        };

    private void ViewManifesto(string manifesto)
    {
        var rows = System.Console.WindowHeight - 5;
        var lines = manifesto.Split(Environment.NewLine);
        for (int i = 0; i < lines.Length; i++)
        {
            AnsiConsole.MarkupLine(lines[i]);
            if (i % rows == 0 && i != 0)
            {
                AnsiConsole.Write(":");
                System.Console.ReadKey();
                AnsiConsole.Write("\b");
            }
        }
    }

    const string AgileManifesto = @"
[green]Manifesto for Agile Software Development[/]

[silver]We are uncovering better ways of developing[/]
[silver]software by doing it and helping others do it.[/]
[silver]Through this work we have come to value:[/]

[green]Individuals and interactions[/][silver] over processes and tools[/]
[green]Working software[/][silver] over comprehensive documentation[/]
[green]Customer collaboration[/][silver] over contract negotiation[/]
[green]Responding to change[/][silver] over following a plan[/]

[silver]That is, while there is value in the items on[/]
[silver]the right, we value the items on the left more.[/]
";


    const string HackerManifesto = @"
[green]               \/\The Conscience of a Hacker/\/[/]

[green]                              by[/]

[green]                       +++The Mentor+++[/]

[green]                  Written on January 8, 1986[/]
[silver]=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=[/]

[silver]Another one got caught today, it's all over the papers.  ""Teenager[/]
[silver]Arrested in Computer Crime Scandal"", ""Hacker Arrested after Bank Tampering""...[/]
[green]Damn kids.  They're all alike.[/]

[silver]But did you, in your three-piece psychology and 1950's technobrain,[/]
[silver]ever take a look behind the eyes of the hacker?  Did you ever wonder what[/]
[silver]made him tick, what forces shaped him, what may have molded him?[/]

[silver]I am a hacker, enter my world...[/]

[silver]Mine is a world that begins with school... I'm smarter than most of[/]
[silver]the other kids, this crap they teach us bores me...[/]
[green]Damn underachiever.  They're all alike.[/]

[silver]I'm in junior high or high school.  I've listened to teachers explain[/]
[silver]for the fifteenth time how to reduce a fraction.  I understand it.  ""No, Ms.[/]
[silver]Smith, I didn't show my work.  I did it in my head...""[/]
[green]Damn kid.  Probably copied it.  They're all alike.[/]

[silver]I made a discovery today.  I found a computer.  Wait a second, this is[/]
[silver]cool.  It does what I want it to.  If it makes a mistake, it's because I[/]
[silver]screwed it up.  Not because it doesn't like me...[/]
[silver]        Or feels threatened by me...[/]
[silver]        Or thinks I'm a smart ass...[/]
[silver]        Or doesn't like teaching and shouldn't be here...[/]
[green]Damn kid.  All he does is play games.  They're all alike.[/]

[silver]And then it happened... a door opened to a world... rushing through[/]
[silver]the phone line like heroin through an addict's veins, an electronic pulse is[/]
[silver]sent out, a refuge from the day-to-day incompetencies is sought... a board is[/]
[silver]found.[/]

[silver]""This is it... this is where I belong...""[/]
[silver]I know everyone here... even if I've never met them, never talked to[/]
[silver]them, may never hear from them again... I know you all...[/]

[green]Damn kid.  Tying up the phone line again.  They're all alike...[/]

[silver]You bet your ass we're all alike... we've been spoon-fed baby food at[/]
[silver]school when we hungered for steak... the bits of meat that you did let slip[/]
[silver]through were pre-chewed and tasteless.  We've been dominated by sadists, or[/]
[silver]ignored by the apathetic.  The few that had something to teach found us will-[/]
[silver]ing pupils, but those few are like drops of water in the desert.[/]

[silver]This is our world now... the world of the electron and the switch, the[/]
[silver]beauty of the baud.  We make use of a service already existing without paying[/]
[silver]for what could be dirt-cheap if it wasn't run by profiteering gluttons, and[/]
[silver]you call us criminals.  We explore... and you call us criminals.  We seek[/]
[silver]after knowledge... and you call us criminals.  We exist without skin color,[/]
[silver]without nationality, without religious bias... and you call us criminals.[/]
[silver]You build atomic bombs, you wage wars, you murder, cheat, and lie to us[/]
[silver]and try to make us believe it's for our own good, yet we're the criminals.[/]

[silver]Yes, I am a criminal.  My crime is that of curiosity.  My crime is[/]
[silver]that of judging people by what they say and think, not what they look like.[/]
[silver]My crime is that of outsmarting you, something that you will never forgive me[/]
[silver]for.[/]

[green]I am a hacker, and this is my manifesto.  You may stop this individual,[/]
[green]but you can't stop us all... after all, we're all alike.[/]

[silver]                       +++The Mentor+++[/]";
}
