namespace ScimFilterParserTests
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ScimFilterParser.Lexer;
    using ScimFilterParser.Lexer.Config;
    using ScimFilterParser.Lexer.Error;

    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void Throws_When_Scanning_Invalid_Input()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            Assert.ThrowsException<UnknownTokenException>(() => lexer.Scan("2 +3 /4 -1 @@"));
        }

        [TestMethod]
        public void Can_Scan_Valid_Input()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");

            Assert.AreEqual(lexer.Token.Value, "2");
        }

        [TestMethod]
        public void Can_Move_Next()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");

            Assert.AreEqual(lexer.Token.Name, "number");
            Assert.AreEqual(lexer.Token.Value, "2");
            Assert.AreEqual(lexer.LookAhead.Name, "whitespace");
            Assert.AreEqual(lexer.LookAhead.Value, " ");

            lexer.MoveNext();
            lexer.MoveNext();

            Assert.AreEqual(lexer.Token.Name, "plus");
            Assert.AreEqual(lexer.Token.Value, "+");
            Assert.AreEqual(lexer.LookAhead.Name, "number");
            Assert.AreEqual(lexer.LookAhead.Value, "3");
            
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();
            
            Assert.AreEqual(lexer.Token.Name, "div");
            Assert.AreEqual(lexer.Token.Value, "/");
            Assert.AreEqual(lexer.LookAhead.Name, "number");
            Assert.AreEqual(lexer.LookAhead.Value, "4");
            
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();

            Assert.AreEqual(lexer.Token.Name, "whitespace");
            Assert.AreEqual(lexer.Token.Value, " ");
            Assert.IsNull(lexer.LookAhead);
        }

        [TestMethod]
        public void Can_Peek()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");
            Token token;

            lexer.MoveNext();
            lexer.MoveNext();

            token = lexer.Peek();
            Assert.AreEqual("3", token.Value);
            
            token = lexer.Peek();
            Assert.AreEqual(" ", token.Value);
            
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.Peek();
            
            token = lexer.Peek();
            Assert.AreEqual("4", token.Value);
            
            lexer.ResetPeek();
            token = lexer.Peek();
            Assert.AreEqual("/", token.Value);
        }

        [TestMethod]
        public void Can_Skip_Until()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");

            lexer.MoveNext();
            lexer.SkipUntil("minus");

            Assert.AreEqual("minus", lexer.Token.Name);
            Assert.AreEqual("1", lexer.LookAhead.Value);
        }

        [TestMethod]
        public void Can_Check_Tokens()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");

            Assert.IsFalse(lexer.IsNextToken("number"));
            Assert.IsTrue(lexer.IsNextToken("whitespace"));

            lexer.MoveNext();
            lexer.MoveNext();
            Assert.IsTrue(lexer.IsTokenAny(new List<string>{ "minus", "plus" }));
            Assert.IsTrue(lexer.IsNextToken("number"));

            lexer.MoveNext();
            Assert.IsTrue(lexer.IsToken("number"));
        }

        [TestMethod]
        public void Can_Reset_Position()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");

            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();
            lexer.MoveNext();

            lexer.ResetPosition();

            Assert.AreEqual(lexer.Token.Value, "2");
        }

        [TestMethod]
        public void Can_Glimpse()
        {
            var lexer = new Lexer(this.GetAlgebraConfig());
            lexer.Scan("2 +3 /4 -1 ");

            lexer.MoveNext();

            Assert.AreEqual(lexer.Glimpse().Value, "+");
            Assert.AreEqual(lexer.Glimpse().Value, "+");
            Assert.AreEqual(lexer.Glimpse().Value, "+");
        }

        private LexerConfig GetAlgebraConfig()
        {
            return new LexerConfig(new List<KeyValuePair<string, Regex>>()
            {
                new KeyValuePair<string, Regex>("whitespace", new Regex("^\\s+")),
                new KeyValuePair<string, Regex>("number", new Regex("^\\d+")),
                new KeyValuePair<string, Regex>("plus", new Regex("^\\+")),
                new KeyValuePair<string, Regex>("minus", new Regex("^-")),
                new KeyValuePair<string, Regex>("mul", new Regex("^\\*")),
                new KeyValuePair<string, Regex>("div", new Regex("^/")),
            });
        }
    }
}
