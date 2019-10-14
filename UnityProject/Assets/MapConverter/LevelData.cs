using System.Collections.Generic;

namespace QMapToUnity
{
    public class LevelData
    {
        public QEntity WorldSpawn;
        public QEntity[] Entities;
    }

    public struct QEntity
    {
        public string Classname;
        public KeyValuePair<string, string>[] KeyValuePairs;
        public QBrush[] Brushes;
    }

    public struct QBrush
    {
        public Plane[] Planes;

        public QBrush(Plane[] lPlanes)
        {
            Planes = lPlanes;
        }
    }
}