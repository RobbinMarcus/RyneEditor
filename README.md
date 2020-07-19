# Ryne Editor
Ryne Engine is a voxel based renderer. This is the open source Ryne Editor project that uses the Ryne Engine rendering backend. It contains code to run all of the Editor capabilities in addition to a simple ECS setup to create a framework for games.   

**Note:** Ryne is still in Alpha. At this stage there is a good possibility there are bugs, if you encounter any you can open an issue here.

Currently RyneEngine has a reference pathtracer that can render images like these:

![Pathtraced image](http://ryneengine.com/wp-content/uploads/2019/07/Render7.jpg)

You can find more renders and information on [the website](http://ryneengine.com/)

## Installation
Ryne should compile out-of-the-box on windows using Visual Studio 2017/2019. 

The necessary RyneEngine DLLs are redistributed through NuGet in a package called RyneEngine. This project will always be updated to the latest version of the package.

To load in the provided default cube model, follow the steps in **Voxelizing models** below.

## Voxelizing models
Since the latest release of RyneEngine, voxelization has been separated from the core engine. To voxelize triangle models in order to load them in the editor, there are two provided scripts in the root folder. 

Before running them make sure to **first compile the project** and then run the respective script for the configuration.
If you add new models you want to voxelize, you will have to manually add them to the voxelization script for now in the following format:

```
CALL :Voxelize cube\cube.obj cube.bsvdag [normals, smoothnormals] 
```

This will voxelize a model called cube.obj to cube.bsvdag. The optional parameter normals or smoothnormals allow for calling the Assimp functionality for generating normals or smoothmodels for the object. Please refer to the Assimp project for more information.

Once the voxelization script is done, you can now load in the models in the editor.

## Documentation
Until I can finish a more coherent documentation you can find a high level overview on [this page](http://ryneengine.com/2019/06/18/ryne/).

## Credits
RyneEngine would not be possible without the following projects:

* [ImGui](https://github.com/ocornut/imgui)
* [Assimp](https://github.com/assimp/assimp)
* [GLFW](https://github.com/glfw/glfw)
* [MessagePack](https://github.com/neuecc/MessagePack-CSharp)