using System;

namespace SharpBgfx {
    /// <summary>
    /// Specifies state information used to configure rendering operations.
    /// </summary>
    public struct RenderState : IEquatable<RenderState> {
        const int AlphaRefShift = 40;
        const int PointSizeShift = 52;
        const ulong AlphaRefMask = 0x0000ff0000000000;
        const ulong PointSizeMask = 0x0ff0000000000000;

        readonly ulong value;

        /// <summary>
        /// No state bits set.
        /// </summary>
        public static readonly RenderState None = 0;

        /// <summary>
        /// Enable writing the Red color channel to the framebuffer.
        /// </summary>
        public static readonly RenderState WriteR = 0x0000000000000001;

        /// <summary>
        /// Enable writing the Green color channel to the framebuffer.
        /// </summary>
        public static readonly RenderState WriteG = 0x0000000000000002;

        /// <summary>
        /// Enable writing the Blue color channel to the framebuffer.
        /// </summary>
        public static readonly RenderState WriteB = 0x0000000000000004;

        /// <summary>
        /// Enable writing alpha data to the framebuffer.
        /// </summary>
        public static readonly RenderState WriteA = 0x0000000000000008;

        /// <summary>
        /// Enable writing to the depth buffer.
        /// </summary>
        public static readonly RenderState WriteZ = 0x0000004000000000;

        /// <summary>
        /// Enable writing all three color channels to the framebuffer.
        /// </summary>
        public static readonly RenderState WriteRGB = WriteR | WriteG | WriteB;

        /// <summary>
        /// Use a "less than" comparison to pass the depth test.
        /// </summary>
        public static readonly RenderState DepthTestLess = 0x0000000000000010;

        /// <summary>
        /// Use a "less than or equal to" comparison to pass the depth test.
        /// </summary>
        public static readonly RenderState DepthTestLessEqual = 0x0000000000000020;

        /// <summary>
        /// Pass the depth test if both values are equal.
        /// </summary>
        public static readonly RenderState DepthTestEqual = 0x0000000000000030;

        /// <summary>
        /// Use a "greater than or equal to" comparison to pass the depth test.
        /// </summary>
        public static readonly RenderState DepthTestGreaterEqual = 0x0000000000000040;

        /// <summary>
        /// Use a "greater than" comparison to pass the depth test.
        /// </summary>
        public static readonly RenderState DepthTestGreater = 0x0000000000000050;

        /// <summary>
        /// Pass the depth test if both values are not equal.
        /// </summary>
        public static readonly RenderState DepthTestNotEqual = 0x0000000000000060;

        /// <summary>
        /// Never pass the depth test.
        /// </summary>
        public static readonly RenderState DepthTestNever = 0x0000000000000070;

        /// <summary>
        /// Always pass the depth test.
        /// </summary>
        public static readonly RenderState DepthTestAlways = 0x0000000000000080;

        /// <summary>
        /// Use a value of 0 as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendZero = 0x0000000000001000;

        /// <summary>
        /// Use a value of 1 as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendOne = 0x0000000000002000;

        /// <summary>
        /// Use the source pixel color as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendSourceColor = 0x0000000000003000;

        /// <summary>
        /// Use one minus the source pixel color as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendInverseSourceColor = 0x0000000000004000;

        /// <summary>
        /// Use the source pixel alpha as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendSourceAlpha = 0x0000000000005000;

        /// <summary>
        /// Use one minus the source pixel alpha as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendInverseSourceAlpha = 0x0000000000006000;

        /// <summary>
        /// Use the destination pixel alpha as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendDestinationAlpha = 0x0000000000007000;

        /// <summary>
        /// Use one minus the destination pixel alpha as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendInverseDestinationAlpha = 0x0000000000008000;

        /// <summary>
        /// Use the destination pixel color as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendDestinationColor = 0x0000000000009000;

        /// <summary>
        /// Use one minus the destination pixel color as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendInverseDestinationColor = 0x000000000000a000;

        /// <summary>
        /// Use the source pixel alpha (saturated) as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendSourceAlphaSaturate = 0x000000000000b000;

        /// <summary>
        /// Use an application supplied blending factor as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendFactor = 0x000000000000c000;

        /// <summary>
        /// Use one minus an application supplied blending factor as an input to a blend equation.
        /// </summary>
        public static readonly RenderState BlendInverseFactor = 0x000000000000d000;

        /// <summary>
        /// Blend equation: A + B
        /// </summary>
        public static readonly RenderState BlendEquationAdd = 0x0000000000000000;

        /// <summary>
        /// Blend equation: B - A
        /// </summary>
        public static readonly RenderState BlendEquationSub = 0x0000000010000000;

        /// <summary>
        /// Blend equation: A - B
        /// </summary>
        public static readonly RenderState BlendEquationReverseSub = 0x0000000020000000;

        /// <summary>
        /// Blend equation: min(a, b)
        /// </summary>
        public static readonly RenderState BlendEquationMin = 0x0000000030000000;

        /// <summary>
        /// Blend equation: max(a, b)
        /// </summary>
        public static readonly RenderState BlendEquationMax = 0x0000000040000000;

        /// <summary>
        /// Enable independent blending of simultaenous render targets.
        /// </summary>
        public static readonly RenderState BlendIndependent = 0x0000000400000000;

        /// <summary>
        /// Enable alpha to coverage blending.
        /// </summary>
        public static readonly RenderState BlendAlphaToCoverage = 0x0000000800000000;

        /// <summary>
        /// Don't perform culling of back faces.
        /// </summary>
        public static readonly RenderState NoCulling = 0x0000000000000000;

        /// <summary>
        /// Perform culling of clockwise faces.
        /// </summary>
        public static readonly RenderState CullClockwise = 0x0000001000000000;

        /// <summary>
        /// Perform culling of counter-clockwise faces.
        /// </summary>
        public static readonly RenderState CullCounterclockwise = 0x0000002000000000;

        /// <summary>
        /// Primitive topology: triangle list.
        /// </summary>
        public static readonly RenderState PrimitiveTriangles = 0x0000000000000000;

        /// <summary>
        /// Primitive topology: triangle strip.
        /// </summary>
        public static readonly RenderState PrimitiveTriangleStrip = 0x0001000000000000;

        /// <summary>
        /// Primitive topology: line list.
        /// </summary>
        public static readonly RenderState PrimitiveLines = 0x0002000000000000;

        /// <summary>
        /// Primitive topology: line strip.
        /// </summary>
        public static readonly RenderState PrimitiveLineStrip = 0x0003000000000000;

        /// <summary>
        /// Primitive topology: point list.
        /// </summary>
        public static readonly RenderState PrimitivePoints = 0x0004000000000000;

        /// <summary>
        /// Enable multisampling.
        /// </summary>
        public static readonly RenderState Multisampling = 0x1000000000000000;

        /// <summary>
        /// Enable line antialiasing.
        /// </summary>
        public static readonly RenderState LineAA = 0x2000000000000000;

        /// <summary>
        /// Enable conservative rasterization.
        /// </summary>
        public static readonly RenderState ConservativeRasterization = 0x4000000000000000;

        /// <summary>
        /// Provides a set of sane defaults.
        /// </summary>
        public static readonly RenderState Default =
            WriteRGB |
            WriteA |
            WriteZ |
            DepthTestLess |
            CullClockwise |
            Multisampling;

        /// <summary>
        /// Predefined blend effect: additive blending.
        /// </summary>
        public static readonly RenderState BlendAdd = BlendFunction(BlendOne, BlendOne);

        /// <summary>
        /// Predefined blend effect: alpha blending.
        /// </summary>
        public static readonly RenderState BlendAlpha = BlendFunction(BlendSourceAlpha, BlendInverseSourceAlpha);

        /// <summary>
        /// Predefined blend effect: "darken" blending.
        /// </summary>
        public static readonly RenderState BlendDarken = BlendFunction(BlendOne, BlendOne) | BlendEquation(BlendEquationMin);

        /// <summary>
        /// Predefined blend effect: "lighten" blending.
        /// </summary>
        public static readonly RenderState BlendLighten = BlendFunction(BlendOne, BlendOne) | BlendEquation(BlendEquationMax);

        /// <summary>
        /// Predefined blend effect: multiplicative blending.
        /// </summary>
        public static readonly RenderState BlendMultiply = BlendFunction(BlendDestinationColor, BlendZero);

        /// <summary>
        /// Predefined blend effect: normal blending based on alpha.
        /// </summary>
        public static readonly RenderState BlendNormal = BlendFunction(BlendOne, BlendInverseSourceAlpha);

        /// <summary>
        /// Predefined blend effect: "screen" blending.
        /// </summary>
        public static readonly RenderState BlendScreen = BlendFunction(BlendOne, BlendInverseSourceColor);

        /// <summary>
        /// Predefined blend effect: "linear burn" blending.
        /// </summary>
        public static readonly RenderState BlendLinearBurn = BlendFunction(BlendDestinationColor, BlendInverseDestinationColor) | BlendEquation(BlendEquationSub);

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderState"/> struct.
        /// </summary>
        /// <param name="value">The integer value of the state.</param>
        public RenderState (long value) {
            this.value = (ulong)value;
        }

        /// <summary>
        /// Encodes an alpha reference value in a render state.
        /// </summary>
        /// <param name="alpha">The alpha reference value.</param>
        /// <returns>The encoded render state.</returns>
        public static RenderState AlphaRef (byte alpha) {
            return (((ulong)alpha) << AlphaRefShift) & AlphaRefMask;
        }

        /// <summary>
        /// Encodes a point size value in a render state.
        /// </summary>
        /// <param name="size">The point size.</param>
        /// <returns>The encoded render state.</returns>
        public static RenderState PointSize (byte size) {
            return (((ulong)size) << PointSizeShift) & PointSizeMask;
        }

        /// <summary>
        /// Builds a render state for a blend function.
        /// </summary>
        /// <param name="source">The source blend operation.</param>
        /// <param name="destination">The destination blend operation.</param>
        /// <returns>The render state for the blend function.</returns>
        public static RenderState BlendFunction (RenderState source, RenderState destination) {
            return BlendFunction(source, destination, source, destination);
        }

        /// <summary>
        /// Builds a render state for a blend function.
        /// </summary>
        /// <param name="sourceColor">The source color blend operation.</param>
        /// <param name="destinationColor">The destination color blend operation.</param>
        /// <param name="sourceAlpha">The source alpha blend operation.</param>
        /// <param name="destinationAlpha">The destination alpha blend operation.</param>
        /// <returns>
        /// The render state for the blend function.
        /// </returns>
        public static RenderState BlendFunction (RenderState sourceColor, RenderState destinationColor, RenderState sourceAlpha, RenderState destinationAlpha) {
            return (sourceColor | (destinationColor << 4)) | ((sourceAlpha | (destinationAlpha << 4)) << 8);
        }

        /// <summary>
        /// Builds a render state for a blend equation.
        /// </summary>
        /// <param name="equation">The equation.</param>
        /// <returns>
        /// The render state for the blend equation.
        /// </returns>
        public static RenderState BlendEquation (RenderState equation) {
            return BlendEquation(equation, equation);
        }

        /// <summary>
        /// Builds a render state for a blend equation.
        /// </summary>
        /// <param name="sourceEquation">The source equation.</param>
        /// <param name="alphaEquation">The alpha equation.</param>
        /// <returns>
        /// The render state for the blend equation.
        /// </returns>
        public static RenderState BlendEquation (RenderState sourceEquation, RenderState alphaEquation) {
            return sourceEquation | (alphaEquation << 3);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode () {
            return value.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specific value is equal to this instance.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns><c>true</c> if the value is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals (RenderState other) {
            return value == other.value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj) {
            var state = obj as RenderState?;
            if (state == null)
                return false;

            return Equals(state);
        }

        /// <summary>
        /// Implements the equality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(RenderState left, RenderState right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the inequality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(RenderState left, RenderState right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Performs an implicit conversion from ulong.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator RenderState (ulong value) {
            return new RenderState((long)value);
        }

        /// <summary>
        /// Performs an explicit conversion to ulong.
        /// </summary>
        /// <param name="state">The value to convert.</param>
        public static explicit operator ulong (RenderState state) {
            return state.value;
        }

        /// <summary>
        /// Implements the bitwise-or operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static RenderState operator |(RenderState left, RenderState right) {
            return left.value | right.value;
        }

        /// <summary>
        /// Implements the bitwise-and operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static RenderState operator &(RenderState left, RenderState right) {
            return left.value & right.value;
        }

        /// <summary>
        /// Implements the bitwise-complement operator.
        /// </summary>
        /// <param name="state">The operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static RenderState operator ~(RenderState state) {
            return ~state.value;
        }

        /// <summary>
        /// Implements the left shift operator.
        /// </summary>
        /// <param name="state">The value to shift.</param>
        /// <param name="amount">The amount to shift.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static RenderState operator <<(RenderState state, int amount) {
            return state.value << amount;
        }

        /// <summary>
        /// Implements the right shift operator.
        /// </summary>
        /// <param name="state">The value to shift.</param>
        /// <param name="amount">The amount to shift.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static RenderState operator >>(RenderState state, int amount) {
            return state.value >> amount;
        }
    }
}
