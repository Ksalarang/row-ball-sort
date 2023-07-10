using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions {
public static class IListExtensions {
    public static void shuffle<T>(this IList<T> list) {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}
}