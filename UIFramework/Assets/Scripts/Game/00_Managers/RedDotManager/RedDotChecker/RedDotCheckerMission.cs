
namespace YJClient
{
    public class RedDotCheckerMission : RedDotCheckerBase
    {
        public RedDotCheckerMission() : base(EContentsType.Mission) { }
        public override void Refresh()
        {
            // 콘텐츠 별 레드닷 체크 로직.
            HasRedDot = true;
        }
    }

}
