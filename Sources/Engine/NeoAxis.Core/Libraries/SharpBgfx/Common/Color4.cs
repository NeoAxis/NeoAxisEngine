//using System;
//using System.Globalization;
//using System.Runtime.InteropServices;

//namespace Internal.SharpBgfx.Common {
//    /// <summary>
//    /// A four-component (RGBA) color value where each channel is represented by a byte.
//    /// </summary>
//    [StructLayout(LayoutKind.Sequential, Pack = 4)]
//    public struct Color4 : IEquatable<Color4>, IFormattable {
//        /// <summary>
//        /// The red component of the color.
//        /// </summary>
//        public byte Red;

//        /// <summary>
//        /// The green component of the color.
//        /// </summary>
//        public byte Green;

//        /// <summary>
//        /// The blue component of the color.
//        /// </summary>
//        public byte Blue;

//        /// <summary>
//        /// The alpha component of the color.
//        /// </summary>
//        public byte Alpha;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Color4"/> struct.
//        /// </summary>
//        /// <param name="rgba">A packed RGBA integer containing all four color components.</param>
//        public Color4 (int rgba) {
//            Red = (byte)((rgba >> 24) & 0xFF);
//            Green = (byte)((rgba >> 16) & 0xFF);
//            Blue = (byte)((rgba >> 8) & 0xFF);
//            Alpha = (byte)(rgba & 0xFF);
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Color4"/> struct.
//        /// </summary>
//        /// <param name="red">The red component of the color.</param>
//        /// <param name="green">The green component of the color.</param>
//        /// <param name="blue">The blue component of the color.</param>
//        public Color4 (int red, int green, int blue)
//            : this(red, green, blue, 0xFF) {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="Color4" /> struct.
//        /// </summary>
//        /// <param name="red">The red component of the color.</param>
//        /// <param name="green">The green component of the color.</param>
//        /// <param name="blue">The blue component of the color.</param>
//        /// <param name="alpha">The alpha component of the color.</param>
//        public Color4 (int red, int green, int blue, int alpha) {
//            Red = (byte)red;
//            Green = (byte)green;
//            Blue = (byte)blue;
//            Alpha = (byte)alpha;
//        }

//        /// <summary>
//        /// Converts the color into a packed ARGB integer.
//        /// </summary>
//        /// <returns>A packed integer containing all four color components.</returns>
//        public int ToArgb () {
//            return (int)(((uint)Alpha << 24) | ((uint)Red << 16) | ((uint)Green << 8) | Blue);
//        }

//        /// <summary>
//        /// Converts the color into a packed RGBA integer.
//        /// </summary>
//        /// <returns>A packed integer containing all four color components.</returns>
//        public int ToRgba () {
//            return (int)(((uint)Red << 24) | ((uint)Green << 16) | ((uint)Blue << 8) | Alpha);
//        }

//        /// <summary>
//        /// Adds two colors.
//        /// </summary>
//        /// <param name="left">The first color to add.</param>
//        /// <param name="right">The second color to add.</param>
//        /// <returns>The sum of the two colors.</returns>
//        public static Color4 Add (Color4 left, Color4 right) {
//            return new Color4(left.Red + right.Red, left.Green + right.Green, left.Blue + right.Blue, left.Alpha + right.Alpha);
//        }

//        /// <summary>
//        /// Subtracts two colors.
//        /// </summary>
//        /// <param name="left">The first color to subtract.</param>
//        /// <param name="right">The second color to subtract</param>
//        /// <returns>The difference of the two colors.</returns>
//        public static Color4 Subtract (Color4 left, Color4 right) {
//            return new Color4(left.Red - right.Red, left.Green - right.Green, left.Blue - right.Blue, left.Alpha - right.Alpha);
//        }

//        /// <summary>
//        /// Modulates two colors.
//        /// </summary>
//        /// <param name="left">The first color to modulate.</param>
//        /// <param name="right">The second color to modulate.</param>
//        /// <returns>The modulated color.</returns>
//        public static Color4 Modulate (Color4 left, Color4 right) {
//            return new Color4(left.Red * right.Red, left.Green * right.Green, left.Blue * right.Blue, left.Alpha * right.Alpha);
//        }

//        /// <summary>
//        /// Scales a color.
//        /// </summary>
//        /// <param name="value">The color to scale.</param>
//        /// <param name="scalar">The amount by which to scale.</param>
//        /// <returns>The scaled color.</returns>
//        public static Color4 Scale (Color4 value, float scalar) {
//            return new Color4(
//                (byte)(value.Red * scalar),
//                (byte)(value.Green * scalar),
//                (byte)(value.Blue * scalar),
//                (byte)(value.Alpha * scalar)
//            );
//        }

//        /// <summary>
//        /// Negates a color.
//        /// </summary>
//        /// <param name="value">The color to negate.</param>
//        /// <returns>The negated color.</returns>
//        public static Color4 Negate (Color4 value) {
//            return new Color4(-value.Red, -value.Green, -value.Blue, -value.Alpha);
//        }

//        /// <summary>
//        /// Inverts the color (takes the complement of the color).
//        /// </summary>
//        /// <param name="value">The color to invert.</param>
//        /// <returns>The inverted color.</returns>
//        public static Color4 Invert (Color4 value) {
//            return new Color4(-value.Red, -value.Green, -value.Blue, -value.Alpha);
//        }

//        /// <summary>
//        /// Restricts a value to be within a specified range.
//        /// </summary>
//        /// <param name="value">The value to clamp.</param>
//        /// <param name="min">The minimum value.</param>
//        /// <param name="max">The maximum value.</param>
//        /// <returns>The clamped value.</returns>
//        public static Color4 Clamp (Color4 value, Color4 min, Color4 max) {
//            return new Color4(
//                MathHelpers.Clamp(value.Red, min.Red, max.Red),
//                MathHelpers.Clamp(value.Green, min.Green, max.Green),
//                MathHelpers.Clamp(value.Blue, min.Blue, max.Blue),
//                MathHelpers.Clamp(value.Alpha, min.Alpha, max.Alpha)
//            );
//        }

//        /// <summary>
//        /// Performs a linear interpolation between two colors.
//        /// </summary>
//        /// <param name="start">Start color.</param>
//        /// <param name="end">End color.</param>
//        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
//        /// <returns>The linear interpolation of the two colors.</returns>
//        /// <remarks>
//        /// This method performs the linear interpolation based on the following formula.
//        /// <code>start + (end - start) * amount</code>
//        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
//        /// </remarks>
//        public static Color4 Lerp (Color4 start, Color4 end, float amount) {
//            return new Color4(
//                MathHelpers.Lerp(start.Red, end.Red, amount),
//                MathHelpers.Lerp(start.Green, end.Green, amount),
//                MathHelpers.Lerp(start.Blue, end.Blue, amount),
//                MathHelpers.Lerp(start.Alpha, end.Alpha, amount)
//            );
//        }

//        /// <summary>
//        /// Performs a cubic interpolation between two colors.
//        /// </summary>
//        /// <param name="start">Start color.</param>
//        /// <param name="end">End color.</param>
//        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
//        /// <returns>The cubic interpolation of the two colors.</returns>
//        public static Color4 SmoothStep (Color4 start, Color4 end, float amount) {
//            amount = MathHelpers.Clamp(amount, 0.0f, 1.0f);
//            amount = (amount * amount) * (3.0f - (2.0f * amount));

//            return Lerp(start, end, amount);
//        }

//        /// <summary>
//        /// Returns a color containing the largest components of the specified colorss.
//        /// </summary>
//        /// <param name="left">The first source color.</param>
//        /// <param name="right">The second source color.</param>
//        /// <returns>A color containing the largest components of the source colors.</returns>
//        public static Color4 Max (Color4 left, Color4 right) {
//            return new Color4(
//                (left.Red > right.Red) ? left.Red : right.Red,
//                (left.Green > right.Green) ? left.Green : right.Green,
//                (left.Blue > right.Blue) ? left.Blue : right.Blue,
//                (left.Alpha > right.Alpha) ? left.Alpha : right.Alpha
//            );
//        }

//        /// <summary>
//        /// Returns a color containing the smallest components of the specified colors.
//        /// </summary>
//        /// <param name="left">The first source color.</param>
//        /// <param name="right">The second source color.</param>
//        /// <returns>A color containing the smallest components of the source colors.</returns>
//        public static Color4 Min (Color4 left, Color4 right) {
//            return new Color4(
//                (left.Red < right.Red) ? left.Red : right.Red,
//                (left.Green < right.Green) ? left.Green : right.Green,
//                (left.Blue < right.Blue) ? left.Blue : right.Blue,
//                (left.Alpha < right.Alpha) ? left.Alpha : right.Alpha
//            );
//        }

//        /// <summary>
//        /// Adjusts the contrast of a color.
//        /// </summary>
//        /// <param name="value">The color whose contrast is to be adjusted.</param>
//        /// <param name="contrast">The amount by which to adjust the contrast.</param>
//        /// <returns>The adjusted color.</returns>
//        public static Color4 AdjustContrast (Color4 value, float contrast) {
//            return new Color4(
//                0x7F + (byte)(contrast * (value.Red - 0x7F)),
//                0x7F + (byte)(contrast * (value.Green - 0x7F)),
//                0x7F + (byte)(contrast * (value.Blue - 0x7F)),
//                value.Alpha
//            );
//        }

//        /// <summary>
//        /// Adjusts the saturation of a color.
//        /// </summary>
//        /// <param name="value">The color whose saturation is to be adjusted.</param>
//        /// <param name="saturation">The amount by which to adjust the saturation.</param>
//        /// <returns>The adjusted color.</returns>
//        public static Color4 AdjustSaturation (Color4 value, float saturation) {
//            var grey = value.Red * 0.2125f + value.Green * 0.7154f + value.Blue * 0.0721f;

//            return new Color4(
//                (byte)(grey + saturation * ((float)value.Red - grey)),
//                (byte)(grey + saturation * ((float)value.Green - grey)),
//                (byte)(grey + saturation * ((float)value.Blue - grey)),
//                value.Alpha
//            );
//        }

//        /// <summary>
//        /// Inverts the color (takes the complement of the color).
//        /// </summary>
//        /// <param name="value">The color to invert.</param>
//        /// <returns>The inverted color.</returns>
//        public static Color4 operator ~(Color4 value) {
//            return Invert(value);
//        }

//        /// <summary>
//        /// Adds two colors.
//        /// </summary>
//        /// <param name="left">The first color to add.</param>
//        /// <param name="right">The second color to add.</param>
//        /// <returns>The sum of the two colors.</returns>
//        public static Color4 operator +(Color4 left, Color4 right) {
//            return Add(left, right);
//        }

//        /// <summary>
//        /// Assert a color (return it unchanged).
//        /// </summary>
//        /// <param name="value">The color to assert (unchange).</param>
//        /// <returns>The asserted (unchanged) color.</returns>
//        public static Color4 operator +(Color4 value) {
//            return value;
//        }

//        /// <summary>
//        /// Subtracts two colors.
//        /// </summary>
//        /// <param name="left">The first color to subtract.</param>
//        /// <param name="right">The second color to subtract.</param>
//        /// <returns>The difference of the two colors.</returns>
//        public static Color4 operator -(Color4 left, Color4 right) {
//            return Subtract(left, right);
//        }

//        /// <summary>
//        /// Negates a color.
//        /// </summary>
//        /// <param name="value">The color to negate.</param>
//        /// <returns>A negated color.</returns>
//        public static Color4 operator -(Color4 value) {
//            return Negate(value);
//        }

//        /// <summary>
//        /// Tests for equality between two objects.
//        /// </summary>
//        /// <param name="left">The first value to compare.</param>
//        /// <param name="right">The second value to compare.</param>
//        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
//        public static bool operator ==(Color4 left, Color4 right) {
//            return left.Equals(right);
//        }

//        /// <summary>
//        /// Tests for inequality between two objects.
//        /// </summary>
//        /// <param name="left">The first value to compare.</param>
//        /// <param name="right">The second value to compare.</param>
//        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
//        public static bool operator !=(Color4 left, Color4 right) {
//            return !left.Equals(right);
//        }

//        /// <summary>
//        /// Performs an explicit conversion from <see cref="System.Int32"/> to <see cref="Color4"/>.
//        /// </summary>
//        /// <param name="value">The value.</param>
//        /// <returns>The result of the conversion.</returns>
//        public static implicit operator Color4 (int value) {
//            return new Color4(value);
//        }

//        /// <summary>
//        /// Performs an implicit conversion from <see cref="Color4"/> to <see cref="System.Int32"/>.
//        /// </summary>
//        /// <param name="value">The value.</param>
//        /// <returns>
//        /// The result of the conversion.
//        /// </returns>
//        public static implicit operator int (Color4 value) {
//            return value.ToArgb();
//        }

//        /// <summary>
//        /// Returns a <see cref="System.String"/> that represents this instance.
//        /// </summary>
//        /// <returns>
//        /// A <see cref="System.String"/> that represents this instance.
//        /// </returns>
//        public override string ToString () {
//            return ToString(CultureInfo.CurrentCulture);
//        }

//        /// <summary>
//        /// Returns a <see cref="System.String"/> that represents this instance.
//        /// </summary>
//        /// <param name="formatProvider">The format provider.</param>
//        /// <returns>
//        /// A <see cref="System.String"/> that represents this instance.
//        /// </returns>
//        public string ToString (IFormatProvider formatProvider) {
//            return string.Format(formatProvider, "Red:{0} Green:{1} Blue:{2} Alpha:{3}", Red, Green, Blue, Alpha);
//        }

//        /// <summary>
//        /// Returns a <see cref="System.String"/> that represents this instance.
//        /// </summary>
//        /// <param name="format">The format.</param>
//        /// <returns>
//        /// A <see cref="System.String"/> that represents this instance.
//        /// </returns>
//        public string ToString (string format) {
//            return ToString(format, CultureInfo.CurrentCulture);
//        }

//        /// <summary>
//        /// Returns a <see cref="System.String"/> that represents this instance.
//        /// </summary>
//        /// <param name="format">The format.</param>
//        /// <param name="formatProvider">The format provider.</param>
//        /// <returns>
//        /// A <see cref="System.String"/> that represents this instance.
//        /// </returns>
//        public string ToString (string format, IFormatProvider formatProvider) {
//            if (format == null)
//                return ToString(formatProvider);

//            return string.Format(formatProvider, "Red:{0} Green:{1} Blue:{2} Alpha:{3}",
//                Red.ToString(format, formatProvider),
//                Green.ToString(format, formatProvider),
//                Blue.ToString(format, formatProvider),
//                Alpha.ToString(format, formatProvider)
//            );
//        }

//        /// <summary>
//        /// Returns a hash code for this instance.
//        /// </summary>
//        /// <returns>
//        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
//        /// </returns>
//        public override int GetHashCode () {
//            return Alpha.GetHashCode() + Red.GetHashCode() + Green.GetHashCode() + Blue.GetHashCode();
//        }

//        /// <summary>
//        /// Determines whether the specified <see cref="Color4"/> is equal to this instance.
//        /// </summary>
//        /// <param name="other">The <see cref="Color4"/> to compare with this instance.</param>
//        /// <returns>
//        /// <c>true</c> if the specified <see cref="Color4"/> is equal to this instance; otherwise, <c>false</c>.
//        /// </returns>
//        public bool Equals (Color4 other) {
//            return
//                Red == other.Red &&
//                Green == other.Green &&
//                Blue == other.Blue &&
//                Alpha == other.Alpha;
//        }

//        /// <summary>
//        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
//        /// </summary>
//        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
//        /// <returns>
//        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
//        /// </returns>
//        public override bool Equals (object obj) {
//            var color = obj as Color4?;
//            if (color == null)
//                return false;

//            return Equals(color);
//        }
//    }
//}
