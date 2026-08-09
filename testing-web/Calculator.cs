using System.Data;
using System.Numerics;

namespace testing_web;

public class Calculator
{
   public int Add(int a,int b)=>a+b;
   public int Subtract(int a,int b)=>a-b;
    public T Generic_Add<T>(T a, T b) where T : INumber<T>
    {
        return (Convert.ToDecimal(a) < 0 || Convert.ToDecimal(b) < 0) ?
            throw new ArgumentException("Prices cannot be negative values.") : checked(a + b);
    }
}
