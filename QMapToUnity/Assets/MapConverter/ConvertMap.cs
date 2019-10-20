using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace QMapToUnity
{
    public static class ConvertMap
    {
        private static ConvertMapSettings s_Settings;

        private static string s_AssetPath;

        private static Dictionary<string, Material> s_MaterialDic;

        private static int s_BaseColourMapID;
        private static int s_EmissionMapID;
        private static int s_EmissionColorID;
        private static int s_EmissionColorLDRID;
        private static int s_CutoffEnabledID;
        private static int s_CutoffAlphaID;

        public static void ConvertMapToUnityObjects(ConvertMapSettings lSettings)
        {
            s_Settings = lSettings;

            EditorGUI.BeginChangeCheck();

            if (s_Settings.UsingHDRPMaterials)
            {
                s_BaseColourMapID = Shader.PropertyToID("_BaseColorMap");
                s_EmissionMapID = Shader.PropertyToID("_EmissiveColorMap");
                s_EmissionColorID = Shader.PropertyToID("_EmissiveColor");
                s_EmissionColorLDRID = Shader.PropertyToID("_EmissiveColor");
                s_CutoffEnabledID = Shader.PropertyToID("_EmissiveColor");
                s_CutoffAlphaID = Shader.PropertyToID("_AlphaCutoff");
            }
            else
            {
                s_BaseColourMapID = Shader.PropertyToID("_MainTex");
                s_EmissionMapID = Shader.PropertyToID("_EmissionMap");
                s_EmissionColorID = Shader.PropertyToID("_EmissionColor");
                s_CutoffEnabledID = Shader.PropertyToID("_EmissiveColor");
                s_CutoffAlphaID = Shader.PropertyToID("_Cutoff");
            }

            s_MaterialDic = new Dictionary<string, Material>(s_Settings.TexDefs.Textures.Length);

            QMapLevel[] lCurrentLevels = GameObject.FindObjectsOfType<QMapLevel>();

            for (int i = 0; i < lCurrentLevels.Length; i++)
                GameObject.DestroyImmediate(lCurrentLevels[i].gameObject);

            GameObject lLevelObject = new GameObject(s_Settings.MapFile.name);
            QMapLevel lQMap = lLevelObject.AddComponent<QMapLevel>();
            lQMap.EntDefs = s_Settings.EntDefs;

            lLevelObject.isStatic = true;

            LevelData lLevelData = MapParser.ParseMapToLevelData(s_Settings.MapFile);

            int lBrushCount = 0;
            int lTotalBrushes = 0;

            for (int i = 0; i < lLevelData.Entities.Length; i++)
                lTotalBrushes += lLevelData.Entities[i].Brushes.Length;

            List<UEntity> lUEnts = new List<UEntity>(lLevelData.Entities.Length);

            for (int i = 0; i < lLevelData.Entities.Length; i++)
            {
                QEntity lQEnt = lLevelData.Entities[i];
                EntDef lEntDef = s_Settings.EntDefs.GetDefinition(lQEnt.Classname);

                GameObject lEntGO = null;
                UEntity lUEnt = null;

                if (lEntDef.ConvertedPrefab != null)
                {
                    lEntGO = GameObject.Instantiate(lEntDef.ConvertedPrefab).gameObject;
                    lEntGO.name = i + " " + lQEnt.Classname;
                    lUEnt = lEntGO.GetComponent<UEntity>();
                }
                else if (lEntDef.RuntimePrefab != null)
                {
                    lEntGO = new GameObject(i + " " + lQEnt.Classname);
                    lUEnt = lEntGO.AddComponent<UEntity>();
                }
                else
                {
                    lEntGO = new GameObject(i + " " + lQEnt.Classname);
                    lUEnt = lEntGO.AddComponent<UEmptyEntity>();
                }

                lUEnts.Add(lUEnt);

                lEntGO.isStatic = lEntDef.IsStatic;
                lEntGO.layer = lEntDef.EntLayer.LayerIndex;

                lEntGO.transform.parent = lLevelObject.transform;

                List<GameObject> lBrushes = new List<GameObject>();
                List<ConvexMeshData> lConvexMeshDatas = new List<ConvexMeshData>();

                for (int j = 0; j < lQEnt.Brushes.Length; j++)
                {
                    QBrush lBrush = lQEnt.Brushes[j];

                    lBrushCount++;

                    CMesh lCMesh = ClipMesh.CreateConvexPolyhedron(lBrush.Planes);

                    List<Vector3> lVertList = new List<Vector3>();

                    for (int v = 0; v < lCMesh.V.Length; v++)
                        if (lCMesh.V[v].Visible)
                            lVertList.Add(lCMesh.V[v].Position);

                    Vector3 lMin;
                    Vector3 lMax;

                    GetMinMax(lVertList.ToArray(), out lMin, out lMax);

                    Vector3 lSize = lMax - lMin;
                    Vector3 lMidPointDelta = lMin + lSize * 0.5f;

                    lCMesh = ClipMesh.CreateConvexPolyhedron(lBrush.Planes, lMidPointDelta, lSize + Vector3.one * 0.2f);

                    lVertList.Clear();

                    for (int v = 0; v < lCMesh.V.Length; v++)
                        if (lCMesh.V[v].Visible)
                            lVertList.Add(lCMesh.V[v].Position);

                    GetMinMax(lVertList.ToArray(), out lMin, out lMax);

                    lSize = lMax - lMin;
                    lMidPointDelta = lMin + lSize * 0.5f;

                    UMeshData lMeshData = lCMesh.GetMeshInformation(lMidPointDelta);

                    Texture[] lTextures = FetchTextures(lMeshData);

                    UpdateMaterials(lTextures);

                    UpdateMeshData(ref lMeshData, lTextures);

                    lConvexMeshDatas.Add(new ConvexMeshData(lMeshData, lMidPointDelta));

                    Mesh[] lNewMeshes = UMeshCreator.ConvertToMeshes(lMeshData);

                    if (s_Settings.AutoGenerateUV2s)
                    {
                        for (int m = 1; m < lNewMeshes.Length; m++)
                        {
                            UnwrapParam lParam = new UnwrapParam();
                            lParam.angleError = 0.05f;
                            lParam.areaError = 0.05f;
                            lParam.hardAngle = 85f;
                            lParam.packMargin = 2f;

                            Unwrapping.GenerateSecondaryUVSet(lNewMeshes[m], lParam);
                        }
                    }

                    if (lEntDef.HasCollider || lEntDef.HasMesh)
                    {
                        GameObject lColliderGO = new GameObject("Brush " + j);

                        lBrushes.Add(lColliderGO);

                        if (lEntDef.HasCollider)
                        {
                            lColliderGO.transform.position = lMidPointDelta;
                            lColliderGO.transform.parent = lEntGO.transform;

                            lColliderGO.isStatic = lEntDef.IsStatic;
                            lColliderGO.layer = lEntDef.ColLayer.LayerIndex;

                            MeshCollider lMCollider = lColliderGO.AddComponent<MeshCollider>();
                            lMCollider.sharedMesh = lNewMeshes[0];
                            lMCollider.convex = true;
                            lMCollider.isTrigger = lEntDef.IsTrigger;

                            //if (s_Settings.SaveLevelAsAsset)
                            //    AssetDatabase.CreateAsset(lNewMeshes[0], GetAssetPath(lEntGO.name, lColliderGO.name, lNewMeshes[0].name));
                        }

                        if (lEntDef.HasMesh)
                        {
                            for (int m = 1; m < lNewMeshes.Length; m++)
                            {
                                GameObject lMeshGO = new GameObject("Mesh " + (m - 1));

                                lMeshGO.transform.position = lMidPointDelta;
                                lMeshGO.transform.parent = lColliderGO.transform;

                                lMeshGO.isStatic = lEntDef.IsStatic;
                                lMeshGO.layer = lEntDef.MeshLayer.LayerIndex;

                                MeshFilter lMFilter = lMeshGO.AddComponent<MeshFilter>();
                                MeshRenderer lMRender = lMeshGO.AddComponent<MeshRenderer>();

                                lMRender.sharedMaterial = s_MaterialDic[lTextures[m - 1].name];
                                lMRender.receiveShadows = true;
                                lMRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                                lMRender.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                                lMFilter.mesh = lNewMeshes[m];

                                //if (s_Settings.SaveLevelAsAsset)
                                //    AssetDatabase.CreateAsset(lNewMeshes[m], GetAssetPath(lEntGO.name, lColliderGO.name, lMeshGO.name, lNewMeshes[m].name));

                                /**
                                 * 
                                 *  Add Area Lights
                                 * 
                                **/

                                TexDef lTexDef;
                                if (s_Settings.TexDefs.HasDefinition(lTextures[m - 1].name, out lTexDef) && lTexDef.HasAreaLight)
                                {
                                    GameObject lAreaLightGO = new GameObject("AreaLight " + (m - 1));
                                    lAreaLightGO.transform.position = lMidPointDelta;
                                    lAreaLightGO.transform.parent = lMeshGO.transform;

                                    lAreaLightGO.isStatic = lEntDef.IsStatic;
                                    //lAreaLightGO.layer = LayerMask.NameToLayer("AreaLight");

                                    lAreaLightGO.AddComponent<DestroyObjectOnSpawn>();

                                    MeshFilter lALMFilter = lAreaLightGO.AddComponent<MeshFilter>();
                                    MeshRenderer lALMRender = lAreaLightGO.AddComponent<MeshRenderer>();

                                    lALMRender.sharedMaterial = s_MaterialDic[lTextures[m - 1].name + "_AreaLightMAT"];
                                    lALMRender.receiveShadows = false;
                                    lALMRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                                    lALMRender.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                                    Mesh lAreaLightMesh = new Mesh();

                                    FaceInfo[] lFaceInfos = lMeshData.MeshFaceInfos[m];

                                    int lVertCount = 0;
                                    int lTriCount = 0;

                                    for (int f = 0; f < lFaceInfos.Length; f++)
                                    {
                                        lVertCount += lFaceInfos[f].Indices.Length;

                                        if (lFaceInfos[f].Indices.Length > 2)
                                            lTriCount += lFaceInfos[f].Indices.Length - 2;
                                    }

                                    int lTriIndex = 0;

                                    Vector3[] lNewVerts = new Vector3[lVertCount];
                                    Vector3[] lNormals = new Vector3[lVertCount];
                                    int[] lTriangles = new int[lTriCount * 3];
                                    int[][] lNewIndices = new int[lFaceInfos.Length][];

                                    int lVertIndex = 0;

                                    Vector3[] lPosVerts = lMeshData.PosVertices;
                                    Vector3[] lUVVerts = lMeshData.UVVertices;

                                    for (int f = 0; f < lFaceInfos.Length; f++)
                                    {
                                        FaceInfo lFaceInfo = lFaceInfos[f];
                                        Plane lPlane = lFaceInfo.Plane;
                                        int[] lIndices = lFaceInfo.Indices;

                                        lNewIndices[f] = new int[lIndices.Length];

                                        for (int v = 0; v < lIndices.Length; v++)
                                        {
                                            Vector3 lVertPos = lPosVerts[lIndices[v]];
                                            Vector3 lVertUV = lUVVerts[lIndices[v]];

                                            lNewVerts[lVertIndex] = (lVertPos * lTexDef.AreaSizeScale) - lPlane.Normal * lTexDef.AreaDisplacement;
                                            lNormals[lVertIndex] = -lPlane.Normal;

                                            lNewIndices[f][v] = lVertIndex;

                                            lVertIndex++;
                                        }

                                        for (int v = 0; v < lNewIndices[f].Length - 2; v++)
                                        {
                                            lTriangles[lTriIndex++] = lNewIndices[f][0];
                                            lTriangles[lTriIndex++] = lNewIndices[f][v + 1];
                                            lTriangles[lTriIndex++] = lNewIndices[f][v + 2];
                                        }
                                    }

                                    lAreaLightMesh.vertices = lNewVerts;
                                    lAreaLightMesh.triangles = lTriangles;
                                    lAreaLightMesh.normals = lNormals;

                                    lAreaLightMesh.RecalculateTangents();
                                    lAreaLightMesh.RecalculateBounds();

                                    lAreaLightMesh.name = "Area Light Mesh";

                                    Unwrapping.GenerateSecondaryUVSet(lAreaLightMesh);

                                    lALMFilter.mesh = lAreaLightMesh;

                                    //if (s_Settings.SaveLevelAsAsset)
                                    //    AssetDatabase.CreateAsset(lAreaLightMesh, GetAssetPath(lEntGO.name, lColliderGO.name, lMeshGO.name, lAreaLightMesh.name));
                                }
                            }
                        }
                    }
                }

                {
                    List<Vector3> lConvexVertList = new List<Vector3>();
                    List<FaceInfo> lFaceList = new List<FaceInfo>();

                    int lIndexShift = 0;

                    for (int c = 0; c < lConvexMeshDatas.Count; c++)
                    {
                        int lVertCount = lConvexMeshDatas[c].MeshData.PosVertices.Length;
                        Vector3 lDeltaPos = lConvexMeshDatas[c].DeltaPos;

                        for (int v = 0; v < lVertCount; v++)
                            lConvexVertList.Add(lConvexMeshDatas[c].MeshData.PosVertices[v] + lDeltaPos);

                        for (int f = 0; f < lConvexMeshDatas[c].MeshData.FaceInfos.Length; f++)
                        {
                            FaceInfo lInfo = lConvexMeshDatas[c].MeshData.FaceInfos[f];

                            for (int fi = 0; fi < lInfo.Indices.Length; fi++)
                                lInfo.Indices[fi] += lIndexShift;

                            lFaceList.Add(lInfo);
                        }

                        lIndexShift += lVertCount;
                    }

                    Vector3 lMin, lMax;

                    GetMinMax(lConvexVertList.ToArray(), out lMin, out lMax);

                    Vector3 lSize = lMax - lMin;
                    Vector3 lMidPoint = lMin + lSize * 0.5f;

                    for (int v = 0; v < lConvexVertList.Count; v++)
                        lConvexVertList[v] -= lMidPoint;

                    UMeshData lConvexData = new UMeshData();
                    lConvexData.PosVertices = lConvexData.UVVertices = lConvexVertList.ToArray();

                    lConvexData.MeshFaceInfos = new FaceInfo[1][];
                    lConvexData.MeshFaceInfos[0] = lFaceList.ToArray();

                    if (lEntDef.HasConvexCollider && lConvexMeshDatas.Count > 0)
                    {
                        Mesh[] lConvexMesh = UMeshCreator.ConvertToMeshes(lConvexData);

                        //if (!AssetDatabase.IsValidFolder("Assets/" + m_MapFile.name))
                        //    AssetDatabase.CreateFolder("Assets", m_MapFile.name);

                        //AssetDatabase.CreateAsset(lConvexMesh[0], "Assets/" + m_MapFile.name + "/OriginalMesh.asset");

                        MeshCollider lMCollider = lUEnt.gameObject.AddComponent<MeshCollider>();
                        lMCollider.sharedMesh = lConvexMesh[0];
                        lMCollider.convex = true;
                        lMCollider.isTrigger = lEntDef.IsConvexTrigger;

                        //if (s_Settings.SaveLevelAsAsset)
                        //    AssetDatabase.CreateAsset(lConvexMesh[0], GetAssetPath(lEntGO.name, "CONVEX", lConvexMesh[0].name));
                    }

                    lUEnt.SetupEntity(lQEnt);

                    Vector3 lValue;
                    if (lUEnt.GetValue("origin", out lValue))
                        lUEnt.transform.position = lValue;
                    else if (lBrushes.Count > 0)
                    {
                        for (int b = 0; b < lBrushes.Count; b++)
                        {
                            lBrushes[b].transform.localPosition = lBrushes[b].transform.position - lMidPoint;
                        }

                        lUEnt.transform.position = lMidPoint;
                    }
                }
            }

            //var lSO = new SerializedObject(lLevelObject);

            //if (lSO.ApplyModifiedProperties())
            //{
            //    bool lBreakpoint = false;
            //}

            //if (s_Settings.SaveLevelAsAsset)
            //{
            //    PrefabUtility.SaveAsPrefabAsset(lLevelObject, "Assets/" + s_Settings.MapFile.name + ".prefab", out lSuccess);
            //    AssetDatabase.Refresh();
            //    AssetDatabase.SaveAssets();
            //}

            //if (EditorGUI.EndChangeCheck())
            //{
            //    bool lBreakpoint = false;
            //}

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static string GetAssetPath(params string[] lNames)
        {
            string lPath;

            lPath = string.Concat(string.Concat("Assets/", string.Concat(lNames)), ".asset");

            return lPath;
        }

        private static void GetMinMax(Vector3[] lVerts, out Vector3 lMin, out Vector3 lMax)
        {
            Vector3 lMinTemp = Vector3.positiveInfinity;
            Vector3 lMaxTemp = Vector3.negativeInfinity;

            for (int v = 0; v < lVerts.Length; v++)
            {
                Vector3 lPos = lVerts[v];

                if (lMinTemp.x > lPos.x)
                    lMinTemp.x = lPos.x;
                if (lMinTemp.y > lPos.y)
                    lMinTemp.y = lPos.y;
                if (lMinTemp.z > lPos.z)
                    lMinTemp.z = lPos.z;

                if (lMaxTemp.x < lPos.x)
                    lMaxTemp.x = lPos.x;
                if (lMaxTemp.y < lPos.y)
                    lMaxTemp.y = lPos.y;
                if (lMaxTemp.z < lPos.z)
                    lMaxTemp.z = lPos.z;
            }

            lMin = lMinTemp;
            lMax = lMaxTemp;
        }

        private static Texture[] FetchTextures(UMeshData lMeshInfo)
        {
            List<string> lTextureNames = new List<string>();

            for (int i = 0; i < lMeshInfo.FaceInfos.Length; i++)
            {
                string lTexName = lMeshInfo.FaceInfos[i].Plane.TextureData.Name;
                if (!lTextureNames.Contains(lTexName))
                    lTextureNames.Add(lTexName);
            }

            List<Texture> lTextures = new List<Texture>();
            int lCount = lTextureNames.Count;

            for (int i = 0; i < lCount; i++)
            {
                string lFilePath = lTextureNames[i];
                string[] lPathNames = lFilePath.Split('/');

                string lTexName = lPathNames[lPathNames.Length - 1];

                Texture lTexture = null;

                for (int j = 0; j < s_Settings.TexDefs.Textures.Length; j++)
                {
                    if (lTexName == s_Settings.TexDefs.Textures[j].name)
                    {
                        lTexture = s_Settings.TexDefs.Textures[j];
                        lTextures.Add(lTexture);
                        break;
                    }
                }
            }

            return lTextures.ToArray();
        }

        private static void UpdateMaterials(Texture[] lTextures)
        {
            for (int i = 0; i < lTextures.Length; i++)
            {
                if (!s_MaterialDic.ContainsKey(lTextures[i].name))
                {
                    TexDef lTexDef;

                    if (s_Settings.TexDefs.HasDefinition(lTextures[i].name, out lTexDef))
                    {
                        if (lTexDef.EmissiveTexture != null)
                        {
                            Material lDefaultMat = Material.Instantiate<Material>(s_Settings.EmissiveMaterial);

                            lDefaultMat.SetTexture(s_BaseColourMapID, lTextures[i]);

                            lDefaultMat.SetTexture(s_EmissionMapID, lTexDef.EmissiveTexture);
                            lDefaultMat.SetColor(s_EmissionColorID, lTexDef.EmissiveColour);

                            if (s_Settings.UsingHDRPMaterials)
                                lDefaultMat.SetColor(s_EmissionColorLDRID, lTexDef.EmissiveColour);
                            else
                                lDefaultMat.EnableKeyword("_EMISSION");

                            lDefaultMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

                            s_MaterialDic.Add(lTextures[i].name, lDefaultMat);
                        }
                        else if (lTexDef.IsCutout)
                        {
                            Material lCutoutMat = Material.Instantiate<Material>(s_Settings.CutoutMaterial);

                            lCutoutMat.SetTexture(s_BaseColourMapID, lTextures[i]);
                            lCutoutMat.SetFloat(s_CutoffAlphaID, lTexDef.CutoutAlpha);

                            if (s_Settings.UsingHDRPMaterials)
                                lCutoutMat.SetFloat(s_CutoffEnabledID, 1f);

                            s_MaterialDic.Add(lTextures[i].name, lCutoutMat);
                        }
                        else
                        {
                            Material lDefaultMat = Material.Instantiate<Material>(s_Settings.StandardMaterial);

                            lDefaultMat.SetTexture(s_BaseColourMapID, lTextures[i]);

                            s_MaterialDic.Add(lTextures[i].name, lDefaultMat);
                        }

                        if (lTexDef.HasAreaLight)
                        {
                            Material lAreaLightMat = Material.Instantiate<Material>(s_Settings.AreaLightMaterial);

                            lAreaLightMat.SetColor(s_EmissionColorID, lTexDef.AreaColour);

                            if (s_Settings.UsingHDRPMaterials)
                                lAreaLightMat.SetColor(s_EmissionColorLDRID, lTexDef.EmissiveColour);
                            else
                                lAreaLightMat.EnableKeyword("_EMISSION");

                            lAreaLightMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

                            s_MaterialDic.Add(lTextures[i].name + "_AreaLightMAT", lAreaLightMat);
                        }
                    }
                    else
                    {
                        Material lDefaultMat = GameObject.Instantiate<Material>(s_Settings.StandardMaterial);

                        lDefaultMat.SetTexture(s_BaseColourMapID, lTextures[i]);

                        s_MaterialDic.Add(lTextures[i].name, lDefaultMat);
                    }
                }
            }
        }

        private static void UpdateMeshData(ref UMeshData lMeshData, Texture[] lTextures)
        {
            int lMeshCount = lTextures.Length + 1;
            FaceInfo[][] lMeshFaceInfos = new FaceInfo[lMeshCount][];

            int lFaceCount = lMeshData.FaceInfos.Length;
            lMeshFaceInfos[0] = new FaceInfo[lFaceCount];

            for (int i = 0; i < lFaceCount; i++)
                lMeshFaceInfos[0][i] = lMeshData.FaceInfos[i];

            for (int m = 1; m < lMeshCount; m++)
            {
                Texture lTexture = lTextures[m - 1];
                Vector2 lTexSizeScale = new Vector2(16f / lTexture.width, 16f / lTexture.height);

                List<FaceInfo> lFoundFaces = new List<FaceInfo>();

                for (int f = 0; f < lFaceCount; f++)
                {
                    FaceInfo lFaceInfo = lMeshData.FaceInfos[f];

                    string lFilePath = lFaceInfo.Plane.TextureData.Name;
                    string[] lPathNames = lFilePath.Split('/');

                    string lTexName = lPathNames[lPathNames.Length - 1];

                    if (lTexName == lTexture.name)
                    {
                        TextureData lTexData = lFaceInfo.Plane.TextureData;

                        lTexData.Offset = lTexData.Offset / 16f;
                        lTexData.Offset.y = -lTexData.Offset.y;
                        lTexData.Offset *= lTexSizeScale;

                        lTexData.Scale = lTexSizeScale / lTexData.Scale;

                        lFaceInfo.Plane.TextureData = lTexData;

                        lFoundFaces.Add(lFaceInfo);
                    }
                }

                lMeshFaceInfos[m] = lFoundFaces.ToArray();
            }

            lMeshData.MeshFaceInfos = lMeshFaceInfos;
        }
    }
}
