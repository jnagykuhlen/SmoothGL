# Overview

SmoothGL is an easy-to-use framework for OpenGL graphics in C\#. It is intended to be a replacement for the great but not longer maintained [XNA framework by Microsoft](https://en.wikipedia.org/wiki/Microsoft_XNA). In contrast to projects like [MonoGame](http://www.monogame.net/) or [ANX](https://anxframework.codeplex.com/), SmoothGL does not provide the exact same interface as XNA but tries to transfer its simplicity and user-friendliness into a system that works well with OpenGL. Thus, existing code based on the XNA framework will not compile with SmoothGL, but programmers familiar with XNA will have no problems to quickly build graphics applications based on this new API. This freedom regarding framework design allowed to include useful, modern OpenGL features such as uniform buffers, geometry and tessellation shaders and depth-stencil textures.

This framework is based on [OpenTK](https://github.com/opentk/opentk), which is primarily a low-level binding for OpenGL functions as well as window management. The goal of SmoothGL is to allow programmers to work with strongly-typed, intuitively to use objects to avoid most of the common mistakes in low-level OpenGL. Furthermore, it provides a lightweight content system that makes loading of textures, models, shaders and other resources very easy. However, it does not fully replace OpenTK, which is still required to create a window and main loop.

### What SmoothGL is

SmoothGL wraps most OpenGL objects required for modern graphics applications into an easy-to-use API. When using this framework, no direct calls to OpenGL functions (which are those included in the *OpenTK.Graphics.OpenGL* namespace) are required anymore. The following table shows which features from XNA are implemented in SmoothGL and vice versa, where deviating names are added in brackets.

| Feature                           | XNA                     | SmoothGL                 |
| --------------------------------- | ----------------------- | ------------------------ |
| Vertex buffers                    | **Yes**                 | **Yes**                  |
| Indexed rendering                 | **Yes** (index buffer)  | **Yes** (element buffer) |
| Instanced rendering               | **Yes**                 | **Yes**                  |
| Composed rendering state          | **Yes** (mesh)          | **Yes** (vertex array)   |
| Vertex and fragment shaders       | **Yes** (effect)        | **Yes** (shader program) |
| Geometry and tessellation shaders | No                      | **Yes** (shader program) |
| Color textures                    | **Yes**                 | **Yes**                  |
| Cube textures                     | **Yes**                 | **Yes**                  |
| Depth-stencil textures            | No                      | **Yes**                  |
| Render to texture                 | **Yes** (render target) | **Yes** (frame buffer)   |
| Occlusion queries                 | **Yes**                 | **Yes**                  |
| Shader constant buffers           | No                      | **Yes** (uniform buffer) |
| Blend states                      | **Yes**                 | **Yes**                  |
| Depth-stencil states              | **Yes**                 | **Yes**                  |
| 2D rendering                      | **Yes** (sprite batch)  | No                       |
| Basic collision detection         | **Yes**                 | No                       |
| Content loading system            | **Yes**                 | **Yes**                  |
| Sound                             | **Yes**                 | No                       |

### What SmoothGL is not

This framework is designed primarily as graphics library and thus does not cover other game components such as sound, collision and physics, networking, input and window management. It is furthermore not meant to replace OpenTK entirely, but rather to extend and simplify the graphics-related part of it. This means that OpenTK is still required for creating a window with main loop and input handling, as well as providing the necessary math classes (which is the functionality included in the *OpenTK* and *OpenTK.Input* namespaces).

# How to build

SmoothGL was developed using Visual Studio 2015, but should be compatible with other versions of Visual Studio as well. It depends on the *OpenTK.dll*, which needs to be copied to the *SmoothGL/External/OpenTK/Debug/* and the *SmoothGL/External/OpenTK/Release/* directories with corresponding build configuration. The [source code of OpenTK](https://github.com/opentk/opentk) can be cloned from GitHub and manually compiled, or alternatively, binaries can be downloaded from the [official OpenTK website](http://www.opentk.com/). In order to use the features provided by SmoothGL, a graphics card that supports at least OpenGL version 3.3 is required (OpenGL version 4.0 for tessellation shaders).

# Code structure

The code is split into the two major parts *Graphics* and *Content*, each with corresponding namespaces and subdirectories for separate components. All contained classes and interfaces as well as their public members are annotated with XML documentation comments that provide further usage information directly in Visual Studio. Classes which are required exclusively for internal use were moved to *Internal* folders with corresponding namespaces (such as *SmoothGL.Graphics.Internal* for example), but nevertheless have the 'public' access modifier. This is because they might still be helpful for certain programmers extending the framework. However, they are mostly undocumented and using them requires a detailed understanding of the system. Similarly, instantiation of non-internal classes is considered safe, whereas creating subclasses and using protected methods is not. Subclassing is generally not necessary to use the features provided by SmoothGL, but can be done at own risk to include new OpenGL features (which of course presumes insights into the OpenGL low-level API).

# Getting started

Documented sample code can be found in the *Samples* project, showing which steps are required to present graphics on the screen using the SmoothGL framework.