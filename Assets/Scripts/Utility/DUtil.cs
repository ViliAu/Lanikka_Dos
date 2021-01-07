public static class DUtil {

    /// <summary>
    /// Checks if two values are close enough of each other
    /// </summary>
    /// <param name="a">Value 1</param>
    /// <param name="b">Value 2</param>
    /// <param name="threshold">The threshold of how close the values should be</param>
    /// <returns>Returns true if the floats are threshold away of each other</returns>
    public static bool Approx(float a, float b, float threshold) {
        return a - b < 0 ? b - a <= threshold : a - b <= threshold;
    }

}
