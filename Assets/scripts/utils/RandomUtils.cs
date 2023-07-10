using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils {
public static class RandomUtils {
    public static T nextEnum<T>() where T : Enum {
        var values = Enum.GetValues(typeof(T));
        return (T) values.GetValue(Random.Range(0, values.Length));
    }

    public static List<T> generateEnums<T>(int totalAmount, params float[] proportions) where T : Enum {
        var values = Enum.GetValues(typeof(T));
        var list = new List<T>();
        for (var i = 0; i < proportions.Length && list.Count < totalAmount; i++) {
            var amount = Mathf.RoundToInt(proportions[i] * totalAmount);
            if (totalAmount > 0 && amount == 0) amount = 1;
            for (var j = 0; j < amount; j++) {
                list.Add((T) values.GetValue(i));
                if (list.Count == totalAmount) break;
            }
        }
        if (list.Count < totalAmount) {
            var remainder = totalAmount - list.Count;
            for (var i = 0; i < remainder; i++) {
                list.Add(nextEnum<T>());
            }
        }
        return list;
    }

    public static T nextItem<T>(IList<T> list) => list[Random.Range(0, list.Count)];

    public static bool nextBool() => Random.value > 0.5f;

    public static float nextFloat(float min = 0f, float max = 1f) => min + Random.value * (max - min);
}
}