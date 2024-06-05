using openPER.ViewModels;

namespace openPERTests
{
    static class openPERAssertions
    {
        public static void PartsListMatch(IEnumerable<string> visibleParts, IEnumerable<string> invisibleParts, List<PartViewModel> parts)
        {
            var totalParts = visibleParts.Count() + invisibleParts.Count();
            foreach (var part in parts)
            {
                if (part.Visible && visibleParts.Count(x => x == part.PartNumber) == 0)
                    Assert.Fail($"Part {part.PartNumber} unexpectedly in visible list");
                if (!part.Visible && visibleParts.Count(x => x == part.PartNumber) != 0)
                    Assert.Fail($"Part {part.PartNumber} unexpectedly in invisible list");
            }
            foreach (var part in visibleParts)
            {
                if (parts.Count(x => x.PartNumber == part && x.Visible) == 0)
                    Assert.Fail($"Visible part {part} not found in results");
            }
            foreach (var part in invisibleParts)
            {
                if (parts.Count(x => x.PartNumber == part && !x.Visible) == 0)
                    Assert.Fail($"Invisible part {part} not found in results");
            }
        }
        public static void PartsListMatch(List<(string, bool)> expectedParts, List<PartViewModel> parts)
        {
            Assert.AreEqual(expectedParts.Count(), parts.Count());
            var mismatches = "";
            for (int i = 0; i < expectedParts.Count(); i++)
            {
                if (expectedParts[i].Item1 != parts[i].PartNumber)
                    mismatches += $"Mismatch in part number at slot {i}, expected {expectedParts[i].Item1} got {parts[i].PartNumber}\r\n";
                if (expectedParts[i].Item2 != parts[i].Visible)
                    mismatches += $"Mismatch in visibility at slot {i} for part mumber {expectedParts[i].Item1}, expected {expectedParts[i].Item2} got {parts[i].Visible}\r\n";
            }
            if (mismatches != "")
                Assert.Fail(mismatches);
        }
    }
}