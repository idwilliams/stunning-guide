using Substrate.Core;
using Xunit;

namespace Substrate.Tests;

public class CoreTests
{
    [Fact]
    public void Id_Equality_Works()
    {
        var id1 = new Id<string>(42);
        var id2 = new Id<string>(42);
        var id3 = new Id<string>(43);

        Assert.True(id1 == id2);
        Assert.False(id1 == id3);
        Assert.True(id1.Equals(id2));
        Assert.False(id1.Equals(id3));
    }

    [Fact]
    public void Id_Comparison_Works()
    {
        var id1 = new Id<string>(10);
        var id2 = new Id<string>(20);

        Assert.True(id1 < id2);
        Assert.True(id2 > id1);
        Assert.True(id1 <= id2);
        Assert.True(id2 >= id1);
    }

    [Fact]
    public void Column_Add_And_Index_Works()
    {
        using var column = new Column<int>();
        
        column.Add(10);
        column.Add(20);
        column.Add(30);

        Assert.Equal(3, column.Count);
        Assert.Equal(10, column[0]);
        Assert.Equal(20, column[1]);
        Assert.Equal(30, column[2]);
    }

    [Fact]
    public void Column_Grows_Automatically()
    {
        using var column = new Column<int>(2);
        
        for (int i = 0; i < 100; i++)
        {
            column.Add(i);
        }

        Assert.Equal(100, column.Count);
        Assert.True(column.Capacity >= 100);
    }

    [Fact]
    public void Column_AsSpan_Works()
    {
        using var column = new Column<int>();
        column.Add(1);
        column.Add(2);
        column.Add(3);

        var span = column.AsSpan();
        Assert.Equal(3, span.Length);
        Assert.Equal(1, span[0]);
        Assert.Equal(2, span[1]);
        Assert.Equal(3, span[2]);
    }

    [Fact]
    public void ColumnSet_AddRow_Synchronizes_Columns()
    {
        using var set = new ColumnSet();
        var col1 = set.AddColumn<int>("col1");
        var col2 = set.AddColumn<string>("col2");

        int row1 = set.AddRow();
        col1[row1] = 42;
        col2[row1] = "test";

        int row2 = set.AddRow();
        col1[row2] = 99;
        col2[row2] = "hello";

        Assert.Equal(2, set.RowCount);
        Assert.Equal(42, col1[row1]);
        Assert.Equal("test", col2[row1]);
        Assert.Equal(99, col1[row2]);
        Assert.Equal("hello", col2[row2]);
    }

    [Fact]
    public void IdRange_Contains_Works()
    {
        var range = new IdRange<string>(new Id<string>(10), new Id<string>(20));
        
        Assert.True(range.Contains(new Id<string>(10)));
        Assert.True(range.Contains(new Id<string>(15)));
        Assert.True(range.Contains(new Id<string>(19)));
        Assert.False(range.Contains(new Id<string>(20)));
        Assert.False(range.Contains(new Id<string>(5)));
    }
}
