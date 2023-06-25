namespace FgoAssetsDownloader
{
    public class CraftEssence
    {
        public int CollectionNo { get; set; }
        public string Name { get; set; }
        public CraftEssenceExtraAssets ExtraAssets { get; set; }

        public class CraftEssenceExtraAssets
        {
            public CraftEssenceExtraAssetsCharaGraph CharaGraph { get; set; }

            public class CraftEssenceExtraAssetsCharaGraph
            {
                public Dictionary<int, string> Equip { get; set; }
            }
        }
    }
}
