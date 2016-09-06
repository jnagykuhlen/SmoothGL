using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;

using SmoothGL.Graphics;
using SmoothGL.Content.Internal;


namespace SmoothGL.Content
{
    /// <summary>
    /// Handles loading of content from files or streams and takes care of disposing loaded content automatically.
    /// </summary>
    public class ContentManager : IDisposable
    {
        private class ContentReaderData
        {
            private IContentReader<object> _reader;
            private bool _allowCaching;

            public ContentReaderData(IContentReader<object> reader, bool allowCaching)
            {
                _reader = reader;
                _allowCaching = allowCaching;
            }

            public IContentReader<object> Reader
            {
                get
                {
                    return _reader;
                }
            }

            public bool AllowCaching
            {
                get
                {
                    return _allowCaching;
                }
            }
        }

        private string _rootPath;
        private Dictionary<Type, IContentReader<object>> _contentReaders;
        private List<IDisposable> _disposables;
        private bool _disposed;

        /// <summary>
        /// Creates a new content manager with readers for default content types already registered.
        /// </summary>
        /// <param name="rootPath">Root directory the paths of content files are relative to.</param>
        /// <returns>Content manager.</returns>
        public static ContentManager CreateDefault(string rootPath)
        {
            ContentManager contentManager = new ContentManager(rootPath);
            contentManager.SetContentReader(new SerializationReader());
            contentManager.SetContentReader(new StringReader());
            contentManager.SetContentReader(new TextureDataReader(true));
            contentManager.SetContentReader(new TextureCubeReader(TextureFilterMode.Default, false));
            contentManager.SetContentReader(new FactoryReader<ShaderProgram, ShaderProgramFactory>());
            contentManager.SetContentReader(new WavefrontOBJReader());
            contentManager.SetContentReader(new VertexArrayReader());

            ColorTextureReader colorTextureReader = new ColorTextureReader(TextureFilterMode.Default, true);
            contentManager.SetContentReader<Texture2D>(colorTextureReader);
            contentManager.SetContentReader<ColorTexture2D>(colorTextureReader);

            return contentManager;
        }

        /// <summary>
        /// Creates a new content manager without any registered content readers.
        /// </summary>
        /// <param name="rootPath">Root directory the paths of content files are relative to.</param>
        public ContentManager(string rootPath)
        {
            _rootPath = rootPath ?? "";
            _contentReaders = new Dictionary<Type, IContentReader<object>>();
            _disposables = new List<IDisposable>();
            _disposed = false;
        }

        /// <summary>
        /// Registers a content reader which handles loading of content of the specified type.
        /// When a reader is already registered for this type, it will be replaced. The content manager is
        /// allowed to cache created content objects of this type.
        /// </summary>
        /// <typeparam name="T">Content type which can be loaded by the reader.</typeparam>
        /// <param name="contentReader">Content reader of the specified type.</param>
        public void SetContentReader<T>(IContentReader<T> contentReader)
        {
            SetContentReader<T>(contentReader, true);
        }

        /// <summary>
        /// Registers a content reader which handles loading of content of the specified type.
        /// When a reader is already registered for this type, it will be replaced. The content manager can be
        /// allowed to cache created content objects of this type to reduce loading times when the same file is
        /// requested multiple times. However, caching can cause problems when content objects are not immutable.
        /// </summary>
        /// <typeparam name="T">Content type which can be loaded by the reader.</typeparam>
        /// <param name="contentReader">Content reader of the specified type.</param>
        /// <param name="allowCaching">Indicates whether the content manager is allowed to cache created content objects.</param>
        public void SetContentReader<T>(IContentReader<T> contentReader, bool allowCaching)
        {
            if (contentReader == null)
                throw new ArgumentNullException("contentReader");

            IContentReader<object> cachedReader = (IContentReader<object>)contentReader;
            if (allowCaching)
                cachedReader = new CachedReader<T>(contentReader);

            _contentReaders[typeof(T)] = cachedReader;
        }

        /// <summary>
        /// Loads content from a file.
        /// </summary>
        /// <typeparam name="T">The requested content type to load.</typeparam>
        /// <param name="filename">Path to the file storing content data.</param>
        /// <returns>Content object.</returns>
        public T Load<T>(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            filename = Path.Combine(_rootPath, filename);
            
            try
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    return Load<T>(stream);
                }
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                throw new ContentLoadException("Cannot find content file " + filename + ".", fileNotFoundException, filename, typeof(T));
            }
            catch (Exception exception)
            {
                throw new ContentLoadException("Unable to load content file " + filename + ":\n" + exception.Message, exception, filename, typeof(T));
            }
        }

        /// <summary>
        /// Loads content from a stream.
        /// </summary>
        /// <typeparam name="T">The requested content type to load.</typeparam>
        /// <param name="stream">Stream from which content data is read.</param>
        /// <returns>Content object.</returns>
        public T Load<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (_disposed)
                throw new ObjectDisposedException("ContentManager", "The object is already disposed.");

            string filename = null;
            FileStream fileStream = stream as FileStream;
            if (fileStream != null)
                filename = fileStream.Name;

            Type requestedType = typeof(T);
            Type type = requestedType;
            do
            {
                IContentReader<object> contentReader;
                if (_contentReaders.TryGetValue(type, out contentReader))
                {
                    if (contentReader.CanReadSubtypes || type == requestedType)
                    {
                        object result = contentReader.Read(stream, requestedType, this);
                        CachedResult cachedResult = result as CachedResult;

                        if(cachedResult != null)
                        {
                            result = cachedResult.Value;
                            if (!cachedResult.IsNew)
                                return (T)result;
                        }

                        IDisposable disposable = result as IDisposable;
                        if (disposable != null)
                            _disposables.Add(disposable);
                        
                        return (T)result;
                    }
                }

                type = type.BaseType;
            }
            while (type != null);
            
            throw new ContentLoadException(string.Format("There is no content reader registered for type {0}.", requestedType), filename, requestedType);
        }

        /// <summary>
        /// Adds a disposable content object so that its lifetime is managed by this content manager.
        /// </summary>
        /// <param name="disposable"></param>
        public void Add(IDisposable disposable)
        {
            if (_disposed)
                throw new ObjectDisposedException("ContentManager", "The object is already disposed.");
            _disposables.Add(disposable);
        }

        /// <summary>
        /// Disposes all content objects managed by this content manager.
        /// </summary>
        public void Unload()
        {
            foreach (ICachedReader cachedReader in _contentReaders.Values.OfType<ICachedReader>())
                cachedReader.ClearCache();

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
        }

        /// <summary>
        /// Disposes all content objects managed by this content manager, as well as the content manager itself.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Unload();
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        ~ContentManager()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the root directory that paths of content files are relative to.
        /// </summary>
        public string RootPath
        {
            get
            {
                return _rootPath;
            }
        }
    }
}
