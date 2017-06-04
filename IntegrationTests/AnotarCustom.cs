[assembly: Anotar.Custom.LoggerFactory(typeof(LoggerFactory))]

namespace IntegrationTests
{
    public class AnotarCustom
    {
        public void Weave()
        {
            Anotar.Custom.LogTo.Debug("This weaver should work: {0}", "Custom");
        }
    }
}


