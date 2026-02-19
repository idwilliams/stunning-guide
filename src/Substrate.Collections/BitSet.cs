using System.Numerics;

namespace Substrate.Collections;

/// <summary>
/// Dense bitset with rank/select operations.
/// </summary>
public sealed class BitSet
{
    private ulong[] _bits;
    private int _count;

    public BitSet(int capacity = 64)
    {
        int wordCount = (capacity + 63) / 64;
        _bits = new ulong[wordCount];
        _count = 0;
    }

    public int Count => _count;

    public bool this[int index]
    {
        get
        {
            int word = index / 64;
            int bit = index % 64;
            return word < _bits.Length && (_bits[word] & (1UL << bit)) != 0;
        }
        set
        {
            EnsureCapacity(index + 1);
            int word = index / 64;
            int bit = index % 64;
            
            if (value)
            {
                if ((_bits[word] & (1UL << bit)) == 0)
                {
                    _bits[word] |= 1UL << bit;
                    _count++;
                }
            }
            else
            {
                if ((_bits[word] & (1UL << bit)) != 0)
                {
                    _bits[word] &= ~(1UL << bit);
                    _count--;
                }
            }
        }
    }

    public void Set(int index)
    {
        this[index] = true;
    }

    public void Clear(int index)
    {
        this[index] = false;
    }

    public int Rank(int index)
    {
        int count = 0;
        int fullWords = index / 64;
        
        for (int i = 0; i < fullWords; i++)
        {
            count += BitOperations.PopCount(_bits[i]);
        }
        
        int remainingBits = index % 64;
        if (remainingBits > 0)
        {
            ulong mask = (1UL << remainingBits) - 1;
            count += BitOperations.PopCount(_bits[fullWords] & mask);
        }
        
        return count;
    }

    private void EnsureCapacity(int bitCount)
    {
        int requiredWords = (bitCount + 63) / 64;
        if (requiredWords > _bits.Length)
        {
            Array.Resize(ref _bits, Math.Max(_bits.Length * 2, requiredWords));
        }
    }
}
