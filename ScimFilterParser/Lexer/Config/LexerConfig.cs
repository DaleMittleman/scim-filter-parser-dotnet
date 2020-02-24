namespace ScimFilterParser.Lexer.Config
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class LexerConfig : ILexerConfig
    {
        public LexerConfig(IEnumerable<TokenDefinition> tokenDefinitions)
        {
            this.TokenDefinitions = tokenDefinitions.ToList();
        }

        public LexerConfig(IEnumerable<KeyValuePair<string, Regex>> tokenDefinitions)
        {
            this.TokenDefinitions = tokenDefinitions.Select(td => new TokenDefinition(td.Key, td.Value));
        }

        public LexerConfig(IEnumerable<KeyValuePair<string, string>> tokenDefinitions)
        {
            var tokenDefinitionsLocal = new List<TokenDefinition>();
            foreach (var tokenDefinition in tokenDefinitions)
            {
                tokenDefinitionsLocal.Add(new TokenDefinition(tokenDefinition.Key, new Regex(tokenDefinition.Value)));
            }

            this.TokenDefinitions = tokenDefinitionsLocal;
        }

        public IEnumerable<TokenDefinition> TokenDefinitions { get; }
    }
}
