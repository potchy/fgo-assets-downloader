namespace FgoAssetsDownloader
{
    public class Servant
    {
        public int CollectionNo { get; set; }
        public string Name { get; set; }
        public ServantExtraAssets ExtraAssets { get; set; }

        public class ServantExtraAssets
        {
            public ServantExtraAssetsCharaGraph CharaGraph { get; set; }

            public class ServantExtraAssetsCharaGraph
            {
                public Dictionary<int, string> Ascension { get; set; }
                public Dictionary<int, string> Costume { get; set; }
            }
        }
    }
}
