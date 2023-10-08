namespace Localizer.Core.Model;

public class ResxFileReadProgressEventArg : EventArgs
{
    /// <summary>
    /// Name of the file that was being read
    /// </summary>
    public string FileName { get; set; }
    public int Progress { get; set; }
    public int Total { get; set; }
    public ResxFileReadProgressEventArg(string fileName, int progress, int total)
    {
        FileName = fileName;
        Progress = progress;
        Total = total;
    }
}

public class ResxReadFinishedEventArgs : EventArgs
{
    /// <summary>
    /// Name of the file that was being read
    /// </summary>
    public string FileName { get; set; }
    public int Total { get; set; }
    public ResxReadFinishedEventArgs(string fileName, int total)
    {
        FileName = fileName;
        Total = total;
    }
}