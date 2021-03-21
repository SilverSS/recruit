using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildingReneder : MonoBehaviour
{
    public bool showGizmos = false;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    Material _mat;

    BuildingData _meshData;

    public void SetMeshData(BuildingData meshData, Material mat)
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _meshFilter = gameObject.GetComponent<MeshFilter>();

        _meshData = meshData;
        _meshRenderer.material = mat;
        _meshFilter.mesh = _meshData.mesh;
        _mat = _meshRenderer.material;

        SetTextureScale();
    }

    void SetTextureScale()
    {
        // 건물 높이에 따른 텍스쳐 Scale 조정
        Mesh mesh = _meshFilter.mesh;

        float maxHeight = 0f;
        for (int i = 0; i < mesh.vertices.Length-1; i+=3)
        {
            float diff = Mathf.Abs(mesh.vertices[i].y - mesh.vertices[i+1].y);
            maxHeight = Mathf.Max(maxHeight, diff);
            diff = Mathf.Abs(mesh.vertices[i+1].y - mesh.vertices[i+2].y);
            maxHeight = Mathf.Max(maxHeight, diff);
        }

        int floorHeight = maxHeight < 3f ? 1 : (int)(maxHeight / 3.0f);
        _mat.SetTextureScale("_BaseMap", new Vector2(1.0f, floorHeight));
    }

    public void OnUpdate()
    {
        
    }

    // 디버그용 기능 (과제 구현시 필요한 정점당 normal Vector 및 과제 조건용 각도 확인 용도)
    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject == gameObject && showGizmos)
        {
            for (int j = 0; j < _meshFilter.mesh.vertices.Length; j++)
            {
                Color color = Color.red;
                //Debug.Log(buildingObjects[i].name + ": " + j);
                Gizmos.color = color;
                Gizmos.DrawLine(_meshFilter.mesh.vertices[j], _meshFilter.mesh.vertices[j] + (_meshFilter.mesh.normals[j] * 10f));

                float angle1 = Vector3.Angle(_meshFilter.mesh.normals[j], Vector3.forward);
                if (_meshFilter.mesh.normals[j].x < 0f)
                {
                    angle1 += (180f - angle1) * 2f;
                }

                GUIStyle style = GUIStyle.none;
                style.fontSize = 10;
                //Handles.Label(_meshFilter.mesh.vertices[j] + (_meshFilter.mesh.normals[j] * 10f), angle1.ToString());
                Handles.Label(_meshFilter.mesh.vertices[j] + (_meshFilter.mesh.normals[j] * 10f), _meshFilter.mesh.normals[j].ToString(), style);
            }
        }
    }
}
