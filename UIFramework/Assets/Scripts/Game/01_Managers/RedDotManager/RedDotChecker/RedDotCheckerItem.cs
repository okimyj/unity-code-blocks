
namespace YJClient
{
    public class RedDotCheckerItem : RedDotCheckerBase
    {
        public RedDotCheckerItem() : base(EContentsType.Item) { }
        public override void Refresh()
        {
            // Refresh RedDot.
            HasRedDot = true;
        }
    }

}
