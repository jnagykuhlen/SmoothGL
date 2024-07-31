﻿namespace SmoothGL.Graphics;

/// <summary>
/// Exception which is thrown when linking of compiled shaders fails.
/// </summary>
public class ShaderProgramLinkException : Exception
{
    /// <summary>
    /// Creates a new ShaderProgramLinkException.
    /// </summary>
    /// <param name="message">Message specifying why shader linking failed.</param>
    public ShaderProgramLinkException(string message)
        : base(message)
    {
    }
}