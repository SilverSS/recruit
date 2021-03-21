using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class APIResponse
{
    public bool success;
    public int code;
    public List<BuildingInfo> data;
}

[Serializable]
public class BuildingInfo
{
    public List<RoomTypeInfo> roomtypes;
    public BuildingMeta meta;
}

[Serializable]
public class BuildingMeta
{
    public int bd_id;
    public string 동;
    public int 지면높이;
}

[Serializable]
public class RoomTypeInfo
{
    public List<string> coordinatesBase64s;
    public RoomTypeMeta meta;
}

[Serializable]
public class RoomTypeMeta
{
    public int 룸타입id;
}

public class JsonMeshImpoter
{
    public static List<BuildingData> ImportMesh(TextAsset asset)
    {
        List<BuildingData> buildingDatas = new List<BuildingData>();

        APIResponse apiResponse = JsonUtility.FromJson<APIResponse>(asset.text);
        List<BuildingInfo> buildingInfos = apiResponse.data;
        for (int i = 0; i < buildingInfos.Count; i++)
        {
            BuildingData buildingData = new BuildingData();
            for (int j = 0; j < buildingInfos[i].roomtypes.Count; j++)
            {
                RoomTypeInfo roomtype = buildingInfos[i].roomtypes[j];
                for (int k = 0; k < roomtype.coordinatesBase64s.Count; k++)
                {
                    string coordinatesString = roomtype.coordinatesBase64s[k];
                    byte[] bytes = Convert.FromBase64String(coordinatesString);
                    float[] floats = new float[bytes.Length / 4];

                    for (int l = 0; l < bytes.Length / 4; l++)
                    {
                        floats[l] = BitConverter.ToSingle(bytes, l * 4);
                    }

                    buildingData.AddVertices(floats);
                }
            }

            buildingData.GenerateMesh();
            buildingData.SetMetaData(buildingInfos[i].meta);
            buildingDatas.Add(buildingData);
        }

        return buildingDatas;
    }
}
