using CubiclesPrinterLib.Data;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// Delegate AllowPrintDocument
    /// </summary>
    /// <param name="title">title</param>
    public delegate void AllowPrintDocument(PrintJobTitle title);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    public delegate void AddJobWatcher(PrintJobTitle title);
}
