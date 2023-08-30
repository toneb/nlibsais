using System.Runtime.InteropServices;

namespace NLibSais;

// TODO: validate that all SA methods return the same index as BWT
// TODO: validate that temporary array for BWT is actually SA at the end
// TODO: what are auxiliary indexes?
// TODO: validate return index of SA methods

/// <summary>
/// libsais is a library for linear time suffix array, longest common prefix array and burrows wheeler transform construction based on induced sorting algorithm.
/// </summary>
public static class LibSais
{
    /// <summary>
    /// Constructs the suffix array of a given string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input string.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="outputFrequencyTable">[0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructSuffixArray(ReadOnlySpan<byte> inputString, Span<int> outputSuffixArray,
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

        return result;
    }
    
    /// <summary>
    /// Constructs the suffix array of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="outputFrequencyTable">[0..65535] The output 16-bit symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructSuffixArray(ReadOnlySpan<ushort> inputString, Span<int> outputSuffixArray,
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

        return result;
    }

    /// <summary>
    /// Constructs the suffix array of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="outputFrequencyTable">[0..65535] The output 16-bit symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static int ConstructSuffixArray(ReadOnlySpan<char> inputString, Span<int> outputSuffixArray,
        Span<int> outputFrequencyTable = default)
        => ConstructSuffixArray(MemoryMarshal.Cast<char, ushort>(inputString), outputSuffixArray,
            outputFrequencyTable);
    
    /// <summary>
    /// Constructs the suffix array of a given integer array.
    /// Note, during construction input array will be modified, but restored at the end if no errors occurred.
    /// </summary>
    /// <param name="inputArray">[0..n-1] The input integer array.</param>
    /// <param name="outputSuffixArray">[0..n-1+fs] The output array of suffixes. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="alphabetSize">The alphabet size of the input integer array.</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when outputSuffixArray is shorter than inputArray.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructSuffixArray(Span<int> inputArray, Span<int> outputSuffixArray,
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

        return result;
    }

    /// <summary>
    /// Constructs the burrows-wheeler transformed string (BWT) of a given string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input string.</param>
    /// <param name="outputString">[0..n-1] The output string (can be the same as inputString).</param>
    /// <param name="temporaryArray">[0..n-1+fs] The temporary array. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="outputFrequencyTable">0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match, or when temporaryArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructBWT(ReadOnlySpan<byte> inputString, Span<byte> outputString,
        Span<int> temporaryArray, Span<int> outputFrequencyTable = default)
    {
        if (outputString.Length != inputString.Length)
            throw new ArgumentException("Input string and output string must be of the same length.");
        
        if (temporaryArray.Length < inputString.Length)
            throw new ArgumentException("Temporary array is shorter than input array.");

        int result;
        
        fixed (byte* inputStringPtr = inputString)
        fixed (byte* outputStringPtr = outputString)
        fixed (int* temporaryArrayPtr = temporaryArray)
        fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
        {
            result = NativeMethods.libsais_bwt(inputStringPtr, outputStringPtr, temporaryArrayPtr,
                inputString.Length, temporaryArray.Length - inputString.Length, outputFrequencyTablePtr);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");

        return result;
    }
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed 16-bit string (BWT) of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputString">[0..n-1] The output 16-bit string (can be the same as inputString).</param>
    /// <param name="temporaryArray">[0..n-1+fs] The temporary array. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="outputFrequencyTable">0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match, or when temporaryArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructBWT(ReadOnlySpan<ushort> inputString, Span<ushort> outputString,
        Span<int> temporaryArray, Span<int> outputFrequencyTable = default)
    {
        if (outputString.Length != inputString.Length)
            throw new ArgumentException("Input string and output string must be of the same length.");
        
        if (temporaryArray.Length < inputString.Length)
            throw new ArgumentException("Temporary array is shorter than input array.");

        int result;
        
        fixed (ushort* inputStringPtr = inputString)
        fixed (ushort* outputStringPtr = outputString)
        fixed (int* temporaryArrayPtr = temporaryArray)
        fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
        {
            result = NativeMethods.libsais16_bwt(inputStringPtr, outputStringPtr, temporaryArrayPtr,
                inputString.Length, temporaryArray.Length - inputString.Length, outputFrequencyTablePtr);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");

        return result;
    }
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed 16-bit string (BWT) of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputString">[0..n-1] The output 16-bit string (can be the same as inputString).</param>
    /// <param name="temporaryArray">[0..n-1+fs] The temporary array. Can optionally include extra space at the end which might be used to improve performance.</param>
    /// <param name="outputFrequencyTable">0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match, or when temporaryArray is shorter than inputString.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructBWT(ReadOnlySpan<char> inputString, Span<char> outputString,
        Span<int> temporaryArray, Span<int> outputFrequencyTable = default)
        => ConstructBWT(MemoryMarshal.Cast<char, ushort>(inputString), 
            MemoryMarshal.Cast<char, ushort>(outputString), temporaryArray, outputFrequencyTable);

    
}