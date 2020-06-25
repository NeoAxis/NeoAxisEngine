using System;

namespace SharpBgfx {
    /// <summary>
    /// Specifies state information used to configure rendering operations.
    /// </summary>
    public struct StencilFlags : IEquatable<StencilFlags> {
        const int ReadMaskShift = 8;
        const uint RefMask = 0x000000ff;
        const uint ReadMaskMask = 0x0000ff00;

        readonly uint value;

        /// <summary>
        /// No state bits set.
        /// </summary>
        public static readonly StencilFlags None = 0;

        /// <summary>
        /// Perform a "less than" stencil test.
        /// </summary>
        public static readonly StencilFlags TestLess = 0x00010000;

        /// <summary>
        /// Perform a "less than or equal" stencil test.
        /// </summary>
        public static readonly StencilFlags TestLessEqual = 0x00020000;

        /// <summary>
        /// Perform an equality stencil test.
        /// </summary>
        public static readonly StencilFlags TestEqual = 0x00030000;

        /// <summary>
        /// Perform a "greater than or equal" stencil test.
        /// </summary>
        public static readonly StencilFlags TestGreaterEqual = 0x00040000;

        /// <summary>
        /// Perform a "greater than" stencil test.
        /// </summary>
        public static readonly StencilFlags TestGreater = 0x00050000;

        /// <summary>
        /// Perform an inequality stencil test.
        /// </summary>
        public static readonly StencilFlags TestNotEqual = 0x00060000;

        /// <summary>
        /// Never pass the stencil test.
        /// </summary>
        public static readonly StencilFlags TestNever = 0x00070000;

        /// <summary>
        /// Always pass the stencil test.
        /// </summary>
        public static readonly StencilFlags TestAlways = 0x00080000;

        /// <summary>
        /// On failing the stencil test, zero out the stencil value.
        /// </summary>
        public static readonly StencilFlags FailSZero = 0x00000000;

        /// <summary>
        /// On failing the stencil test, keep the old stencil value.
        /// </summary>
        public static readonly StencilFlags FailSKeep = 0x00100000;

        /// <summary>
        /// On failing the stencil test, replace the stencil value.
        /// </summary>
        public static readonly StencilFlags FailSReplace = 0x00200000;

        /// <summary>
        /// On failing the stencil test, increment the stencil value.
        /// </summary>
        public static readonly StencilFlags FailSIncrement = 0x00300000;

        /// <summary>
        /// On failing the stencil test, increment the stencil value (with saturation).
        /// </summary>
        public static readonly StencilFlags FailSIncrementSaturate = 0x00400000;

        /// <summary>
        /// On failing the stencil test, decrement the stencil value.
        /// </summary>
        public static readonly StencilFlags FailSDecrement = 0x00500000;

        /// <summary>
        /// On failing the stencil test, decrement the stencil value (with saturation).
        /// </summary>
        public static readonly StencilFlags FailSDecrementSaturate = 0x00600000;

        /// <summary>
        /// On failing the stencil test, invert the stencil value.
        /// </summary>
        public static readonly StencilFlags FailSInvert = 0x00700000;

        /// <summary>
        /// On failing the stencil test, zero out the depth value.
        /// </summary>
        public static readonly StencilFlags FailZZero = 0x00000000;

        /// <summary>
        /// On failing the stencil test, keep the depth value.
        /// </summary>
        public static readonly StencilFlags FailZKeep = 0x01000000;

        /// <summary>
        /// On failing the stencil test, replace the depth value.
        /// </summary>
        public static readonly StencilFlags FailZReplace = 0x02000000;

        /// <summary>
        /// On failing the stencil test, increment the depth value.
        /// </summary>
        public static readonly StencilFlags FailZIncrement = 0x03000000;

        /// <summary>
        /// On failing the stencil test, increment the depth value (with saturation).
        /// </summary>
        public static readonly StencilFlags FailZIncrementSaturate = 0x04000000;

        /// <summary>
        /// On failing the stencil test, decrement the depth value.
        /// </summary>
        public static readonly StencilFlags FailZDecrement = 0x05000000;

        /// <summary>
        /// On failing the stencil test, decrement the depth value (with saturation).
        /// </summary>
        public static readonly StencilFlags FailZDecrementSaturate = 0x06000000;

        /// <summary>
        /// On failing the stencil test, invert the depth value.
        /// </summary>
        public static readonly StencilFlags FailZInvert = 0x07000000;

        /// <summary>
        /// On passing the stencil test, zero out the depth value.
        /// </summary>
        public static readonly StencilFlags PassZZero = 0x00000000;

        /// <summary>
        /// On passing the stencil test, keep the old depth value.
        /// </summary>
        public static readonly StencilFlags PassZKeep = 0x10000000;

        /// <summary>
        /// On passing the stencil test, replace the depth value.
        /// </summary>
        public static readonly StencilFlags PassZReplace = 0x20000000;

        /// <summary>
        /// On passing the stencil test, increment the depth value.
        /// </summary>
        public static readonly StencilFlags PassZIncrement = 0x30000000;

        /// <summary>
        /// On passing the stencil test, increment the depth value (with saturation).
        /// </summary>
        public static readonly StencilFlags PassZIncrementSaturate = 0x40000000;

        /// <summary>
        /// On passing the stencil test, decrement the depth value.
        /// </summary>
        public static readonly StencilFlags PassZDecrement = 0x50000000;

        /// <summary>
        /// On passing the stencil test, decrement the depth value (with saturation).
        /// </summary>
        public static readonly StencilFlags PassZDecrementSaturate = 0x60000000;

        /// <summary>
        /// On passing the stencil test, invert the depth value.
        /// </summary>
        public static readonly StencilFlags PassZInvert = 0x70000000;

        /// <summary>
        /// Initializes a new instance of the <see cref="StencilFlags"/> struct.
        /// </summary>
        /// <param name="value">The integer value of the state.</param>
        public StencilFlags (int value) {
            this.value = (uint)value;
        }

        /// <summary>
        /// Encodes a reference value in a stencil state.
        /// </summary>
        /// <param name="reference">The stencil reference value.</param>
        /// <returns>The encoded stencil state.</returns>
        public static StencilFlags ReferenceValue (byte reference) {
            return reference & RefMask;
        }

        /// <summary>
        /// Encodes a read mask in a stencil state.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns>
        /// The encoded stencil state.
        /// </returns>
        public static StencilFlags ReadMask (byte mask) {
            return (((uint)mask) << ReadMaskShift) & ReadMaskMask;
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
        public bool Equals (StencilFlags other) {
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
            var state = obj as StencilFlags?;
            if (state == null)
                return false;

            return Equals(state.Value);
        }

        /// <summary>
        /// Implements the equality operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>
        /// <c>true</c> if the two objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(StencilFlags left, StencilFlags right) {
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
        public static bool operator !=(StencilFlags left, StencilFlags right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Performs an implicit conversion from uint.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator StencilFlags (uint value) {
            return new StencilFlags((int)value);
        }

        /// <summary>
        /// Performs an explicit conversion to uint.
        /// </summary>
        /// <param name="state">The value to convert.</param>
        public static explicit operator uint (StencilFlags state) {
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
        public static StencilFlags operator |(StencilFlags left, StencilFlags right) {
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
        public static StencilFlags operator &(StencilFlags left, StencilFlags right) {
            return left.value & right.value;
        }

        /// <summary>
        /// Implements the bitwise-complement operator.
        /// </summary>
        /// <param name="state">The operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static StencilFlags operator ~(StencilFlags state) {
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
        public static StencilFlags operator <<(StencilFlags state, int amount) {
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
        public static StencilFlags operator >>(StencilFlags state, int amount) {
            return state.value >> amount;
        }
    }
}