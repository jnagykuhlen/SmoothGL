namespace SmoothGL.Content.Internal;

public class ContentDirectory(string rootPath)
{
    public FileStream OpenRead(string relativeFilePath)
    {
        var filePath = FullFilePath(relativeFilePath);
        try
        {
            return File.OpenRead(filePath);
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            throw new ContentLoadException($"Cannot find content file {filePath}.", fileNotFoundException, filePath);
        }
        catch (Exception exception)
        {
            throw new ContentLoadException($"Unable to read content file {filePath}:\n{exception.Message}", exception, filePath);
        }
    }
    
    public DateTime GetLastWriteTime(string relativeFilePath)
    {
        var filePath = FullFilePath(relativeFilePath);
        try
        {
            return File.GetLastWriteTime(filePath);
        }
        catch (Exception exception)
        {
            throw new ContentLoadException($"Unable to read write time for content file {filePath}:\n{exception.Message}", exception, filePath);
        }
    }

    private string FullFilePath(string relativeFilePath) => Path.Combine(rootPath, relativeFilePath);
}