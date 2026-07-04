using sicoain.api.Services;
using sicoain.shared.Interfaces;
using FluentAssertions;

namespace sicoain.UnitTests.Services;

public class IsMainHelperTests
{
    private class TestItem : IHasIsMain
    {
        public bool IsMain { get; set; }
    }

    private static List<TestItem> CreateItems(params bool[] isMainValues)
        => isMainValues.Select(v => new TestItem { IsMain = v }).ToList();

    // 0 items → no-op, no exception
    [Fact]
    public void EnsureSingleMain_WithEmptyList_DoesNothing()
    {
        var items = new List<TestItem>();

        IsMainHelper.EnsureSingleMain(items);

        items.Should().BeEmpty();
    }

    // 1 item → IsMain becomes true
    [Fact]
    public void EnsureSingleMain_WithSingleItem_SetsIsMainTrue()
    {
        var items = CreateItems(false);

        IsMainHelper.EnsureSingleMain(items);

        items[0].IsMain.Should().BeTrue();
    }

    // 2+ items, none marked → first item becomes IsMain, rest stay false
    [Fact]
    public void EnsureSingleMain_WithMultipleItemsNoneMarked_SetsFirstAsMain()
    {
        var items = CreateItems(false, false);

        IsMainHelper.EnsureSingleMain(items);

        items[0].IsMain.Should().BeTrue();
        items[1].IsMain.Should().BeFalse();
    }

    // 2+ items, one marked → stays as is
    [Fact]
    public void EnsureSingleMain_WithMultipleItemsOneMarked_KeepsItAsMain()
    {
        var items = CreateItems(false, true, false);

        IsMainHelper.EnsureSingleMain(items);

        items[0].IsMain.Should().BeFalse();
        items[1].IsMain.Should().BeTrue();
        items[2].IsMain.Should().BeFalse();
    }

    // 2+ items, multiple marked → first marked keeps true, all after it become false
    [Fact]
    public void EnsureSingleMain_WithMultipleItemsMultipleMarked_KeepsFirstMarkedOnly()
    {
        var items = CreateItems(false, true, true, false);

        IsMainHelper.EnsureSingleMain(items);

        items[0].IsMain.Should().BeFalse();
        items[1].IsMain.Should().BeTrue();
        items[2].IsMain.Should().BeFalse();
        items[3].IsMain.Should().BeFalse();
    }

    // 2+ items, all marked → first keeps true, rest become false
    [Fact]
    public void EnsureSingleMain_WithAllMarked_KeepsFirstOnly()
    {
        var items = CreateItems(true, true, true);

        IsMainHelper.EnsureSingleMain(items);

        items[0].IsMain.Should().BeTrue();
        items[1].IsMain.Should().BeFalse();
        items[2].IsMain.Should().BeFalse();
    }

    // Single-element list already marked → stays true
    [Fact]
    public void EnsureSingleMain_WithSingleItemAlreadyMarked_StaysTrue()
    {
        var items = CreateItems(true);

        IsMainHelper.EnsureSingleMain(items);

        items[0].IsMain.Should().BeTrue();
    }
}
