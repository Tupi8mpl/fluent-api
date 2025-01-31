﻿using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting.Solved;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void ObjectPrinter_ExcludingType()
        {
            var person = new Person {Name = "Alex", Age = 19};

            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>();
            var result = printer.PrintToString(person);
            result.Should().NotContain("Id = Guid");
        }

        [Test]
        public void ObjectPrinter_ExcludingMember()
        {
            var person = new Person {Name = "Alex", Age = 19};

            var printer = ObjectPrinter.For<Person>()
                .Excluding(x => x.Name);
            var result = printer.PrintToString(person);
            result.Should().NotContain("Name = Alex");
        }

        [Test]
        public void ObjectPrinter_UsingCultureInfo()
        {
            var person = new Person {Name = "Alex", Age = 19, Height = 178.5};

            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(CultureInfo.CurrentCulture);
            var result = printer.PrintToString(person);
            result.Should().NotContain("Height = 178.5").And.Contain("Height = 178,5");
        }

        [Test]
        public void ObjectPrinter_UsingPropertySettings()
        {
            var person = new Person {Name = "Alex", Age = 19, Height = 178.5};

            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().Using(x => x.ToUpper());
            var result = printer.PrintToString(person);
            result.Should().NotContain("Name = Alex").And.Contain("Name = ALEX");
        }

        [Test]
        public void ObjectPrinter_PrintSameClass()
        {
            var son = new Person {Name = "Alex", Age = 19, Height = 178.5};

            var father = new Person {Name = "Alex Father", Age = 45, Height = 190.0};
            son.Father = father;

            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>();
            var resultForFather = printer.PrintToString(father);
            var result = printer.PrintToString(son);
            result.Should().Contain(resultForFather.Replace("\t", "\t\t"));
        }

        [Test]
        public void ObjectPrinter_ShouldPrintArray()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var result = array.PrintToString();
            result.Should().Contain("System.Int32[]" +
                                    $"{Environment.NewLine}[" +
                                    $"{Environment.NewLine}\t1" +
                                    $"{Environment.NewLine}\t2" +
                                    $"{Environment.NewLine}\t3" +
                                    $"{Environment.NewLine}\t4" +
                                    $"{Environment.NewLine}\t5" +
                                    $"{Environment.NewLine}]");
        }

        [Test]
        public void ObjectPrinter_ShouldPrintList()
        {
            var list = new List<int>(new int[] {1, 2, 3, 4, 5});
            var result = list.PrintToString();
            result.Should().Contain("System.Collections.Generic.List`1[System.Int32]" +
                                    $"{Environment.NewLine}[" +
                                    $"{Environment.NewLine}\t1" +
                                    $"{Environment.NewLine}\t2" +
                                    $"{Environment.NewLine}\t3" +
                                    $"{Environment.NewLine}\t4" +
                                    $"{Environment.NewLine}\t5" +
                                    $"{Environment.NewLine}]");
        }

        [Test]
        public void ObjectPrinter_ShouldPrintDictionary()
        {
            var dict = new Dictionary<int, int>
            {
                {1, 1},
                {2, 2}
            };
            var result = dict.PrintToString();
            result.Should().Contain("System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]" +
                                    $"{Environment.NewLine}[" +
                                    $"{Environment.NewLine}\tKeyValuePair`2" +
                                    $"{Environment.NewLine}\tKey = 1" +
                                    $"{Environment.NewLine}\tKeyValuePair`2" +
                                    $"{Environment.NewLine}\tKey = 2" +
                                    $"{Environment.NewLine}]");
        }

        [Test]
        public void ObjectPrinter_ShouldPrintCollectionsInClass()
        {
            var printer = ObjectPrinter.For<ExampleCollections<int>>();
            var exampleCollections = new ExampleCollections<int>
            {
                Collection = new int[] {1, 2, 3, 4, 5},
                ListCollection = new List<int>(new[] {1, 2, 3, 4, 5, 6}),
                DictionaryCollection = new Dictionary<int, int>
                {
                    {1, 1},
                    {2, 2}
                }
            };
            var result = printer.PrintToString(exampleCollections);
            result.Should().Contain("Collection")
                .And.Contain("ListCollection")
                .And.Contain("DictionaryCollection");
        }

        [Test]
        public void ObjectPrinter_ShouldNotThrowStackOverflowException()
        {
            var son = new Person {Name = "Alex", Age = 19, Height = 178.5};

            var father = new Person {Name = "Alex Father", Age = 45, Height = 190.0};
            var mother = new Person {Name = "Alex Mother", Age = 45, Height = 170.0};
            son.Father = father;
            son.Mother = mother;
            father.Wife = mother;

            var printer = ObjectPrinter.For<Person>();
            Func<string> printing = () => printer.PrintToString(son);
            printing.Should().NotThrow<StackOverflowException>();
        }

        [Test]
        public void ObjectPrinter_ShouldTrimmedToLength()
        {
            var person = new Person {Name = "Alex", Age = 19, Height = 178.5};
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>()
                .TrimmedToLength(1);
            var result = printer.PrintToString(person);
            result.Should().Contain("Name = A");
        }
    }
}