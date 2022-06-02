namespace VinSearcher.KtdReader
{
    public class BlockSize
    {
        internal readonly long Start;
        internal readonly long End;

        public BlockSize(long start, long end)
        {
            Start = start;
            End = end;
        }
    }
}
