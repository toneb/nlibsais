using System.Runtime.InteropServices;

namespace NLibSais;

/// <summary>
/// libsais is a library for linear time suffix array, longest common prefix array and burrows wheeler transform construction based on induced sorting algorithm.
/// </summary>
public static class LibSais
{
    /// <summary>
    /// Constructs the suffix array of a given string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input string.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which could be used to improve performance.</param>
    /// <param name="outputFrequencyTable">[0..255] The output symbol frequency table (can be empty/default).</param>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructSuffixArray(ReadOnlySpan<byte> inputString, Span<int> outputSuffixArray,
        Span<int> outputFrequencyTable = default)
    {
        if (outputSuffixArray.Length < inputString.Length)
            throw new ArgumentException("Suffix array is shorter than input string.");

        int result;

        fixed (byte* inputStringPtr = inputString)
        fixed (int* outputSuffixArrayPtr = outputSuffixArray)
        fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
        {
            result = NativeMethods.libsais(inputStringPtr, outputSuffixArrayPtr, inputString.Length,
                outputSuffixArray.Length - inputString.Length, outputFrequencyTablePtr);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");
    }
    
    /// <summary>
    /// Constructs the suffix array of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which could be used to improve performance.</param>
    /// <param name="outputFrequencyTable">[0..65535] The output 16-bit symbol frequency table (can be NULL).</param>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructSuffixArray(ReadOnlySpan<ushort> inputString, Span<int> outputSuffixArray,
        Span<int> outputFrequencyTable = default)
    {
        if (outputSuffixArray.Length < inputString.Length)
            throw new ArgumentException("Suffix array is shorter than input string.");

        int result;

        fixed (ushort* inputStringPtr = inputString)
        fixed (int* outputSuffixArrayPtr = outputSuffixArray)
        fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
        {
            result = NativeMethods.libsais16(inputStringPtr, outputSuffixArrayPtr, inputString.Length,
                outputSuffixArray.Length - inputString.Length, outputFrequencyTablePtr);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");
    }

    /// <summary>
    /// Constructs the suffix array of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which could be used to improve performance.</param>
    /// <param name="outputFrequencyTable">[0..65535] The output 16-bit symbol frequency table (can be NULL).</param>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static void ConstructSuffixArray(ReadOnlySpan<char> inputString, Span<int> outputSuffixArray,
        Span<int> outputFrequencyTable = default)
        => ConstructSuffixArray(MemoryMarshal.Cast<char, ushort>(inputString), outputSuffixArray,
            outputFrequencyTable);
    
    /// <summary>
    /// Constructs the suffix array of a given integer array.
    /// Note, during construction input array will be modified, but restored at the end if no errors occurred.
    /// </summary>
    /// <param name="inputArray">[0..n-1] The input integer array.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which could be used to improve performance.</param>
    /// <param name="alphabetSize">The alphabet size of the input integer array.</param>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputArray.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructSuffixArray(Span<int> inputArray, Span<int> outputSuffixArray,
        int alphabetSize)
    {
        if (outputSuffixArray.Length < inputArray.Length)
            throw new ArgumentException("Suffix array is shorter than input array.");

        int result;

        fixed (int* inputArrayPtr = inputArray)
        fixed (int* outputSuffixArrayPtr = outputSuffixArray)
        {
            result = NativeMethods.libsais_int(inputArrayPtr, outputSuffixArrayPtr, inputArray.Length,
                alphabetSize, outputSuffixArray.Length - inputArray.Length);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");
    }
}