# StarCraft 2 AI 1-1-1-TvT-Bread-And-Butter-Tactic

using the Sharky framework adaptation by Aceto

## What should we do?

+ Build an opener from the database
+ Refine the opener
+ Which buildings from the opener can we use?
+ And to which follow-up strategies can we transition with the preconditions of the opener

e.g. Reaper Opener → Mech

### What is difficult?
+ How can we evaluate, which tactics are good counters to which others?
+ How can we even detect a tactic?
+ How can we automatically detect a tactic and decide which counter to play?

e.g. https://lotv.spawningtool.com/build/166798/

## Tactic
This tactic is written for use by other AI builders. It should adapt to 
different enemy tactics well.

e.g. 
Against Battle Cruisers → build Vikings (have higher range and are faster) 

We can recognise, which tactics the enemy can choose by analyzing the possibilities of buildings:
- If he builds a Starport, then he will build air units.
- If the star port has a reactor: → vikings
- if the star port has a tech lab: → probably battleships
- If the enemy does not have factories by a certain point, he will probably focus on biological units (e.g. marines or reapers)
- You do not spend upgrades on biological units, if you won't focus on them. So, producing upgrades is a good indicator, that he will use them.
- Stimpacks are not visible, so they have to be observed in battle.
- ...

We can deduct necessary buildings from the properties that units have: e.g. 2-2, 2-0 means that the enemy has an armory 
and a factory.

If he only has reactors on barracks, then he lacks a response to armoured units.

*Therefore it is absolutely necessary to scan the enemy regularly.*

## Opener
Currently copied from: https://lotv.spawningtool.com/build/174631/

	Supply Depot	
	Barracks	
	Refinery
	Reaper, Orbital Command	
	Supply Depot	
	Factory	
	Reaper
	Command Center	
	Hellion

	Starport: Prioritize this over Reactor, Supply Depot and even SCVs if necessary.
	Barracks Reactor	
    Supply Depot
	Widow Mine	
	Marine x2	
	Factory Tech Lab	
	Starport Tech Lab	
	Refinery: To make sure the refinery finishes *with* the CC.
	Marine x2

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


## Known Issues

1. `System.AggregateException: 'One or more errors occurred. (An error occurred trying to start process '...' with working directory '...'. File not found.)'`

Some files needed for running the StartCraft 2 bot could not be found because Battle.net has not yet created them.

Solution: Start StarCraft 2 from Battle.net and close it again. The files should exist now.
