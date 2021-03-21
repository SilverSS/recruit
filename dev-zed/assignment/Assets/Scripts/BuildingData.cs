using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum MappingType
{
    Normal,
    TopBottom,
    Side
}

public class BuildingData
{
    List<Vector3> _vertices = new List<Vector3>();
    List<int> _indices = new List<int>();
    List<Vector2> _UVs = new List<Vector2>();

    BuildingMeta _meta;
    public BuildingMeta meta
    {
        get { return _meta; }
    }

    Mesh _mesh;
    public Mesh mesh
    {
        get { return _mesh; }
    }

    public void AddVertices(float[] floats)
    {
        for (int i = 0; i < floats.Length; i+=3)
        {
            Vector3 vertex = new Vector3(floats[i], floats[i+2], floats[i+1]);
            //Debug.Log("Vertices[" + (i / 3) + "] : " + (vertex));

            _vertices.Add((vertex));
        }
    }

    public void GenerateMesh()
    {
        _mesh = new Mesh();
        SetBuffers();
    }

    public void SetMetaData(BuildingMeta meta)
    {
        _meta = meta;
    }

    void SetBuffers()
    {
        // Vertex buffer setting
        _mesh.vertices = _vertices.ToArray();

        // Index buffer setting
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _indices = Enumerable.Range(0, _vertices.Count).ToList();
        _mesh.triangles = _indices.ToArray();

        // Caculate Normals
        _mesh.RecalculateNormals();

        //List<List<Vector3>> normalGroups = new List<List<Vector3>>();
        //for (int i = 0; i < 4; i++)
        //{
        //    normalGroups.Add(new List<Vector3>());
        //}

        //for (int i = 0; i < _mesh.normals.Length; i++)
        //{
        //    foreach (List<Vector3> group in normalGroups)
        //    {
        //        if (group.Count == 0)
        //        {
        //            group.Add(_mesh.normals[i]);
        //            break;
        //        } 
        //        else
        //        {
        //            float angle = Vector3.Angle(group[0], _mesh.normals[i]);
        //            if ((int)angle < 5)
        //            {
        //                group.Add(_mesh.normals[i]);
        //                break;
        //            }
        //        }
        //    }
        //}

        //List<Vector3> mostNormalList = null;
        //for (int i = 0; i < normalGroups.Count-1; i++)
        //{
        //    if (mostNormalList == null)
        //        mostNormalList = normalGroups[i];

        //    mostNormalList = normalGroups[i].Count > normalGroups[i+1].Count ? normalGroups[i] : normalGroups[i+1];
        //}

        for (int i = 0; i < _mesh.normals.Length; i++)
        {
            float u = 0f;
            float v = 0f;

            u = (i % 6 == 1 || i % 6 == 2 || i % 6 == 3) ? 0.0f : 1.0f;
            v = (i % 6 == 0 || i % 6 == 1 || i % 6 == 5) ? 0.0f : 1.0f;

            float angle1 = Vector3.Angle(_mesh.normals[i], Vector3.forward);
            //float angle1 = Vector3.Angle(_mesh.normals[i], mostNormalList[0]);
            
            if (_mesh.normals[i].x < 0f)                    // 시계 방향 기준 180도 보다 큰 각을 찾음
            {
                angle1 += (180f - angle1) * 2f;
            }

            float angle2 = Vector3.Angle(_mesh.normals[i], Vector3.up);
            float angle3 = Vector3.Angle(_mesh.normals[i], Vector3.down);

            if ((int)angle2 == 0f || (int)angle3 == 0f)
            {
                u = Mathf.Lerp(0.75f, 1.0f, u);
            }
            else
            {
                //if (angle1 >= 180f && angle1 <= 220f)
                if (angle1 <= 10 || angle1 >= 170f)
                {
                    u = Mathf.Lerp(0.0f, 0.5f, u);
                }
                else
                {
                    u = Mathf.Lerp(0.5f, 0.75f, u);
                }
            }

            Vector2 uv = new Vector2(u, v);
            //Debug.Log(uv);
            _UVs.Add(uv);
        }
        _mesh.uv = _UVs.ToArray();

        //Debug.Log("Vectices Count : " + _vertices.Count);
        //Debug.Log("Indices Count : " + _indices.Count);
        //Debug.Log("Normals Count : " + _mesh.normals.Length);

        // UV Setting
    }
}
