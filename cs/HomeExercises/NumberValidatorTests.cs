using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void NumberValidator_ShouldThrow_OnNegativePrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        }

	    [Test]
	    public void NumberValidator_ShouldThrow_OnZeroPrecision()
	    {
		    Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2, true));
	    }

        [Test]
        public void NumberValidator_ShouldThrow_OnNegativeScale()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1, true));
        }

        [Test]
        public void NumberValidator_ShouldThrow_OnScaleGreaterThanPrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
        }

	    [Test]
	    public void NumberValidator_ShouldThrow_OnScaleEqualToPrecision()
	    {
		    Assert.Throws<ArgumentException>(() => new NumberValidator(1, 1, true));
	    }

        [TestCase("0", 17, 2, false)]
        [TestCase("0.0", 17, 2, false)]
        [TestCase("0,0", 17, 2, false)]
        [TestCase("0", 1, 0, false)]
        [TestCase("0", 1, 0, true)]
        [TestCase("+0", 2, 0, false)]
        [TestCase("+0", 2, 0, true)]
        [TestCase("-0", 2, 0, false)]
        [TestCase("+1.23", 4, 2, false)]
        [TestCase("+1.23", 4, 2, true)]
        [TestCase("-1.23", 4, 2, false)]
        public void TestValidNumbers(string value, int precision, int scale, bool onlyPositive)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
                .Should().BeTrue("because \"{0}\" number is valid {3}number of form N({1}.{2})", 
		            value, precision, scale, onlyPositive ? "non-negative " : "");
        }

        [TestCase(null, 17, 2, false)]
        [TestCase("", 17, 2, false)]
        [TestCase("++0", 17, 2, false)]
        [TestCase("+-0", 17, 2, false)]
        [TestCase("a", 17, 2, false)]
        [TestCase("a.sd", 17, 2, false)]
        [TestCase(".0", 17, 2, false)]
        [TestCase(",0", 17, 2, false)]
        [TestCase("0.", 17, 2, false)]
        [TestCase("0,", 17, 2, false)]
        [TestCase("0*0", 17, 2, false)]
        [TestCase("-0", 17, 0, true)]
        [TestCase("-0.0", 17, 2, true)]
        [TestCase("00.00", 3, 2, false)]
        [TestCase("-0.00", 3, 2, false)]
        [TestCase("+0.00", 3, 2, false)]
        [TestCase("0.000", 17, 2, false)]
        public void TestInvalidNumbers(string value, int precision, int scale, bool onlyPositive)
	    {
		    new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
			    .Should().BeFalse("because \"{0}\" number is invalid {3}number of form N({1}.{2})",
				    value, precision, scale, onlyPositive ? "non-negative " : "");
	    }
    }

    public class NumberValidator
    {
        private readonly Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision;
        private readonly int scale;

        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;
            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");
            if (scale < 0 || scale >= precision)
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            // Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

            if (string.IsNullOrEmpty(value))
                return false;

            var match = numberRegex.Match(value);
            if (!match.Success)
                return false;

            // Знак и целая часть
            var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
            // Дробная часть
            var fracPart = match.Groups[4].Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
                return false;

            if (onlyPositive && match.Groups[1].Value == "-")
                return false;
            return true;
        }
    }
}