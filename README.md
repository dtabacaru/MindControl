# ClassicWowNeuralParasite
An open-source C# API and automater for World of Warcraft Classic

## Installation notes
The solution is developed in Visual Studio 2019.

All libraries are provided and this solution should compile natively in .NET Framework 4.7.2.

Since the WowAutomaterUi manifest file requires uiAccess="true", you will have to manually set the WowAutomaterUi
debug executable to have administrator privelages if you'd like to attach the Visual Studio debugger. Simply right 
click the executable and go to Properties, Compatability, and check the "Run this program as an administrator" checkbox.

## Structure
The solution contains 4 separate projects:

### WindowsInput
This project simulates the keyboard and mouse. Original code (no license): https://archive.codeplex.com/?p=inputsimulator

It is only used by the autoamter.

### WowApi
This project provides access to a static API object that relays in game information available to the World of Warcraft addon
system.

You can subscribe to the ClassicWowNeuralParasite.WowApi.UpdateEvent event to get real-time information from a running
instance of World of Warcraft Classic, provided you have installed the 'NeuralParasiteSensor' addon.

The addon itself is found within this project in the NeuralParasiteSensor folder.

Copy-paste this folder into the "Interface" folder of your World of Warcraft Classic installation forlder.

### WowAutomater
This project provides automated control of a World of Warcraft classic character via a WowAutomater object.

As of the current commit, Rogue automation is mostly supported. All other classes will need some development.

### WowNeuralParasiteUi
This project provides a graphical user interface for a user to run the WowAutomater.