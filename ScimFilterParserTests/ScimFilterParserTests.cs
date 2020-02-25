namespace ScimFilterParserTests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ScimFilterParser.Parser;
    using ScimFilterParser.Parser.AbstractSyntaxTree;
    using ScimFilterParser.Parser.AbstractSyntaxTree.Comparison;
    using ScimFilterParser.Parser.Error;

    [TestClass]
    public class ScimFilterParserTests
    {
        [TestMethod]
        public void Can_Parse_Comparison_Expression_All_Operators()
        {
            var parser = new Parser();

            var comparisonOperators = new List<KeyValuePair<string, Type>>()
            {
                new KeyValuePair<string, Type>("eq", typeof(Equal)),
                new KeyValuePair<string, Type>("ne", typeof(NotEqual)),
                new KeyValuePair<string, Type>("co", typeof(Contains)),
                new KeyValuePair<string, Type>("sw", typeof(StartsWith)),
                new KeyValuePair<string, Type>("ew", typeof(EndsWith)),
                new KeyValuePair<string, Type>("lt", typeof(LessThan)),
                new KeyValuePair<string, Type>("gt", typeof(GreaterThan)),
                new KeyValuePair<string, Type>("le", typeof(LessThanOrEqual)),
                new KeyValuePair<string, Type>("ge", typeof(GreaterThanOrEqual))
            };

            foreach (var @operator in comparisonOperators)
            {
                var filter = $"userName {@operator.Key} \"bjensen\"";
                var expression = parser.Parse(filter) as Expression;

                Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

                var comparisonExpression = expression as Term as Factor as ComparisonExpression;

                Assert.IsNull(comparisonExpression.LeftOperand.Schema);
                Assert.IsInstanceOfType(comparisonExpression.Operator, @operator.Value);
            }
        }

        [TestMethod]
        public void Can_Parse_Present_Expression()
        {
            var parser = new Parser();
            
            var filter = $"userName pr";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;

            Assert.IsNull(comparisonExpression.LeftOperand.Schema);
            Assert.IsInstanceOfType(comparisonExpression.Operator, typeof(Present));
            Assert.AreEqual(null, comparisonExpression.RightOperand);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_True()
        {
            var parser = new Parser();

            var filter = $"test eq true";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(true, compareValue.Value);
            Assert.IsTrue(compareValue.IsBool);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_False()
        {
            var parser = new Parser();

            var filter = $"test eq false";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(false, compareValue.Value);
            Assert.IsTrue(compareValue.IsBool);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Null()
        {
            var parser = new Parser();

            var filter = $"test eq null";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(null, compareValue.Value);
            Assert.IsTrue(compareValue.IsNull);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Plain_Int()
        {
            var parser = new Parser();

            var filter = $"test eq 100";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual((double)100, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Negative_Int()
        {
            var parser = new Parser();

            var filter = $"test eq -100";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual((double)-100, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Double_With_Fraction()
        {
            var parser = new Parser();

            var filter = $"test eq 100.019";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(100.019, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Double_With_Fraction_And_Exponent()
        {
            var parser = new Parser();

            var filter = $"test eq 100.019e5";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(100.019e5, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Plain_Int_With_Exponent()
        {
            var parser = new Parser();

            var filter = $"test eq 100e2";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(100e2, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Plain_Int_With_Negative_Exponent()
        {
            var parser = new Parser();

            var filter = $"test eq 100e-2";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(100e-2, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Plain_Int_With_Positive_Exponent()
        {
            var parser = new Parser();

            var filter = $"test eq 100e+2";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(100e+2, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_Number_Plain_Int_With_Exponent_Capital_E()
        {
            var parser = new Parser();

            var filter = $"test eq 100E2";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual(100E2, compareValue.Value);
            Assert.IsTrue(compareValue.IsNumber);
        }

        [TestMethod]
        public void Can_Parse_Compare_Value_String()
        {
            var parser = new Parser();

            var filter = $"test eq \"bjensen\"";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;
            var compareValue = comparisonExpression.RightOperand;

            Assert.IsNotNull(compareValue);
            Assert.AreEqual("bjensen", compareValue.Value);
            Assert.IsTrue(compareValue.IsString);
        }

        [TestMethod]
        public void Can_Parse_Present_Grouped_Expression()
        {
            var parser = new Parser();
            
            var filter = $"(test eq true)";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is GroupedExpression);

            var groupedExpression = expression as Term as Factor as GroupedExpression;
            var innerExpression = groupedExpression.Expression;

            Assert.IsTrue(innerExpression is Term innerTerm && innerTerm is Factor innerFactor && innerFactor is ComparisonExpression);
        }

        [TestMethod]
        public void Can_Parse_Present_Negated_Expression()
        {
            var parser = new Parser();
            
            var filter = $"not (test eq true)";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is NegatedExpression);

            var negatedExpression = expression as Term as Factor as NegatedExpression;
            var groupedExpression = negatedExpression.Expression;
            var innerExpression = groupedExpression.Expression;

            Assert.IsTrue(innerExpression is Term innerTerm && innerTerm is Factor innerFactor && innerFactor is ComparisonExpression);
        }

        [TestMethod]
        public void Can_Parse_Attribute_Path_With_Schema()
        {
            var parser = new Parser();
            
            var filter = $"urn:ietf:params:scim:schemas:core:2.0:User:test eq true";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;

            var attributePath = comparisonExpression.LeftOperand;
            Assert.IsNotNull(attributePath.Schema);
            Assert.AreEqual("urn:ietf:params:scim:schemas:core:2.0:User", attributePath.Schema);
            Assert.IsNotNull(attributePath.AttributeName);
            Assert.AreEqual("test", attributePath.AttributeName);
        }

        [TestMethod]
        public void Can_Parse_Attribute_Path_With_SubAttribute()
        {
            var parser = new Parser();
            
            var filter = $"test.subTest eq true";
            var expression = parser.Parse(filter) as Expression;

            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ComparisonExpression);

            var comparisonExpression = expression as Term as Factor as ComparisonExpression;

            var attributePath = comparisonExpression.LeftOperand;
            Assert.IsNotNull(attributePath.AttributeName);
            Assert.AreEqual("test", attributePath.AttributeName);
            Assert.IsNotNull(attributePath.SubAttribute);
            Assert.AreEqual("subTest", attributePath.SubAttribute);
        }
        
        [TestMethod]
        public void Can_Parse_Disjunction()
        {
            var parser = new Parser();
            
            var filter = $"test.subTest eq true or userName pr";
            var expression = parser.Parse(filter) as Expression;
            
            Assert.IsTrue(expression is Disjunction);

            var disjunction = expression as Disjunction;

            var leftOperand = disjunction.LeftOperand;
            var rightOperand = disjunction.RightOperand;

            Assert.IsTrue(leftOperand is Term leftTerm && leftTerm is Factor leftFactor && leftFactor is ComparisonExpression);
            Assert.IsTrue(rightOperand is Term rightTerm && rightTerm is Factor rightFactor && rightFactor is ComparisonExpression);
        }

        [TestMethod]
        public void Can_Parse_Conjunction()
        {
            var parser = new Parser();
            
            var filter = $"test.subTest eq true and userName pr";
            var expression = parser.Parse(filter) as Expression;
            
            Assert.IsTrue(expression is Term term && term is Conjunction);

            var conjunction = expression as Term as Conjunction;

            var leftOperand = conjunction.LeftOperand;
            var rightOperand = conjunction.RightOperand;

            Assert.IsTrue(leftOperand is Term leftTerm && leftTerm is Factor leftFactor && leftFactor is ComparisonExpression);
            Assert.IsTrue(rightOperand is Term rightTerm && rightTerm is Factor rightFactor && rightFactor is ComparisonExpression);
        }

        [TestMethod]
        public void Can_Parse_Logical_Expression_Complex()
        {
            var parser = new Parser();
            
            var filter = $"(test.subTest eq true and userName pr) or otherTest ne 100";
            var expression = parser.Parse(filter) as Expression;
            
            Assert.IsTrue(expression is Disjunction);

            var outerdisjuncTion = expression as Disjunction;

            var leftOperand = outerdisjuncTion.LeftOperand;
            var rightOperand = outerdisjuncTion.RightOperand;

            Assert.IsTrue(leftOperand is Expression leftExpression && leftExpression is GroupedExpression);
            Assert.IsTrue(rightOperand is Expression rightExpresion 
                          && rightExpresion is Term rightTerm 
                          && rightTerm is Factor rightFactor 
                          && rightFactor is ComparisonExpression);
        }

        [TestMethod]
        public void Can_Parse_Nested_Parens()
        {
            var parser = new Parser();
            
            var filter = $"((test eq true))";
            var expression = parser.Parse(filter) as Expression;
            
            Assert.IsTrue(expression is GroupedExpression outerGroup && outerGroup is GroupedExpression);

            var nextedExpression = expression as GroupedExpression as GroupedExpression;

            var innerExpression = (nextedExpression.Expression as GroupedExpression).Expression;
            Assert.IsTrue(innerExpression is Term term && term is Factor factor && factor is ComparisonExpression);
        }

        [TestMethod]
        public void Can_Parse_Value_Path()
        {
            var parser = new Parser();
            
            var filter = $"test[subTest eq 100]";
            var expression = parser.Parse(filter) as Expression;
            
            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ValuePath);

            var valuePath = expression as Term as Factor as ValuePath;

            Assert.AreEqual("test", valuePath.AttributePath.AttributeName);
            Assert.AreEqual("subTest", (valuePath.ValueFilter as ComparisonExpression).LeftOperand.AttributeName);
        }

        [TestMethod]
        public void Can_Parse_Logical_Expression_Within_Value_Path()
        {
            var parser = new Parser();
            
            var filter = $"test[(subTest eq 100) or otherThing co \"innerText\"]";
            var expression = parser.Parse(filter) as Expression;
            
            Assert.IsTrue(expression is Term term && term is Factor factor && factor is ValuePath);

            var valuePath = expression as Term as Factor as ValuePath;

            Assert.AreEqual("test", valuePath.AttributePath.AttributeName);

            var innerDisjunction = valuePath.ValueFilter as Disjunction;

            Assert.IsTrue((innerDisjunction.LeftOperand as GroupedExpression).Expression is ComparisonExpression);
            Assert.AreEqual("innerText", (innerDisjunction.RightOperand as ComparisonExpression).RightOperand.Value);
        }

        [TestMethod]
        public void Throws_When_Parsing_Invalid_Value_Filter_Value_Path()
        {
            var parser = new Parser();
            
            var filter = $"test[subtest[user ne null]]";

            Assert.ThrowsException<ValueFilterParsingException>(() => parser.Parse(filter));
        }

        [TestMethod]
        public void Throws_When_Parsing_Invalid_Value_Filter_Negated_Value_Path()
        {
            var parser = new Parser();
            
            var filter = $"test[not (subtest[user ne null])]";

            Assert.ThrowsException<ValueFilterParsingException>(() => parser.Parse(filter));
        }

        [TestMethod]
        public void Throws_When_Parsing_Invalid_Value_Filter_Grouped_Value_Path()
        {
            var parser = new Parser();
            
            var filter = $"test[(subtest[user ne null])]";

            Assert.ThrowsException<ValueFilterParsingException>(() => parser.Parse(filter));
        }

        [TestMethod]
        public void Throws_When_Parsing_Invalid_Attribute_Path_Too_Many_SubAttributes()
        {
            var parser = new Parser();
            
            var filter = $"test.one.two eq 100";

            Assert.ThrowsException<AttributePathParsingException>(() => parser.Parse(filter));
        }
        
        [TestMethod]
        public void Throws_When_Parsing_Invalid_Attribute_Path()
        {
            var parser = new Parser();
            
            var filter = $"test##test eq 100";

            Assert.ThrowsException<FilterException>(() => parser.Parse(filter));
        }
        
        [TestMethod]
        public void Parser_Constructor_Throws_When_Passed_Invalid_Arguments()
        {
            Assert.ThrowsException<ArgumentException>(() => new Parser(ParserMode.Path, SCIMVersion.V1));
        }

        [TestMethod]
        public void Throws_When_Parsing_Value_Path_With_Version_V1()
        {
            var parser = new Parser(null, SCIMVersion.V1);
            
            var filter = $"test[user ne null]";

            Assert.ThrowsException<FilterException>(() => parser.Parse(filter));
        }
    }
}