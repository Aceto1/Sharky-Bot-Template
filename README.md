# StarCraft 2 Sharky Bot Template

## Prerequisites

1. [Visual Studio 2022](https://visualstudio.microsoft.com/de/downloads/)
2. StarCraft 2 installed via Battle.net
3. [git](https://git-scm.com/downloads)
4. [NET SDK 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Setup

1. Fork this repository on GitHub
 
2. Clone your fork
	
	`git clone https://github.com/{your GitHub username}/Sharky-Bot-Template.git`

3. Install the Sharky framework

	3.1 Open the commandline in the folder of your repo

	3.2 Run `git submodule init`
	 
	3.3 Run `git submodule update`

4. Download and install the SC2AI Arena maps as described on their [site](https://sc2ai.net/wiki/maps/)
## Development

The example should be runnable by opening Visual Studio and hitting F5 (or the green 'play' symbol in the top menu bar).

Adding a build is as easy as creating a new class in the 'Builds' folder and inheriting the `Build` class. You should now be able to implement all the lifecycle methods. For reference on how this could look, see `Builds/RepaerCheese.cs` or the example in the official [Sharky repo](https://github.com/sharknice/Sharky).

After implementing the class just add it to the builds in the `BuildChoiceManager.cs` file.

## Data

Every 5 seconds ingame a log entry is written to a database located at `C:\Users\{your username}\AppData\Local\StarCraft 2 Bot\sc2bot.db`. This database also contains the results of every played game as well as other telemtry data.

To view this data you can use [DB Browser for SQlite](https://sqlitebrowser.org/).

## Known Issues

1. `System.AggregateException: 'One or more errors occurred. (An error occurred trying to start process '...' with working directory '...'. File not found.)'`

Some files needed for running the StartCraft 2 bot could not be found because Battle.net has not yet created them.

Solution: Start StarCraft 2 from Battle.net and close it again. The files should exist now.
