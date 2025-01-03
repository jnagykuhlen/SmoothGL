﻿namespace SmoothGL.Content;

/// <summary>
/// Represents a content provider which can be used to load content from files or streams.
/// </summary>
public interface IContentProvider
{
    /// <summary>
    /// Loads content from a file.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="relativeFilePath">Relative path to the file storing content data.</param>
    /// <returns>Content object.</returns>
    T Load<T>(string relativeFilePath) where T : class;

    /// <summary>
    /// Loads content from a stream.
    /// </summary>
    /// <typeparam name="T">The requested content type to load.</typeparam>
    /// <param name="stream">Stream from which content data is read.</param>
    /// <returns>Content object.</returns>
    T Load<T>(Stream stream) where T : class;

    /// <summary>
    /// Adds a disposable content object so that its lifetime is managed by this content provider.
    /// </summary>
    /// <param name="disposable"></param>
    T Add<T>(T disposable) where T : IDisposable;
}