using System.Buffers;
using System.Runtime.InteropServices;

namespace NLibSais;

// TODO: auxiliary indexes methods
// TODO: omp methods
// TODO: 64-bit methods

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
    public static unsafe void ConstructSuffixArray(ReadOnlySpan<byte> inputString, Span<int> outputSuffixArray,
        Span<int> outputFrequencyTable = default)
    {
        if (outputSuffixArray.Length < inputString.Length)
            throw new ArgumentException("Suffix array is shorter than input string.");
        
        if (outputFrequencyTable != default && outputFrequencyTable.Length < 256)
            throw new ArgumentException("Output frequency table is shorter than 256 characters.");

        int result;

        fixed (byte* inputStringPtr = inputString)
        fixed (int* outputSuffixArrayPtr = outputSuffixArray)
        fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
        {
            result = NativeMethods.libsais(inputStringPtr, outputSuffixArrayPtr, inputString.Length,
                outputSuffixArray.Length - inputString.Length, outputFrequencyTablePtr);
        }

        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");
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
    public static unsafe void ConstructSuffixArray(ReadOnlySpan<ushort> inputString, Span<int> outputSuffixArray,
        Span<int> outputFrequencyTable = default)
    {
        if (outputSuffixArray.Length < inputString.Length)
            throw new ArgumentException("Suffix array is shorter than input string.");
        
        if (outputFrequencyTable != default && outputFrequencyTable.Length < 65536)
            throw new ArgumentException("Output frequency table is shorter than 65536 characters.");

        int result;

        fixed (ushort* inputStringPtr = inputString)
        fixed (int* outputSuffixArrayPtr = outputSuffixArray)
        fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
        {
            result = NativeMethods.libsais16(inputStringPtr, outputSuffixArrayPtr, inputString.Length,
                outputSuffixArray.Length - inputString.Length, outputFrequencyTablePtr);
        }

        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");
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
    public static void ConstructSuffixArray(ReadOnlySpan<char> inputString, Span<int> outputSuffixArray,
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

        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing suffix array.");
    }

    /// <summary>
    /// Constructs the burrows-wheeler transformed string (BWT) of a given string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input string.</param>
    /// <param name="outputString">[0..n-1] The output string (can be the same as inputString).</param>
    /// <param name="outputFrequencyTable">[0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <remarks>Temporary array is rented from shared array pool.</remarks>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructBWT(ReadOnlySpan<byte> inputString, Span<byte> outputString,
        Span<int> outputFrequencyTable = default)
    {
        if (outputString.Length != inputString.Length)
            throw new ArgumentException("Input string and output string must be of the same length.");
        
        if (outputFrequencyTable != default && outputFrequencyTable.Length < 256)
            throw new ArgumentException("Output frequency table is shorter than 256 characters.");

        int result;
        var temporaryArray = ArrayPool<byte>.Shared.Rent(inputString.Length * sizeof(int));
        var temporaryArrayInt = MemoryMarshal.Cast<byte, int>(temporaryArray);

        try
        {
            fixed (byte* inputStringPtr = inputString)
            fixed (byte* outputStringPtr = outputString)
            fixed (int* temporaryArrayPtr = temporaryArrayInt)
            fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
            {
                result = NativeMethods.libsais_bwt(inputStringPtr, outputStringPtr, temporaryArrayPtr,
                    inputString.Length, temporaryArrayInt.Length - inputString.Length, outputFrequencyTablePtr);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(temporaryArray);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing BWT.");

        return result;
    }
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed 16-bit string (BWT) of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputString">[0..n-1] The output 16-bit string (can be the same as inputString).</param>
    /// <param name="outputFrequencyTable">[0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe int ConstructBWT(ReadOnlySpan<ushort> inputString, Span<ushort> outputString,
        Span<int> outputFrequencyTable = default)
    {
        if (outputString.Length != inputString.Length)
            throw new ArgumentException("Input string and output string must be of the same length.");
        
        if (outputFrequencyTable != default && outputFrequencyTable.Length < 256)
            throw new ArgumentException("Output frequency table is shorter than 256 characters.");

        int result;
        var temporaryArray = ArrayPool<byte>.Shared.Rent(inputString.Length * sizeof(int));
        var temporaryArrayInt = MemoryMarshal.Cast<byte, int>(temporaryArray);

        try
        {
            fixed (ushort* inputStringPtr = inputString)
            fixed (ushort* outputStringPtr = outputString)
            fixed (int* temporaryArrayPtr = temporaryArrayInt)
            fixed (int* outputFrequencyTablePtr = outputFrequencyTable)
            {
                result = NativeMethods.libsais16_bwt(inputStringPtr, outputStringPtr, temporaryArrayPtr,
                    inputString.Length, temporaryArrayInt.Length - inputString.Length, outputFrequencyTablePtr);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(temporaryArray);
        }

        if (result < 0)
            throw new InvalidOperationException("Error occured while constructing BWT.");

        return result;
    }
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed 16-bit string (BWT) of a given 16-bit string.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputString">[0..n-1] The output 16-bit string (can be the same as inputString).</param>
    /// <param name="outputFrequencyTable">[0..255] The output symbol frequency table (can be empty/default).</param>
    /// <returns>The primary index.</returns>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static int ConstructBWT(ReadOnlySpan<char> inputString, Span<char> outputString,
        Span<int> outputFrequencyTable = default)
        => ConstructBWT(MemoryMarshal.Cast<char, ushort>(inputString), 
            MemoryMarshal.Cast<char, ushort>(outputString), outputFrequencyTable);

    /// <summary>
    /// Constructs the original string from a given burrows-wheeler transformed string (BWT) with primary index.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input string.</param>
    /// <param name="outputString">[0..n-1] The output string (can be inputString).</param>
    /// <param name="primaryIndex">The primary index.</param>
    /// <param name="inputFrequencyTable">[0..255] The input symbol frequency table (can be empty/default).</param>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ReconstructOriginalFromBWT(ReadOnlySpan<byte> inputString, Span<byte> outputString,
        int primaryIndex, Span<int> inputFrequencyTable = default)
    {
        if (outputString.Length != inputString.Length)
            throw new ArgumentException("Input string and output string must be of the same length.");
        
        if (inputFrequencyTable != default && inputFrequencyTable.Length < 256)
            throw new ArgumentException("Input frequency table is shorter than 256 characters.");

        int result;
        var temporaryArray = ArrayPool<byte>.Shared.Rent((inputString.Length + 1) * sizeof(int));

        try
        {
            fixed (byte* inputStringPtr = inputString)
            fixed (byte* outputStringPtr = outputString)
            fixed (int* temporaryArrayPtr = MemoryMarshal.Cast<byte, int>(temporaryArray))
            fixed (int* inputFrequencyTablePtr = inputFrequencyTable)
            {
                result = NativeMethods.libsais_unbwt(inputStringPtr, outputStringPtr, temporaryArrayPtr,
                    inputString.Length, inputFrequencyTablePtr, primaryIndex);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(temporaryArray);
        }

        if (result != 0)
            throw new InvalidOperationException("Error occured while reconstructing original string from BWT.");
    }

    /// <summary>
    /// Constructs the original 16-bit string from a given burrows-wheeler transformed 16-bit string (BWT) with primary index.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputString">[0..n-1] The output 16-bit string (can be T).</param>
    /// <param name="primaryIndex">The primary index.</param>
    /// <param name="inputFrequencyTable">[0..65535] The input 16-bit symbol frequency table (can be empty/default).</param>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ReconstructOriginalFromBWT(ReadOnlySpan<ushort> inputString, Span<ushort> outputString,
        int primaryIndex, Span<int> inputFrequencyTable = default)
    {
        if (outputString.Length != inputString.Length)
            throw new ArgumentException("Input string and output string must be of the same length.");
        
        if (inputFrequencyTable != default && inputFrequencyTable.Length < 256)
            throw new ArgumentException("Input frequency table is shorter than 256 characters.");
        
        int result;
        var temporaryArray = ArrayPool<byte>.Shared.Rent((inputString.Length + 1) * sizeof(int));

        try
        {
            fixed (ushort* inputStringPtr = inputString)
            fixed (ushort* outputStringPtr = outputString)
            fixed (int* temporaryArrayPtr = MemoryMarshal.Cast<byte, int>(temporaryArray))
            fixed (int* inputFrequencyTablePtr = inputFrequencyTable)
            {
                result = NativeMethods.libsais16_unbwt(inputStringPtr, outputStringPtr, temporaryArrayPtr,
                    inputString.Length, inputFrequencyTablePtr, primaryIndex);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(temporaryArray);
        }

        if (result != 0)
            throw new InvalidOperationException("Error occured while reconstructing original string from BWT.");
    }

    /// <summary>
    /// Constructs the original 16-bit string from a given burrows-wheeler transformed 16-bit string (BWT) with primary index.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="outputString">[0..n-1] The output 16-bit string (can be T).</param>
    /// <param name="primaryIndex">The primary index.</param>
    /// <param name="inputFrequencyTable">[0..65535] The input 16-bit symbol frequency table (can be empty/default).</param>
    /// <exception cref="ArgumentException">Thrown when inputString and outputString lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static void ReconstructOriginalFromBWT(ReadOnlySpan<char> inputString, Span<char> outputString,
        int primaryIndex, Span<int> inputFrequencyTable = default)
        => ReconstructOriginalFromBWT(MemoryMarshal.Cast<char, ushort>(inputString),
            MemoryMarshal.Cast<char, ushort>(outputString), primaryIndex, inputFrequencyTable);

    /// <summary>
    /// Constructs the permuted longest common prefix array (PLCP) of a given string and a suffix array.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input string.</param>
    /// <param name="inputSuffixArray">[0..n-1] The input suffix array.</param>
    /// <param name="outputPLCP">[0..n-1] The output permuted longest common prefix array.</param>
    /// <exception cref="ArgumentException">Thrown when input lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructPLCP(ReadOnlySpan<byte> inputString, ReadOnlySpan<int> inputSuffixArray,
        Span<int> outputPLCP)
    {
        if (inputString.Length != inputSuffixArray.Length || inputString.Length != outputPLCP.Length)
            throw new ArgumentException("Input parameters must be of the same length.");

        int result;

        fixed (byte* inputStringPtr = inputString)
        fixed (int* inputSuffixArrayPtr = inputSuffixArray)
        fixed (int* outputPLCPPtr = outputPLCP)
        {
            result = NativeMethods.libsais_plcp(inputStringPtr, inputSuffixArrayPtr, outputPLCPPtr,
                inputString.Length);
        }
        
        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing PLCP.");
    }
    
    /// <summary>
    /// Constructs the permuted longest common prefix array (PLCP) of a given 16-bit string and a suffix array.
    /// </summary>
    /// <param name="inputString">[0..n-1] The input 16-bit string.</param>
    /// <param name="inputSuffixArray">[0..n-1] The input suffix array.</param>
    /// <param name="outputPLCP">[0..n-1] The output permuted longest common prefix array.</param>
    /// <exception cref="ArgumentException">Thrown when input lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructPLCP(ReadOnlySpan<ushort> inputString, ReadOnlySpan<int> inputSuffixArray,
        Span<int> outputPLCP)
    {
        if (inputString.Length != inputSuffixArray.Length || inputString.Length != outputPLCP.Length)
            throw new ArgumentException("Input parameters must be of the same length.");

        int result;

        fixed (ushort* inputStringPtr = inputString)
        fixed (int* inputSuffixArrayPtr = inputSuffixArray)
        fixed (int* outputPLCPPtr = outputPLCP)
        {
            result = NativeMethods.libsais16_plcp(inputStringPtr, inputSuffixArrayPtr, outputPLCPPtr,
                inputString.Length);
        }
        
        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing PLCP.");
    }

    /// <summary>
    /// Constructs the longest common prefix array (LCP) of a given permuted longest common prefix array (PLCP) and a suffix array.
    /// </summary>
    /// <param name="inputPLCP">[0..n-1] The input permuted longest common prefix array.</param>
    /// <param name="inputSuffixArray">[0..n-1] The input suffix array.</param>
    /// <param name="outputLCP">[0..n-1] The output longest common prefix array (can be SA).</param>
    /// <exception cref="ArgumentException">Thrown when input lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructLCPu8(ReadOnlySpan<int> inputPLCP, ReadOnlySpan<int> inputSuffixArray, Span<int> outputLCP)
    {
        if (inputPLCP.Length != inputSuffixArray.Length || inputPLCP.Length != outputLCP.Length)
            throw new ArgumentException("Input parameters must be of the same length.");

        int result;
        
        fixed (int* inputPLCPPtr = inputPLCP)
        fixed (int* inputSuffixArrayPtr = inputSuffixArray)
        fixed (int* outputLCPptr = outputLCP)
        {
            result = NativeMethods.libsais_lcp(inputPLCPPtr, inputSuffixArrayPtr, outputLCPptr, inputPLCP.Length);
        }
        
        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing LCP.");
    }
    
    /// <summary>
    /// Constructs the longest common prefix array (LCP) of a given permuted longest common prefix array (PLCP) and a suffix array.
    /// </summary>
    /// <param name="inputPLCP">[0..n-1] The input permuted longest common prefix array.</param>
    /// <param name="inputSuffixArray">[0..n-1] The input suffix array.</param>
    /// <param name="outputLCP">[0..n-1] The output longest common prefix array (can be SA).</param>
    /// <exception cref="ArgumentException">Thrown when input lengths do not match.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unexpected error occurs.</exception>
    public static unsafe void ConstructLCPu16(ReadOnlySpan<int> inputPLCP, ReadOnlySpan<int> inputSuffixArray, Span<int> outputLCP)
    {
        if (inputPLCP.Length != inputSuffixArray.Length || inputPLCP.Length != outputLCP.Length)
            throw new ArgumentException("Input parameters must be of the same length.");

        int result;
        
        fixed (int* inputPLCPPtr = inputPLCP)
        fixed (int* inputSuffixArrayPtr = inputSuffixArray)
        fixed (int* outputLCPptr = outputLCP)
        {
            result = NativeMethods.libsais16_lcp(inputPLCPPtr, inputSuffixArrayPtr, outputLCPptr, inputPLCP.Length);
        }
        
        if (result != 0)
            throw new InvalidOperationException("Error occured while constructing LCP.");
    }
}