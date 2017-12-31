using System;

public class Lazy<T> where T : class
{
    Func<T> func;
    T value;

    public Lazy(Func<T> func)
    {
        this.func = func;
    }

    public static implicit operator T(Lazy<T> d)
    {
        if (d.value == null)
        {
            d.value = d.func();
        }
        return d.value;
    }
}