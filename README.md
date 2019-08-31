# Ryne Editor
Ryne Engine is a voxel based renderer. This is the open source Ryne Editor project that uses the Ryne Engine rendering backend. It contains code to run all of the Editor capabilities in addition to a simple ECS setup to create a framework for games.   

**Note:** Ryne is still in Alpha. At this stage there is a good possibility there are bugs, if you encounter any you can open an issue here.

Currently RyneEngine has a reference pathtracer that can render images like these:

![Pathtraced image](http://ryneengine.com/wp-content/uploads/2019/07/Render7.jpg)

You can find more renders and information on [the website](http://ryneengine.com/)

## Installation
Ryne should compile out-of-the-box on windows using Visual Studio 2017/2019. 

The necessary RyneEngine DLLs are redistributed through NuGet in a package called RyneEngine. This project will always be updated to the latest version of the package.

## Documentation
Until I can finish a more coherent documentation you can find a high level overview on [this page](http://ryneengine.com/2019/06/18/ryne/).

## Credits
RyneEngine would not be possible without the following projects:

* [ImGui](https://github.com/ocornut/imgui)
* [Assimp](https://github.com/assimp/assimp)
* [GLFW](https://github.com/glfw/glfw)