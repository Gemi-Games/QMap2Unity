using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QMapToUnity
{
    public static class MapParser
    {
        private enum ParseState
        {
            Body,
            Entity,
            Brush,
        }

        public const string CLASSNAME = "classname";
        public const string WORLDSPAWN = "worldspawn";

        private const float MAP_SCALE = 1f;

        private static ParseState s_State;

        private static LevelData s_LevelData;

        public static LevelData ParseMapToLevelData(TextAsset lMapFile)
        {
            s_State = ParseState.Body;

            s_LevelData = new LevelData();

            List<QEntity> lEntities = new List<QEntity>();

            {
                List<KeyValuePair<string, string>> lKeyValuePairs = new List<KeyValuePair<string, string>>();
                List<QBrush> lBrushes = new List<QBrush>();

                List<Plane> lPlanes = new List<Plane>();

                QEntity lEnt = new QEntity();
                QBrush lBrush = new QBrush();

                string[] lLines = lMapFile.text.Split('\n');

                for (int i = 0; i < lLines.Length; i++)
                {
                    string lLine = lLines[i];
                    string[] lWords = lLine.Split(' ');

                    if (s_State == ParseState.Body)                 /// Body Update
                    {
                        for (int j = 0; j < lWords.Length; j++)
                        {
                            string lWord = lWords[j];

                            if (lWord.Contains('{'))                /// Open new Entity
                            {
                                s_State = ParseState.Entity;
                                break;
                            }
                        }
                    }
                    else if (s_State == ParseState.Entity)          /// Entity Update
                    {
                        for (int j = 0; j < lWords.Length; j++)
                        {
                            string lWord = lWords[j];

                            if (lWord.Contains('{'))                /// Open New Brush
                            {
                                s_State = ParseState.Brush;
                                break;
                            }
                            else if (lWord.Contains('}'))           /// Add and Close Entity
                            {
                                lEnt.Brushes = lBrushes.ToArray();
                                lBrushes.Clear();

                                lEnt.KeyValuePairs = lKeyValuePairs.ToArray();
                                lKeyValuePairs.Clear();

                                for (int k = 0; k < lEnt.KeyValuePairs.Length; k++)
                                {
                                    if (lEnt.KeyValuePairs[k].Key == CLASSNAME)
                                    {
                                        lEnt.Classname = lEnt.KeyValuePairs[k].Value;
                                        break;
                                    }
                                }

                                if (lEnt.Classname.Length > 0)
                                    lEntities.Add(lEnt);
                                else
                                    Debug.LogError(lEnt.ToString() + " doesn't have a classname!");

                                lEnt = new QEntity();

                                s_State = ParseState.Body;

                                break;
                            }
                            else if (j < lWords.Length - 1)             /// Add Key-Value pair
                            {
                                string lKey = "";
                                string lValue = "";

                                int lState = 0;
                                for (int k = 0; k < lLine.Length; k++)
                                {
                                    char lChar = lLine[k];

                                    if (lState == 0 && lChar == '\"')
                                    {
                                        lState = 1;
                                    }
                                    else if (lState == 1)
                                    {
                                        if (lChar == '\"')
                                        {
                                            lState = 2;
                                        }
                                        else
                                        {
                                            lKey = String.Concat(lKey, lChar);
                                        }
                                    }
                                    else if (lState == 2 && lChar == '\"')
                                    {
                                        lState = 3;
                                    }
                                    else if (lState == 3)
                                    {
                                        if (lChar == '\"')
                                        {
                                            lState = 4;
                                            break;
                                        }
                                        else
                                        {
                                            lValue = String.Concat(lValue, lChar);
                                        }
                                    }
                                }

                                if (lKey.Length > 0 && lValue.Length > 0)
                                {
                                    bool lAddKeyValuePair = true;

                                    for (int k = 0; k < lKeyValuePairs.Count; k++)
                                    {
                                        if (lKeyValuePairs[k].Key == lKey)
                                        {
                                            lAddKeyValuePair = false;
                                            break;
                                        }
                                    }

                                    if (lAddKeyValuePair)
                                        lKeyValuePairs.Add(new KeyValuePair<string, string>(lKey, lValue));
                                }
                            }
                        }
                    }
                    else if (s_State == ParseState.Brush)                                               // Brush Update
                    {
                        for (int j = 0; j < lWords.Length; j++)
                        {
                            string lWord = lWords[j];

                            if (lWord.Contains('}'))                        /// Add and Close Brush
                            {
                                if (lPlanes.Count > 3)
                                {
                                    lBrush.Planes = lPlanes.ToArray();
                                    lPlanes.Clear();

                                    lBrushes.Add(lBrush);

                                    lBrush = new QBrush();
                                }

                                s_State = ParseState.Entity;

                                break;
                            }
                            else if (ContainsPlaneData(lWords, j))          /// Add Plane
                            {
                                float PX, PY, PZ, X1, Y1, Z1, X2, Y2, Z2;

                                PX = float.Parse(lWords[j + 1]);
                                PY = float.Parse(lWords[j + 2]);
                                PZ = float.Parse(lWords[j + 3]);

                                X1 = float.Parse(lWords[j + 6]);
                                Y1 = float.Parse(lWords[j + 7]);
                                Z1 = float.Parse(lWords[j + 8]);

                                X2 = float.Parse(lWords[j + 11]);
                                Y2 = float.Parse(lWords[j + 12]);
                                Z2 = float.Parse(lWords[j + 13]);

                                Vector3 lPos = new Vector3(PX, PY, PZ);
                                Vector3 l1 = new Vector3(X1, Y1, Z1);
                                Vector3 l2 = new Vector3(X2, Y2, Z2);

                                l1 = (l1 - lPos).normalized;
                                l2 = (l2 - lPos).normalized;

                                float lTempY = lPos.y;
                                lPos.y = lPos.z;
                                lPos.z = lTempY;

                                Vector3 lNormal = Vector3.Cross(l1, l2);

                                lTempY = lNormal.y;
                                lNormal.y = lNormal.z;
                                lNormal.z = lTempY;

                                lNormal.Normalize();

                                Plane lNewPlane = new Plane(lNormal, Vector3.Dot(lNormal, lPos * MAP_SCALE));

                                float OX, OY, SX, SY, A;

                                OX = float.Parse(lWords[j + 16]);
                                OY = float.Parse(lWords[j + 17]);
                                A = float.Parse(lWords[j + 18]);
                                SX = float.Parse(lWords[j + 19]);
                                string lSYWord = lWords[j + 20];
                                lSYWord = lSYWord.Split('\r')[0];
                                SY = float.Parse(lSYWord);

                                Vector2 lTexOffset = new Vector2(OX, OY);
                                Vector2 lTexScale = new Vector2(SX, SY);
                                float lTexAngle = A;

                                lNewPlane.TextureData = new TextureData(lWords[j + 15]).SetValues(lTexOffset, lTexScale, lTexAngle);
                                lPlanes.Add(lNewPlane);
                            }
                        }
                    }
                }
            }

            s_LevelData.Entities = lEntities.ToArray();

            for (int i = 0; i < s_LevelData.Entities.Length; i++)
            {
                QEntity lEnt = s_LevelData.Entities[i];
                bool lBreak = false;

                for (int k = 0; k < lEnt.KeyValuePairs.Length; k++)
                {
                    if (lEnt.KeyValuePairs[k].Key == CLASSNAME && lEnt.KeyValuePairs[k].Value == WORLDSPAWN)
                    {
                        s_LevelData.WorldSpawn = lEnt;
                        lBreak = true;
                        break;
                    }
                }

                if (lBreak)
                    break;
            }

            return s_LevelData;
        }

        private static bool ContainsPlaneData(string[] lWords, int j)
        {
            return j < lWords.Length - 15 &&
                        lWords[j] == "(" &&
                        lWords[j + 4] == ")" &&
                        lWords[j + 5] == "(" &&
                        lWords[j + 9] == ")" &&
                        lWords[j + 10] == "(" &&
                        lWords[j + 14] == ")";
        }
    }
}