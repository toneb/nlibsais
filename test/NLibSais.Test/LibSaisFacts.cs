using System.Runtime.InteropServices;

namespace NLibSais.Test;

public class LibSaisFacts
{
    [Fact]
    public void Int8SuffixArrayBwtAndUnbwtAreCompatible()
    {
        var testData = new byte[1024 * 1024];
        new Random().NextBytes(testData);
        
        // SA without frequency table
        var SA1 = new int[testData.Length];
        LibSais.ConstructSuffixArray(testData, SA1);
        
        // SA with frequency table
        var SA2 = new int[testData.Length];
        var freq1 = new int[256];
        LibSais.ConstructSuffixArray(testData, SA2, freq1);
        
        // BWT without frequency table
        var BWT1 = new byte[testData.Length];
        var bwt1Result = LibSais.ConstructBWT(testData, BWT1);
        
        // BWT with frequency table
        var BWT2 = new byte[testData.Length];
        var BWT2Freq = new int[256];
        var bwt2Result = LibSais.ConstructBWT(testData, BWT2, BWT2Freq);
        
        // Reconstruct BWT without frequency table
        var UNBWT1 = new byte[testData.Length];
        LibSais.ReconstructOriginalFromBWT(BWT2, UNBWT1, bwt2Result);

        // Reconstruct BWT with frequency table
        var UNBWT2 = new byte[testData.Length];
        LibSais.ReconstructOriginalFromBWT(BWT2, UNBWT2, bwt2Result, BWT2Freq);
        
        // Suffix arrays equal
        Assert.Equal(SA1, SA2);
        
        // BWTs are equal
        Assert.Equal(BWT1, BWT2);
        
        // UNBWT result is equal to original data
        Assert.Equal(testData, UNBWT1);
        Assert.Equal(testData, UNBWT2);
        
        // primary indexes are equal
        Assert.Equal(bwt1Result, bwt2Result);
        
        // frequency tables equal
        Assert.Equal(freq1, BWT2Freq);
    }
    
    [Fact]
    public void Int16SuffixArrayBwtAndUnbwtAreCompatible()
    {
        var tempData = new byte[1024 * 1024 * sizeof(ushort)];
        new Random().NextBytes(tempData);
        var testData = MemoryMarshal.Cast<byte, ushort>(tempData).ToArray();
        
        // SA without frequency table
        var SA1 = new int[testData.Length];
        LibSais.ConstructSuffixArray(testData, SA1);
        
        // SA with frequency table
        var SA2 = new int[testData.Length];
        var freq1 = new int[65535];
        LibSais.ConstructSuffixArray(testData, SA2, freq1);
        
        // BWT without frequency table
        var BWT1 = new ushort[testData.Length];
        var bwt1Result = LibSais.ConstructBWT(testData, BWT1);
        
        // BWT with frequency table
        var BWT2 = new ushort[testData.Length];
        var BWT2Freq = new int[65535];
        var bwt2Result = LibSais.ConstructBWT(testData, BWT2, BWT2Freq);
        
        // Reconstruct BWT without frequency table
        var UNBWT1 = new ushort[testData.Length];
        LibSais.ReconstructOriginalFromBWT(BWT2, UNBWT1, bwt2Result);

        // Reconstruct BWT with frequency table
        var UNBWT2 = new ushort[testData.Length];
        LibSais.ReconstructOriginalFromBWT(BWT2, UNBWT2, bwt2Result, BWT2Freq);
        
        // Suffix arrays equal
        Assert.Equal(SA1, SA2);
        
        // BWTs are equal
        Assert.Equal(BWT1, BWT2);
        
        // UNBWT result is equal to original data
        Assert.Equal(testData, UNBWT1);
        Assert.Equal(testData, UNBWT2);
        
        // primary indexes are equal
        Assert.Equal(bwt1Result, bwt2Result);
        
        // frequency tables equal
        Assert.Equal(freq1, BWT2Freq);
    }
}