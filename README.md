# Introduction 
.NET 4.6.1 implementation of basic map exploration algorithms using a simulated rover. UI is written in WPF.

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
  
Alternatively, maps can be generated from within the UI.

# Prerequisites
 * Windows
   * Download free IDE Visual Studio 2022 Community ( >> https://visualstudio.microsoft.com/de/vs/community/ ), or use commercial Visual Studio 2022 Version.

# Getting Started
 * Clone Repo
 * Run BP.Rover.UI

# Contribute
It´s Open Source (License >> MIT), please feel free to use or contribute. To raise a pull request visit https://github.com/ben-pollard-uk/rover/pulls.

# For Open Questions
Visit https://github.com/ben-pollard-uk/rover/issues
