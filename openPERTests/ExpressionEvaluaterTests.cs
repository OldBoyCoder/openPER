using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openPERHelpers;
using System.Collections;
using System;
using System.Xml.Schema;
using System.Diagnostics;

namespace openPERTests
{
    [TestClass]
    public class ExpressionEvaluaterTests
    {
        private string QueueToString(Queue<string> q)
        {
            var rc = "";
            string item;
            while (q.TryDequeue(result: out item))
            {
                rc += item;
            }
            return rc;
        }
        [TestMethod]
        public void TestSimpleAndShunt()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("AB+", result);
        }
        [TestMethod]
        public void EvaluateSimpleAndShuntTrueTrue()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.True }, { "B", openPERBoolean.True } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(true, rc);
        }
        [TestMethod]
        public void EvaluateSimpleAndShuntTrueFalse()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.True }, { "B", openPERBoolean.False } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(false, rc);
        }
        [TestMethod]
        public void EvaluateSimpleAndShuntFalseFalse()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.False }, { "B", openPERBoolean.False } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(false, rc);
        }
        [TestMethod]
        public void EvaluateSimpleAndShuntFalseTrue()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.False }, { "B", openPERBoolean.True } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(false, rc);
        }
        [TestMethod]
        public void EvaluateSimpleAndShuntTrueUnknown()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.True }, { "B", openPERBoolean.Unknown } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(true, rc);
        }
        [TestMethod]
        public void EvaluateSimpleAndShuntFalseUnknown()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.False }, { "B", openPERBoolean.Unknown } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(false, rc);
        }
        public void EvaluateSimpleAndShuntUnknownUnknown()
        {
            var expr = "A+B";
            var sut = new ExpressionEvaluater();
            var values = new Dictionary<string, openPERBoolean> { { "A", openPERBoolean.Unknown }, { "B", openPERBoolean.Unknown } };
            var rc = sut.Evaluate(expr, values);
            Assert.AreEqual(true, rc);
        }
        [TestMethod]
        public void TestSimpleOrShunt()
        {
            var expr = "A,B";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("AB,", result);
        }
        [TestMethod]
        public void TestSimpleNotShunt()
        {
            var expr = "!A";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("A!", result);
        }
        [TestMethod]
        public void TestSimpleAndNotShunt()
        {
            var expr = "A+!B";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("AB!+", result);
        }
        [TestMethod]
        public void TestSimpleAndOrShunt()
        {
            var expr = "A+B,C";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("AB+C,", result);
        }
        [TestMethod]
        public void TestSimpleAndOrWithBracketsShunt()
        {
            var expr = "A+(B,C)";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("ABC,+", result);
        }
        [TestMethod]
        public void TestSimpleAndOrWithNottedBracketsShunt()
        {
            var expr = "A+!(B,C)";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("ABC,!+", result);
        }
        //(TC2V,TCPAN)+NP4+!399+!614+890
        [TestMethod]
        public void TestePerCondition()
        {
            var expr = "(TC2V,TCPAN)+NP4+!399+!614+890";
            var sut = new ExpressionEvaluater();
            var rc = sut.ShuntingYard(expr);
            var result = QueueToString(rc);
            Assert.AreEqual("TC2VTCPAN,NP4+399!+614!+890+", result);
        }
        [TestMethod]
        public void Test2()
        {
            //3 + 4 × 2 ÷ ( 1 − 5 ) ^ 2 ^ 3
            var expr = "342x15-23^^÷+";
            var oQ = new Queue<string>();
            for (int i = 0; i < expr.Length; i++)
            {
                oQ.Enqueue(expr.Substring(i, 1));
            }
            var t = new Stack<double>();
            string x;
            while (oQ.TryDequeue(out x))
            {
                if (x != "+" && x != "x" && x != "^" && x != "-" && x != "÷")
                {
                    t.Push(double.Parse(x));
                }
                else
                {
                    var o2 = t.Pop();
                    var o1 = t.Pop();
                    if (x == "+")
                        t.Push(o1 + o2);
                    if (x == "-")
                        t.Push(o1 - o2);
                    if (x == "x")
                        t.Push(o1 * o2);
                    if (x == "÷")
                        t.Push(o1 / o2);
                    if (x == "^")
                        t.Push(Math.Pow(o1, o2));
                }
            }
        }
        [TestMethod]
        public void Test3()
        {
            //!true
            var expr = "1!";
            var oQ = new Queue<string>();
            for (int i = 0; i < expr.Length; i++)
            {
                oQ.Enqueue(expr.Substring(i, 1));
            }
            var t = new Stack<bool>();
            string x;
            while (oQ.TryDequeue(out x))
            {
                if (x != "+" && x != "," && x != "!")
                {
                    if (x == "1")
                        t.Push(true);
                    if (x == "0")
                        t.Push(false);
                }
                else
                {
                    if (x == "!")
                    {
                        t.Push(!t.Pop());
                    }
                    else
                    {
                        var o2 = t.Pop();
                        var o1 = t.Pop();
                        if (x == "+")
                            t.Push(o1 && o2);
                        if (x == ",")
                            t.Push(o1 || o2);
                    }
                }
            }
        }

    }
}
