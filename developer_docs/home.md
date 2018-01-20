Welcome to the CityBuilder wiki!

# Project Statement

This project is a long term project that bears the goal of simulating and creating real life scenes, traffic systems and economic systems. The current goal (first step) is to make it something like EA's Simcity 5. Every car can be tracked and displayed, and the car conforms to traffic laws. 

# Principles and Architecture

The game uses Unity Engine, 2017f2. Please use this game engine until we have upgrade plan. The game is broken into several systems. There are currently two systems in the game, the road system and traffic system. More systems will be built as the game gets more complex. You can view the guides of each systems here.
* [Road System](systems/road_system.md)
* [Traffic System](systems/traffic_system.md)

# Code Architecture

The most important principles are dependency injection. We take advantage of the Unity Editor to inject dependencies and linkage between scripts. The game components are broken into several great parts. Each group of the script is managed by a empty game object. Currently there are four broad categories of scripts. They are controllers, managers, builders and maths. There are also types of scripts that describe the underlying data structure. Those scripts are not visible in the Unity Editor, but nearly all scripts interact with abstract data model.
* [Controllers](game_objects/controllers.md) are responsible for user interaction with the System.
* [Builders](game_objects/builders.md) are responsible for visualizing and displaying abstract data structure.
* [Maths](game_objects/maths.md) are mainly mathematical calculations that are needed by other game components.
* [Managers](game_objects/managers.md) are managing the gameplay. Some of them act as a bridge between controllers and abstract data types.
* [Other Scripts](other_scripts/other_scripts.md) include abstract data structure. There will be more components added as game grows.

# Contributing

Please contact me if you want to contribute to this project. Bug reports, feature requests, and PR are all welcome. 