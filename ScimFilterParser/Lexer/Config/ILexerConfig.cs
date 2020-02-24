namespace ScimFilterParser.Lexer.Config
{
    using System.Collections.Generic;

    public interface ILexerConfig
    {
        IEnumerable<TokenDefinition> TokenDefinitions { get; }
    }
}
