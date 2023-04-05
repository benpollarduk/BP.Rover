# Introduction 
.NET 4.6.1 implementation of map exploration algorithms using a simulated rover. A UI is provided, written in WPF.

![image](https://user-images.githubusercontent.com/129943363/230073640-0cfef9ff-f140-4bbf-baa5-02077c3b486e.png)

The rules for exploration are:
 * The rover can only move in the following directions:
   * North
   * East
   * South
   * West
 * The rover cannot move over sea.
  
Map files (.map) can be imported that provide the following tiles:
 * . (0x2e)  sea location
 * : (0x3a)  unexplored land location
 * @ (0x40)  explored land location
 
An example map looks like:

```
...................................................................
...................................................................
...................................................................
...................................................................
................::::::::::::::::::::::.............................
...............::::::::::::::::::::::::............................
..............:::::::::::::@::::::::::::...........................
................::::::::::::::::::::::.............................
.....................:::::::::::::.................................
...................................................................
...................................................................
...................................................................
...................................................................
...................................................................
...................................................................
...................................................................
```

Example map files are provided in the **/ExampleMaps** directory, alternatively maps can be generated from within the UI.

A rendering option can be selected for viewing the number of times a square is passed through.

![image](https://user-images.githubusercontent.com/129943363/230103111-c780974a-4486-49e6-b28d-08b5e8788993.png)

# Prerequisites
 * Windows
   * Download free IDE Visual Studio 2022 Community ( >> https://visualstudio.microsoft.com/de/vs/community/ ), or use commercial Visual Studio 2022 Version.

# Getting Started
 * Clone the repo
 * Build all projects
 * Run the BP.Rover.UI project

# Contribute
It´s Open Source (License >> MIT), please feel free to use or contribute. To raise a pull request visit https://github.com/ben-pollard-uk/rover/pulls.

# For Open Questions
Visit https://github.com/ben-pollard-uk/rover/issues
