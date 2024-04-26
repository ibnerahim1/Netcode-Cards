using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ForEachExtension
{
    public static void ForEach<T>(this IEnumerable<T> ie, Action<T, int> action)
    {
        var i = 0;
        foreach (var e in ie) action(e, i++);
    }

    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
    {
        var i = 0;
        foreach (var e in ie) { action(e); i++; }
    }
}
