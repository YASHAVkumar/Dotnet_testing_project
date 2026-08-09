using System.Numerics;
using System.Runtime.CompilerServices;
using testing_web;

namespace testing.web.tests;

public class CalculateTests
{
    // 1. Define ALL test cases in a single collection
    // public static IEnumerable<object[]> GetPriceTestCases()
    // {
    //     // Format: { inputA, inputB, expectedResult, expectedExceptionType }

    //     // --- Normal Cases ---
    //     yield return new object[] { 10L, 20L, 30M, null! };             // long parameters
    //     yield return new object[] { 5.50f, 4.25f, 9.75M, null! };       // float parameters
    //     yield return new object[] { 8, 20L, 28M, null! };               // mixed types (int and long)

    //     // --- Exception Cases (Negative inputs) ---
    //     yield return new object[] { -5, 20, 0M, typeof(ArgumentException) };
    //     yield return new object[] { 10M, -2.5M, 0M, typeof(ArgumentException) };

    //     // --- Overflow Edge Case ---
    //     // long.MaxValue fits in decimal, but decimal itself has a limit!
    //     yield return new object[] { decimal.MaxValue, 1M, 0M, typeof(OverflowException) };
    // }

    // FIX: Using TheoryData with explicit generic types instead of object[]
    public static TheoryData<decimal, decimal, decimal, Type?> GetPriceTestCases()
    {
        var data = new TheoryData<decimal, decimal, decimal, Type?>
        {
            // .Add() enforces type checking for each parameter at compile time
            { 10M, 20M, 30M, null },                        // Normal case
            { 8M, 20M, 28M, null },                         // Mixed numeric inputs (passed as decimals)
            { -5M, 20M, 0M, typeof(ArgumentException) },    // Exception case
            { decimal.MaxValue, 1M, 0M, typeof(OverflowException) } // Overflow case
        };

        return data;
    }
    public Calculator cal { get; set; }
    [Fact]
    public void Add_ReturnExptectedResult()
    {
        cal = new();
        int result = cal.Add(10, 30);
        Assert.Equal(40, result);

    }

    [Theory]
    [InlineData(1, 5, 6)]
    [InlineData(1, -5, -4)]
    [InlineData(-1, -5, -6)]
    public void Add_OtherInputsExptectedResult(int a, int b, int c)
    {
        cal = new();
        int result = cal.Add(a, b);
        Assert.Equal(c, result);
    }
    [Fact(Skip = "Reason")]
    public void Soon_Test() { }

    [Fact]
    public void Add_TestGenricAddResults()
    {

        cal = new();
        long result = cal.Generic_Add<long>(10L, 20L);
        Assert.Equal(30L, result);  //normal case
        decimal result0 = cal.Generic_Add<decimal>(8.9M, 0.1M);
        Assert.Equal(9.0M, result0);  //failed case

        Assert.Throws<OverflowException>(() =>
        {
            long result1 = cal.Generic_Add(long.MaxValue, 20L);  //Edge case 

        });

    }

    [Fact]
    public void GenricAdd_NegativeValuesTest_ExceptionResult()
    {
        cal = new();
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            cal.Generic_Add(-5M, 20M);
        });

        // Optional: Assert on the specific error message to be extra precise
        Assert.Equal("Prices cannot be negative values.", exception.Message);
    }
    [Theory]
    [MemberData(nameof(GetPriceTestCases))]
    public void Test_AllGenericPriceScenarios<T>(T a, T b, decimal expectedResult, Type expectedException)
        where T : System.Numerics.INumber<T>
    {

        cal = new();
        // Scenario A: If an exception is expected
        if (expectedException != null)
        {
            Assert.Throws(expectedException, () => cal.Generic_Add(a, b));
        }
        // Scenario B: If a regular successful calculation is expected
        else
        {
            decimal actualResult = Convert.ToDecimal(cal.Generic_Add(a, b));
            Assert.Equal(expectedResult, actualResult);
        }
    }
}


//Assert.Equal(10,result);  Assert.NotEqual(5,result); Assert.True(isValid); Assert.False(isDeleted); Assert.Null(customer);
//Assert.NotNull(customer); Assert.Contains("Apple", fruits);  Assert.StartsWith("Hello", message);  Assert.EndsWith(".com", email); Assert.Empty(list);
//