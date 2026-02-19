using Substrate.Collections;
using Xunit;

namespace Substrate.Tests;

public class CollectionsTests
{
    private class SimpleCodec : IOpenAddressTableCodec<int, int>
    {
        public int GetHash(int key) => key;
        public bool KeyEquals(int a, int b) => a == b;
        public int GetKey(int entry) => entry;
        public int CreateEntry(int key) => key;
        public bool IsTombstone(int entry) => entry == -1;
        public int CreateTombstone() => -1;
    }

    [Fact]
    public void OpenAddressTable_Insert_And_Find()
    {
        var table = new OpenAddressTable<int, int, SimpleCodec>();
        
        table.TryInsert(42, out var entry1);
        Assert.Equal(42, entry1);
        Assert.Equal(1, table.Count);

        bool found = table.TryFind(42, out var entry2);
        Assert.True(found);
        Assert.Equal(42, entry2);

        found = table.TryFind(99, out _);
        Assert.False(found);
    }

    [Fact]
    public void OpenAddressTable_Handles_Duplicates()
    {
        var table = new OpenAddressTable<int, int, SimpleCodec>();
        
        bool inserted1 = table.TryInsert(42, out _);
        Assert.True(inserted1);
        Assert.Equal(1, table.Count);

        bool inserted2 = table.TryInsert(42, out _);
        Assert.False(inserted2);
        Assert.Equal(1, table.Count);
    }

    [Fact]
    public void BitSet_Set_And_Get()
    {
        var bitset = new BitSet(128);
        
        bitset.Set(10);
        bitset.Set(50);
        bitset.Set(100);

        Assert.True(bitset[10]);
        Assert.True(bitset[50]);
        Assert.True(bitset[100]);
        Assert.False(bitset[0]);
        Assert.False(bitset[25]);
        Assert.Equal(3, bitset.Count);
    }

    [Fact]
    public void BitSet_Clear_Works()
    {
        var bitset = new BitSet();
        
        bitset.Set(5);
        Assert.True(bitset[5]);
        Assert.Equal(1, bitset.Count);

        bitset.Clear(5);
        Assert.False(bitset[5]);
        Assert.Equal(0, bitset.Count);
    }

    [Fact]
    public void BitSet_Rank_Works()
    {
        var bitset = new BitSet();
        
        bitset.Set(0);
        bitset.Set(5);
        bitset.Set(10);
        bitset.Set(15);

        Assert.Equal(0, bitset.Rank(0));
        Assert.Equal(1, bitset.Rank(1));
        Assert.Equal(2, bitset.Rank(6));
        Assert.Equal(3, bitset.Rank(11));
        Assert.Equal(4, bitset.Rank(16));
    }

    [Fact]
    public void WorkList_Push_Pop()
    {
        var worklist = new WorkList<int>();
        
        worklist.Push(1);
        worklist.Push(2);
        worklist.Push(3);

        Assert.Equal(3, worklist.Count);

        Assert.True(worklist.TryPop(out int val1));
        Assert.Equal(3, val1);

        Assert.True(worklist.TryPop(out int val2));
        Assert.Equal(2, val2);

        Assert.Equal(1, worklist.Count);
    }

    [Fact]
    public void InternTable_Interns_Strings()
    {
        var table = new InternTable();
        
        var id1 = table.Intern("hello");
        var id2 = table.Intern("world");
        var id3 = table.Intern("hello");

        Assert.Equal(id1, id3);
        Assert.NotEqual(id1, id2);
        Assert.Equal(2, table.Count);
        Assert.Equal("hello", table.GetString(id1));
        Assert.Equal("world", table.GetString(id2));
    }
}
