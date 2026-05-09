namespace ProsaicLang.Compiler.Data;

public record FileLocation(string FileName, int StartLine, int StartColumn, int EndLine, int EndColumn)
{
    public FileLocation(string fileName, int line, int column) : this(fileName, line, column, line, column)
    {
    }
    
    public static FileLocation Empty => new("<Unknown>", 0, 0, 0, 0);

    public static FileLocation CreateRange(FileLocation a, FileLocation b)
    {
        int startLine = Math.Min(a.StartLine, b.StartLine);
        int startColumn = a.StartLine == startLine ? a.StartColumn : b.StartColumn;
        int endLine = Math.Max(a.EndLine, b.EndLine);
        int endColumn = a.EndLine == endLine ? a.EndColumn : b.EndColumn;
        return new FileLocation(a.FileName, startLine, startColumn, endLine, endColumn);       
    }

    public bool IsEmpty => StartLine == 0 && StartColumn == 0 && EndLine == 0 && EndColumn == 0 &&
                           StartLine == 0 && StartColumn == 0 && EndLine == 0 && EndColumn == 0;

    public override string ToString()
    {
        if (StartLine == EndLine && StartColumn == EndColumn)
        {
            return $"{FileName}: Line {StartLine}, Col {StartColumn}";
        }

        if (StartLine == EndLine)
        {
            return $"{FileName}: Line {StartLine}, Col {StartColumn} - {EndColumn}";
        }
        
        return $"{FileName}: Line {StartLine}, Col {StartColumn} - Line {EndLine}, Col {EndColumn}";
    }
}