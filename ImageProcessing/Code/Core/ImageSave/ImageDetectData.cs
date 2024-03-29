﻿
namespace ImageProcessing.Core {
    public class ImageDetectData {
        public ImageDetectData() { }

        public string className = "A";
        public string imgName;

        public int nodesBranchesCount = 0;
        public int nodesEndCount = 0;

        public int zondRedIntersectCount = 0;
        public int zondBlueIntersectCount = 0;

        [System.NonSerialized]
        public float distance_NkNy;
        [System.NonSerialized]
        public float distance_Zonds;
    }
}
