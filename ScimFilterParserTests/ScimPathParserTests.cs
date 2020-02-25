namespace ScimFilterParserTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ScimFilterParser.Parser;
    using ScimFilterParser.Parser.AbstractSyntaxTree;

    [TestClass]
    public class ScimPathParserTests
    {
        [TestMethod]
        public void Can_Parse_Path_Attribute_Path()
        {
            var parser = new Parser(ParserMode.Path);

            var path = "userName";
            var pathParsed = parser.Parse(path);

            Assert.IsTrue(pathParsed is Path);
            Assert.AreEqual("userName", (pathParsed as Path).AttributePath.AttributeName);
        }

        [TestMethod]
        public void Can_Parse_Path_Value_Path()
        {
            var parser = new Parser(ParserMode.Path);

            var path = "userName[test eq 100]";
            var pathParsed = parser.Parse(path);

            Assert.IsTrue(pathParsed is Path);
            Assert.AreEqual("userName", (pathParsed as Path).AttributePath.AttributeName);
            Assert.AreEqual("test", ((pathParsed as Path).ValueFilter as ComparisonExpression).LeftOperand.AttributeName);
        }

        [TestMethod]
        public void Can_Parse_Path_Value_Path_With_SubAttribute()
        {
            var parser = new Parser(ParserMode.Path);

            var path = "userName[test eq 100].innerTest";
            var pathParsed = parser.Parse(path);

            Assert.IsTrue(pathParsed is Path);
            Assert.AreEqual("userName", (pathParsed as Path).AttributePath.AttributeName);
            Assert.AreEqual("test", ((pathParsed as Path).ValueFilter as ComparisonExpression).LeftOperand.AttributeName);
            Assert.AreEqual("innerTest", (pathParsed as Path).SubAttribute);
        }
    }
}
