namespace Scanner.Models.Iterfaces
{
    public interface ICode
    {
        string Name { get; }
        string CodeInfo { get; }
        bool TryParseCode(string code);
    }
}
