﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethods
{
    //Canvasgroup
    public static void Show(this CanvasGroup canvasGroup, bool state)
    {
        float alpha = 0.0f;
        if (state) { alpha = 1.0f; }

        canvasGroup.alpha = alpha;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    public static bool IsVisible(this CanvasGroup canvasGroup)
    {
        return (canvasGroup.alpha > 0);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        System.Random rand = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Vector2 Copy(this Vector2 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector3 Copy(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }

    public static Quaternion Copy(this Quaternion quaternion)
    {
        return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}
