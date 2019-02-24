using FluentAssertions;
using System;
using NUnit.Framework;

namespace Challenge
{
    [TestFixture]
    public class WordsStatistics_Tests
    {
        public virtual IWordsStatistics CreateStatistics()
        {
            // меняется на разные реализации при запуске exe
            return new WordsStatistics();
        }

        private IWordsStatistics wordsStatistics;

        [SetUp]
        public void SetUp()
        {
            wordsStatistics = CreateStatistics();
        }

        [Test]
        public void GetStatistics_IsEmpty_AfterCreation()
        {
            wordsStatistics.GetStatistics().Should().BeEmpty();
        }

        [Test]
        public void GetStatistics_ContainsItem_AfterAddition()
        {
            AddWord("abc");
            CheckResult(WordCount("abc", 1));
        }

        [Test]
        public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
        {
            AddWord("abc");
            AddWord("def");
            wordsStatistics.GetStatistics().Should().HaveCount(2);
        }

        [Test]
        public void GetStatistics_HaveThreeRepeats_AfterAdditionOfWordThreeTimes()
        {
            AddWord("abc", 3);
            CheckResult(WordCount("abc", 3));
        }

        [Test]
        public void GetStatistics_HaveThreeRepeats_AfterAdditionOfWordThreeTimesInLowerAndUpper()
        {
            AddWord("Abc");
            AddWord("aBc");
            AddWord("abC");
            CheckResult(WordCount("abc", 3));
        }

	    [Test]
	    public void GetStatistics_ProperlyCountsRussianSymbols()
	    {
		    AddWord("Абв");
		    AddWord("аБв");
		    AddWord("абВ");
		    CheckResult(WordCount("абв", 3));
	    }

	    [Test]
        public void GetStatistics_TakeFirst10Chars_AfterAdditionOfLongWord()
        {
            AddWord("0123456789abcde");
            CheckResult(WordCount("0123456789", 1));
        }

	    [Test]
	    public void GetStatistics_AddsWhitespaces_AfterAdditionOfWhitespacePrefixedLongWord()
	    {
		    AddWord("          a");
		    AddWord("          b");
		    CheckResult(WordCount("          ", 2));
	    }

        [Test]
        public void GetStatistics_ThrowsArgumentNullException_OnAdditionOfNullArgument()
        {
            wordsStatistics.Invoking(o => o.AddWord(null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void GetStatistics_DoesNothing_OnAdditionOfEmptyString()
        {
            AddWord("");
            wordsStatistics.GetStatistics().Should().BeEmpty();
        }

        [Test]
        public void GetStatistics_DoesNothing_OnAdditionOfWhitespace()
        {
            AddWord(" ");
            wordsStatistics.GetStatistics().Should().BeEmpty();
        }

        [Test]
        public void GetStatistics_SortsWordsByCountAndLexicografically()
        {
	        AddWord("bcd", 2);
	        AddWord("abc", 2);
	        AddWord("def", 3);
	        CheckResult(WordCount("def", 3), WordCount("abc", 2), WordCount("bcd", 2));
        }

        [Test]
        public void GetStatistics_ReturnConsistentResults_AfterMultipleCalls()
        {
            AddWord("a");
            CheckResult(WordCount("a", 1));
	        AddWord("a");
	        CheckResult(WordCount("a", 2));
        }

        [Test]
        [Timeout(200)]
        public void GetStatistics_Have1000Elements_AfterAdditionOf1000WordsWith100MsTimeout()
        {
	        for (var i = 0; i < 1000; i++)
				AddWord(i.ToString(), 100);

	        wordsStatistics.GetStatistics().Should().HaveCount(1000);
        }

        [Test]
        public void GetStatistics_ProperlyHandlesWordsWithSpaces()
        {
			AddWord("a b c d");
			CheckResult(WordCount("a b c d", 1));
        }
        [Test]
        public void GetStatistics_ShouldHaveNoStaticDependencies_WithOtherInstances()
        {
			AddWord("abc");
	        CreateStatistics().GetStatistics().Should().BeEmpty();
			CheckResult(WordCount("abc", 1));
        }
        private void CheckResult(params WordCount[] expected)
	    {
		    wordsStatistics.GetStatistics().ShouldAllBeEquivalentTo(expected, 
			    options => options.WithStrictOrdering());
	    }

        private void AddWord(string word, int count = 1)
	    {
			for(var i = 0; i < count; i++)
				wordsStatistics.AddWord(word);
	    }

	    private WordCount WordCount(string word, int count)
	    {
			return new WordCount(word, count);
	    }
	    // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}