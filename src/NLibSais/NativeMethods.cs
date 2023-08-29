using System.Runtime.InteropServices;

namespace NLibSais;

internal static class NativeMethods
{
    /// <summary>
    /// Constructs the suffix array of a given string.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="SA">[0..n-1+fs] The output array of suffixes.</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="fs">The extra space available at the end of SA array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..255] The output symbol frequency table (can be NULL).</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais", ExactSpelling = true)]
    public static extern unsafe int libsais(byte* T, int* SA, int n, int fs, int* freq);
    
    /// <summary>
    /// Constructs the suffix array of a given integer array.
    /// Note, during construction input array will be modified, but restored at the end if no errors occurred.
    /// </summary>
    /// <param name="T">[0..n-1] The input integer array.</param>
    /// <param name="SA">[0..n-1+fs] The output array of suffixes.</param>
    /// <param name="n">The length of the integer array.</param>
    /// <param name="k">The alphabet size of the input integer array.</param>
    /// <param name="fs">Extra space available at the end of SA array (can be 0, but 4k or better 6k is recommended for optimal performance).</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_int", ExactSpelling = true)]
    public static extern unsafe int libsais_int(int* T, int* SA, int n, int k, int fs);
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed string (BWT) of a given string.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n-1+fs] The temporary array.</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="fs">The extra space available at the end of A array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..255] The output symbol frequency table (can be NULL).</param>
    /// <returns>The primary index if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_bwt", ExactSpelling = true)]
    public static extern unsafe int libsais_bwt(byte* T, byte* U, int* A, int n, int fs, int* freq);
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed string (BWT) of a given string with auxiliary indexes.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n-1+fs] The temporary array.</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="fs">The extra space available at the end of A array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..255] The output symbol frequency table (can be NULL).</param>
    /// <param name="r">The sampling rate for auxiliary indexes (must be power of 2).</param>
    /// <param name="I">[0..(n-1)/r] The output auxiliary indexes.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_bwt_aux", ExactSpelling = true)]
    public static extern unsafe int libsais_bwt_aux(byte* T, byte* U, int* A, int n, int fs, int* freq, int r, int* I);
    
    /// <summary>
    /// Constructs the original string from a given burrows-wheeler transformed string (BWT) with primary index.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n] The temporary array (NOTE, temporary array must be n + 1 size).</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="freq">[0..255] The input symbol frequency table (can be NULL).</param>
    /// <param name="i">The primary index.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_unbwt", ExactSpelling = true)]
    public static extern unsafe int libsais_unbwt(byte* T, byte* U, int* A, int n, int* freq, int i);
    
    /// <summary>
    /// Constructs the original string from a given burrows-wheeler transformed string (BWT) with auxiliary indexes.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n] The temporary array (NOTE, temporary array must be n + 1 size).</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="freq">[0..255] The input symbol frequency table (can be NULL).</param>
    /// <param name="r">The sampling rate for auxiliary indexes (must be power of 2).</param>
    /// <param name="I">[0..(n-1)/r] The input auxiliary indexes.</param>
    /// <returns></returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_unbwt_aux", ExactSpelling = true)]
    public static extern unsafe int libsais_unbwt_aux(byte* T, byte* U, int* A, int n, int* freq, int r, int* I);
    
    /// <summary>
    /// Constructs the permuted longest common prefix array (PLCP) of a given string and a suffix array.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="SA">[0..n-1] The input suffix array.</param>
    /// <param name="PLCP">[0..n-1] The output permuted longest common prefix array.</param>
    /// <param name="n">The length of the string and the suffix array.</param>
    /// <returns>0 if no error occurred, -1 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_plcp", ExactSpelling = true)]
    public static extern unsafe int libsais_plcp(byte* T, int* SA, int* PLCP, int n);
    
    /// <summary>
    /// Constructs the longest common prefix array (LCP) of a given permuted longest common prefix array (PLCP) and a suffix array.
    /// </summary>
    /// <param name="PLCP">[0..n-1] The input permuted longest common prefix array.</param>
    /// <param name="SA">[0..n-1] The input suffix array.</param>
    /// <param name="LCP">[0..n-1] The output longest common prefix array (can be SA).</param>
    /// <param name="n">The length of the permuted longest common prefix array and the suffix array.</param>
    /// <returns>0 if no error occurred, -1 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais_lcp", ExactSpelling = true)]
    public static extern unsafe int libsais_lcp(int* PLCP, int* SA, int* LCP, int n);
    
    /// <summary>
    /// Constructs the suffix array of a given 16-bit string.
    /// </summary>
    /// <param name="T">[0..n-1] The input 16-bit string.</param>
    /// <param name="SA">[0..n-1+fs] The output array of suffixes.</param>
    /// <param name="n">The length of the given 16-bit string.</param>
    /// <param name="fs">The extra space available at the end of SA array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..65535] The output 16-bit symbol frequency table (can be NULL).</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16", ExactSpelling = true)]
    public static extern unsafe int libsais16(ushort* T, int* SA, int n, int fs, int* freq);
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed 16-bit string (BWT) of a given 16-bit string.
    /// </summary>
    /// <param name="T">[0..n-1] The input 16-bit string.</param>
    /// <param name="U">[0..n-1] The output 16-bit string (can be T).</param>
    /// <param name="A">[0..n-1+fs] The temporary array.</param>
    /// <param name="n">The length of the given 16-bit string.</param>
    /// <param name="fs">The extra space available at the end of A array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..65535] The output 16-bit symbol frequency table (can be NULL).</param>
    /// <returns>The primary index if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16_bwt", ExactSpelling = true)]
    public static extern unsafe int libsais16_bwt(ushort* T, ushort* U, int* A, int n, int fs, int* freq);
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed 16-bit string (BWT) of a given 16-bit string with auxiliary indexes.
    /// </summary>
    /// <param name="T">[0..n-1] The input 16-bit string.</param>
    /// <param name="U">[0..n-1] The output 16-bit string (can be T).</param>
    /// <param name="A">[0..n-1+fs] The temporary array.</param>
    /// <param name="n">The length of the given 16-bit string.</param>
    /// <param name="fs">The extra space available at the end of A array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..65535] The output 16-bit symbol frequency table (can be NULL).</param>
    /// <param name="r">The sampling rate for auxiliary indexes (must be power of 2).</param>
    /// <param name="I">[0..(n-1)/r] The output auxiliary indexes.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16_bwt_aux", ExactSpelling = true)]
    public static extern unsafe int libsais16_bwt_aux(ushort* T, ushort* U, int* A, int n, int fs, int* freq, int r, int* I);
    
    /// <summary>
    /// Constructs the original 16-bit string from a given burrows-wheeler transformed 16-bit string (BWT) with primary index.
    /// </summary>
    /// <param name="T">[0..n-1] The input 16-bit string.</param>
    /// <param name="U">[0..n-1] The output 16-bit string (can be T).</param>
    /// <param name="A">[0..n] The temporary array (NOTE, temporary array must be n + 1 size).</param>
    /// <param name="n">The length of the given 16-bit string.</param>
    /// <param name="freq">[0..65535] The input 16-bit symbol frequency table (can be NULL).</param>
    /// <param name="i">The primary index.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16_unbwt", ExactSpelling = true)]
    public static extern unsafe int libsais16_unbwt(ushort* T, ushort* U, int* A, int n, int* freq, int i);
    
    /// <summary>
    /// Constructs the original 16-bit string from a given burrows-wheeler transformed 16-bit string (BWT) with auxiliary indexes.
    /// </summary>
    /// <param name="T">[0..n-1] The input 16-bit string.</param>
    /// <param name="U">[0..n-1] The output 16-bit string (can be T).</param>
    /// <param name="A">[0..n] The temporary array (NOTE, temporary array must be n + 1 size).</param>
    /// <param name="n">The length of the given 16-bit string.</param>
    /// <param name="freq">[0..65535] The input 16-bit symbol frequency table (can be NULL).</param>
    /// <param name="r">The sampling rate for auxiliary indexes (must be power of 2).</param>
    /// <param name="I">[0..(n-1)/r] The input auxiliary indexes.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16_unbwt_aux", ExactSpelling = true)]
    public static extern unsafe int libsais16_unbwt_aux(ushort* T, ushort* U, int* A, int n, int* freq, int r, int* I);
    
    /// <summary>
    /// Constructs the permuted longest common prefix array (PLCP) of a given 16-bit string and a suffix array.
    /// </summary>
    /// <param name="T">[0..n-1] The input 16-bit string.</param>
    /// <param name="SA">[0..n-1] The input suffix array.</param>
    /// <param name="PLCP">[0..n-1] The output permuted longest common prefix array.</param>
    /// <param name="n">The length of the 16-bit string and the suffix array.</param>
    /// <returns>0 if no error occurred, -1 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16_plcp", ExactSpelling = true)]
    public static extern unsafe int libsais16_plcp(ushort* T, int* SA, int* PLCP, int n);
    
    /// <summary>
    /// Constructs the longest common prefix array (LCP) of a given permuted longest common prefix array (PLCP) and a suffix array.
    /// </summary>
    /// <param name="PCLP">[0..n-1] The input permuted longest common prefix array.</param>
    /// <param name="SA">[0..n-1] The input suffix array.</param>
    /// <param name="LCP">[0..n-1] The output longest common prefix array (can be SA).</param>
    /// <param name="n">The length of the permuted longest common prefix array and the suffix array.</param>
    /// <returns>0 if no error occurred, -1 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais16_lcp", ExactSpelling = true)]
    public static extern unsafe int libsais16_lcp(int* PCLP, int* SA, int* LCP, int n);

    /// <summary>
    /// Constructs the suffix array of a given string.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="SA">[0..n-1+fs] The output array of suffixes.</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="fs">The extra space available at the end of SA array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..255] The output symbol frequency table (can be NULL).</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64", ExactSpelling = true)]
    public static extern unsafe long libsais64(byte* T, long* SA, long n, long fs, long* freq);
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed string (BWT) of a given string.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n-1+fs] The temporary array.</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="fs">The extra space available at the end of A array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..255] The output symbol frequency table (can be NULL).</param>
    /// <returns>The primary index if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64_bwt", ExactSpelling = true)]
    public static extern unsafe long libsais64_bwt(byte* T, byte* U, long* A, long n, long fs, long* freq);
    
    /// <summary>
    /// Constructs the burrows-wheeler transformed string (BWT) of a given string with auxiliary indexes.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n-1+fs] The temporary array.</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="fs">The extra space available at the end of A array (0 should be enough for most cases).</param>
    /// <param name="freq">[0..255] The output symbol frequency table (can be NULL).</param>
    /// <param name="r">The sampling rate for auxiliary indexes (must be power of 2).</param>
    /// <param name="I">[0..(n-1)/r] The output auxiliary indexes.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64_bwt_aux", ExactSpelling = true)]
    public static extern unsafe long libsais64_bwt_aux(byte* T, byte* U, long* A, long n, long fs, long* freq, long r, long* I);
    
    /// <summary>
    /// Constructs the original string from a given burrows-wheeler transformed string (BWT) with primary index.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n] The temporary array (NOTE, temporary array must be n + 1 size).</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="freq">[0..255] The input symbol frequency table (can be NULL).</param>
    /// <param name="i">The primary index.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64_unbwt", ExactSpelling = true)]
    public static extern unsafe long libsais64_unbwt(byte* T, byte* U, long* A, long n, long* freq, long i);
    
    /// <summary>
    /// Constructs the original string from a given burrows-wheeler transformed string (BWT) with auxiliary indexes.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="U">[0..n-1] The output string (can be T).</param>
    /// <param name="A">[0..n] The temporary array (NOTE, temporary array must be n + 1 size).</param>
    /// <param name="n">The length of the given string.</param>
    /// <param name="freq">[0..255] The input symbol frequency table (can be NULL).</param>
    /// <param name="r">The sampling rate for auxiliary indexes (must be power of 2)</param>
    /// <param name="I">[0..(n-1)/r] The input auxiliary indexes.</param>
    /// <returns>0 if no error occurred, -1 or -2 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64_unbwt_aux", ExactSpelling = true)]
    public static extern unsafe long libsais64_unbwt_aux(byte* T, byte* U, long* A, long n, long* freq, long r, long* I);
    
    /// <summary>
    /// Constructs the permuted longest common prefix array (PLCP) of a given string and a suffix array.
    /// </summary>
    /// <param name="T">[0..n-1] The input string.</param>
    /// <param name="SA">[0..n-1] The input suffix array.</param>
    /// <param name="PLCP">[0..n-1] The output permuted longest common prefix array.</param>
    /// <param name="n">The length of the string and the suffix array.</param>
    /// <returns>0 if no error occurred, -1 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64_plcp", ExactSpelling = true)]
    public static extern unsafe long libsais64_plcp(byte* T, long* SA, long* PLCP, long n);
    
    /// <summary>
    /// Constructs the longest common prefix array (LCP) of a given permuted longest common prefix array (PLCP) and a suffix array.
    /// </summary>
    /// <param name="PLCP">[0..n-1] The input permuted longest common prefix array.</param>
    /// <param name="SA">[0..n-1] The input suffix array.</param>
    /// <param name="LCP">[0..n-1] The output longest common prefix array (can be SA).</param>
    /// <param name="n">The length of the permuted longest common prefix array and the suffix array.</param>
    /// <returns>0 if no error occurred, -1 otherwise.</returns>
    [DllImport("libsais.dll", EntryPoint = "libsais64_lcp", ExactSpelling = true)]
    public static extern unsafe long libsais64_lcp(long* PLCP, long* SA, long* LCP, long n);
}