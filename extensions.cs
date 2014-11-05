using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionsSet
{
    public static bool ApproxEquals(this float f1, float f2, float precision = 3f)
    {
        return (Mathf.Abs(f1 - f2) <= Mathf.Pow(10.0f, -precision));
    }

    public static void SelfDestruct(this MonoBehaviour m)
    {
        MonoBehaviour.Destroy(m.gameObject);
    }

    public static void ResetTransform(this Transform t)
    {
        t.position = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = new Vector3(1, 1, 1);
    }


    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject gameObject, string tag)
     where T : Component
    {
        List<T> results = new List<T>();

        if (gameObject.CompareTag(tag))
            results.Add(gameObject.GetComponent<T>());

        foreach (Transform t in gameObject.transform)
            results.AddRange(t.gameObject.GetComponentsInChildrenWithTag<T>(tag));

        return results.ToArray();
    }

    public static int GetCollisionMask(this GameObject gameObject, int layer = -1)
    {
        if (layer == -1)
            layer = gameObject.layer;

        int mask = 0;
        for (int i = 0; i < 32; i++)
            mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

        return mask;
    }

    public static Color setAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static IEnumerator FadeIn(this GUIText text, float spawnDelay)
    {
        float spawntime = Time.time;
        while (Time.time - spawntime < spawnDelay)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, (Time.time - spawntime) / spawnDelay);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
    }
    public static IEnumerator FadeOut(this GUIText text, float despawnDelay)
    {
        float spawntime = Time.time;
        while (Time.time - spawntime < despawnDelay)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, (despawnDelay - (Time.time - spawntime)) / despawnDelay);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
    }

    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }
    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }
    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }
    public static Vector3 toVector3(this Vector2 v, float newZ = 0f)
    {
        return new Vector3(v.x, v.y, newZ);
    }

    #region Random
    //Vector
    public static Vector3 RandomRangeXY(this Vector3 v, float rangeMinX, float rangeMaxX, float rangeMinY = float.NaN, float rangeMaxY = float.NaN)
    {
        if (rangeMinY == float.NaN) rangeMinY = rangeMinX;
        if (rangeMaxY == float.NaN) rangeMaxY = rangeMaxX;
        return v = new Vector3(Random.Range(rangeMinX, rangeMaxX), Random.Range(rangeMinY, rangeMaxY), 0f);
    }

    public static Vector3 RandomRangeXYZ(this Vector3 v, float rangeMinX, float rangeMaxX, float rangeMinY = float.NaN, float rangeMaxY = float.NaN,
                                         float rangeMinZ = float.NaN, float rangeMaxZ = float.NaN)
    {
        if (rangeMinY == float.NaN) rangeMinY = rangeMinX;
        if (rangeMaxY == float.NaN) rangeMaxY = rangeMaxX;
        if (rangeMinZ == float.NaN) rangeMinZ = rangeMinX;
        if (rangeMaxZ == float.NaN) rangeMaxZ = rangeMaxX;
        return v = new Vector3(Random.Range(rangeMinX, rangeMaxX), Random.Range(rangeMinY, rangeMaxY), Random.Range(rangeMinZ, rangeMaxZ));
    }
    public static Vector2 RandomRangeXY(this Vector2 v, float rangeMinX, float rangeMaxX, float rangeMinY = float.NaN, float rangeMaxY = float.NaN)
    {
        if (rangeMinY == float.NaN) rangeMinY = rangeMinX;
        if (rangeMaxY == float.NaN) rangeMaxY = rangeMaxX;
        return v = new Vector2(Random.Range(rangeMinX, rangeMaxX), Random.Range(rangeMinY, rangeMaxY));
    }

    //Color
    public static Color RandomColor(this Color c, bool withAlpha = false, bool closeToCurrent = false)
    {
        if (closeToCurrent)
        {
            return c = new Color(Mathf.Clamp01(c.r + Random.Range(-.1f, .1f)), Mathf.Clamp01(c.g + Random.Range(-.1f, .1f)),
                                 Mathf.Clamp01(c.b + Random.Range(-.1f, .1f)), withAlpha ? Mathf.Clamp01(c.a + Random.Range(-.1f, .1f)) : c.a);
        }
        else
        {
            return c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), withAlpha ? Random.Range(0f, 1f) : c.a);
        }
    }
    public static void RandomColor(this Renderer r, bool withAlpha = false, bool closeToCurrent = false)
    {
        r.material.color = r.material.color.RandomColor(withAlpha, closeToCurrent);
    }
    #endregion


    #region tweening
    public enum Easing { Linear, Quad, QuadOut, Bounce };

    public static float easeStep(float step, Easing easing)
    {
        switch (easing)
        {
            case Easing.Linear:
                return step;
            case Easing.Quad:
                return QuadEase(step);
            case Easing.QuadOut:
                return QuadOutEase(step);
            case Easing.Bounce:
                return BounceEase(step);
        }
        return step;
    }

    static float SinEase(float step)
    {
        return (Mathf.Sin(step * Mathf.PI - Mathf.PI / 2f) + 1) / 2;
    }
    static float QuadEase(float step)
    {
        if (step < 0.5f) return step * step * 2;
        step *= 2;
        return -0.5f * (Mathf.Pow(step - 2, 2) + -2);
    }
    static float QuadOutEase(float step)
    {
        return -1f * (Mathf.Pow(step - 1, 2) + -1);
    }
    static float BounceEase(float step)
    {
        if (step < (1 / 2.75))
        {
            return 7.5625f * step * step;
        }
        else if (step < (2 / 2.75))
        {
            return 7.5625f * (step -= (1.5f / 2.75f)) * step + 0.75f;
        }
        else if (step < (2.5 / 2.75))
        {
            return 7.5625f * (step -= (2.25f / 2.75f)) * step + 0.9375f;
        }
        else
        {
            return 7.5625f * (step -= (2.625f / 2.75f)) * step + 0.984375f;
        }
    }
    /*
     * alpha tweening
     */
    public static IEnumerator FadeTo(this Renderer renderer, float delay, float to = 0, Easing easing = Easing.Linear)
    {
        return renderer.FadeFromTo(delay, renderer.material.color.a, to, easing);
    }
    public static IEnumerator FadeFromTo(this Renderer renderer, float delay, float from = 1, float to = 0, Easing easing = Easing.Linear)
    {
        Material material = renderer.material;
        float spawntime = Time.time;
        float currentTime = Time.time - spawntime;
        float step = 0;
        while (currentTime < delay)
        {
            currentTime = Time.time - spawntime;
            step = easeStep(currentTime / delay, easing);
            material.color = new Color(material.color.r, material.color.g, material.color.b, Mathf.Lerp(from, to, step));
            yield return null;
        }
        material.color = new Color(material.color.r, material.color.g, material.color.b, to);
    }

    public static IEnumerator FadeBounce(this Renderer renderer, float delay, float from = 1, float to = 0, Easing easing = Easing.Linear)
    {
        while (true)
        {
            IEnumerator ie = renderer.FadeFromTo(delay, from, to, easing);
            while (ie.MoveNext())
            {
                yield return null;
            }
            IEnumerator ie2 = renderer.FadeFromTo(delay, to, from, easing);
            while (ie2.MoveNext())
            {
                yield return null;
            }
        }
    }

    /*
     * rotation tweening
     */
    public static IEnumerator RotateTo(this Transform t, float delay, Vector3 to, Easing easing = Easing.Linear)
    {
        return t.RotateFromTo(delay, t.localEulerAngles, to, easing);
    }

    public static IEnumerator RotateFromTo(this Transform t, float delay, Vector3 from, Vector3 to, Easing easing = Easing.Linear)
    {
        float spawntime = Time.time;
        float currentTime = Time.time - spawntime;
        float step = 0;
        while (currentTime < delay)
        {
            currentTime = Time.time - spawntime;
            step = easeStep(currentTime / delay, easing);
            t.localEulerAngles = Vector3.Lerp(from, to, step);
            yield return null;
        }
        t.localEulerAngles = to;
    }
    public static IEnumerator RotateBounce(this Transform t, float delay, Vector3 from, Vector3 to, Easing easing = Easing.Linear)
    {
        while (true)
        {
            IEnumerator ie = t.RotateFromTo(delay, from, to, easing);
            while (ie.MoveNext())
            {
                yield return null;
            }
            IEnumerator ie2 = t.RotateFromTo(delay, to, from, easing);
            while (ie2.MoveNext())
            {
                yield return null;
            }
        }
    }

    /*
     * scale tweening
     */
    public static IEnumerator ScaleTo(this Transform t, float delay, float to = 1, Easing easing = Easing.Linear)
    {
        return t.ScaleFromTo(delay, t.localScale.x, to, t.localScale.y, to, easing);
    }
    public static IEnumerator ScaleFromTo(this Transform t, float delay, float from = 1, float to = 1, Easing easing = Easing.Linear)
    {
        return t.ScaleFromTo(delay, from, to, from, to, easing);
    }

    public static IEnumerator ScaleFromTo(this Transform t, float delay, float fromX = 1, float toX = 1, float fromY = 1, float toY = 1, Easing easing = Easing.Linear)
    {
        float spawntime = Time.time;
        float currentTime = Time.time - spawntime;
        float step = 0;
        while (currentTime < delay)
        {
            currentTime = Time.time - spawntime;
            step = easeStep(currentTime / delay, easing);
            t.localScale = new Vector3(Mathf.Lerp(fromX, toX, step), Mathf.Lerp(fromY, toY, step), t.localScale.z);
            yield return null;
        }
        t.localScale = new Vector3(toX, toY, t.localScale.z);
    }
    public static IEnumerator ScaleBounce(this Transform t, float delay, float from = 1, float to = 1, Easing easing = Easing.Linear)
    {
        while (true)
        {
            IEnumerator ie = t.ScaleFromTo(delay, from, to, easing);
            while (ie.MoveNext())
            {
                yield return null;
            }
            IEnumerator ie2 = t.ScaleFromTo(delay, to, from, easing);
            while (ie2.MoveNext())
            {
                yield return null;
            }
        }
    }

    /*
     * position tweening
     */
    public static IEnumerator MoveTo(this Transform t, float delay, Vector2 to, Easing easing = Easing.Linear)
    {
        return t.MoveFromTo(delay, t.position, to.toVector3(), easing);
    }
    public static IEnumerator MoveFromTo(this Transform t, float delay, Vector2 from, Vector2 to, Easing easing = Easing.Linear)
    {
        return t.MoveFromTo(delay, from.toVector3(), to.toVector3(), easing);
    }

    public static IEnumerator MoveFromTo(this Transform t, float delay, Vector3 from, Vector3 to, Easing easing = Easing.Linear)
    {
        float spawntime = Time.time;
        float currentTime = Time.time - spawntime;
        float step = 0;
        while (currentTime < delay)
        {
            currentTime = Time.time - spawntime;
            step = easeStep(currentTime / delay, easing);
            t.position = Vector3.Lerp(from, to, step);
            yield return null;
        }
        t.position = to;
    }
    public static IEnumerator MoveBounce(this Transform t, float delay, Vector3 from, Vector3 to, Easing easing = Easing.Linear)
    {
        while (true)
        {
            IEnumerator ie = t.MoveFromTo(delay, from, to, easing);
            while (ie.MoveNext())
            {
                yield return null;
            }
            IEnumerator ie2 = t.MoveFromTo(delay, to, from, easing);
            while (ie2.MoveNext())
            {
                yield return null;
            }
        }
    }

    public static IEnumerator MoveRandomly(this Transform t, float delay, Vector3 upLeft, Vector3 downRight, Easing easing = Easing.Linear)
    {
        while (true)
        {
            Vector3 v = new Vector3();
            v = v.RandomRangeXY(upLeft.x, downRight.x, downRight.y, upLeft.y);
            IEnumerator ie = t.MoveFromTo(delay, t.position, v, easing);
            while (ie.MoveNext())
            {
                yield return null;
            }
        }
    }
    #endregion

}
