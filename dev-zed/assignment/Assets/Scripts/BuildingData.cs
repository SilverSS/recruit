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
            // 삼각형 구성 정점 순서가 (i,i+2,i+1) 순서
            Vector3 vertex = new Vector3(floats[i], floats[i+2], floats[i+1]);
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

        #region Diff 텍스쳐 매핑 1번 조건을 만족하기 위한 다른 구현방법 
        /* --------------------------------------------------------------------------
         * 노멀 벡터가 가장 많은 곳을 넓은 면으로 판단 하기 위해 Normal 좌표들을 분류
         * -------------------------------------------------------------------------- */
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

        // 노멀 좌표가 가장 많은 List<Vector3> (normalList)를 1번조건을 판단하기 위한 노멀리스트로 구분
        //List<Vector3> mostNormalList = null;
        //for (int i = 0; i < normalGroups.Count-1; i++)
        //{
        //    if (mostNormalList == null)
        //        mostNormalList = normalGroups[i];

        //    mostNormalList = normalGroups[i].Count > normalGroups[i+1].Count ? normalGroups[i] : normalGroups[i+1];
        //}
        #endregion

        for (int i = 0; i < _mesh.normals.Length; i++)
        {
            float u = 0f;
            float v = 0f;

            // 삼각형 구성 정점 순서에 따른 기본 uv 세팅
            u = (i % 6 == 1 || i % 6 == 2 || i % 6 == 3) ? 0.0f : 1.0f;
            v = (i % 6 == 0 || i % 6 == 1 || i % 6 == 5) ? 0.0f : 1.0f;

            // 1번조건 문제 요구 사항에서의 벡터간 각도 비교
            float angle1 = Vector3.Angle(_mesh.normals[i], Vector3.forward);

            // 다른 구현 방법을 위한 벡터간 각도 비교
            //float angle1 = Vector3.Angle(_mesh.normals[i], mostNormalList[0]);    

            // Vector3의 Angle함수는 180.0f의 최대값을 가집니다.
            // 이는 기준 방향이 없다는 가정하에 두 벡터가 이루는 각도중 작은 값을 돌려주기 때문인데
            // ( ex - 시계방향 270f = 시계 반대방향 90f )
            // 문제 조건인 180~220사이의 각으로 한정을 해야 한다면, 방향 기준이 있어야 합니다.
            // 본 구현에서는 시계 방향을 기준으로 각도를 계산 하도록 했습니다.

            // 시계방향일경우 Vector3.forward가 (0, 0, 1)이고 normal의 x축의 방향이 0보다 작을 경우
            // 180도 보다 큰 각을 가지게 됩니다.
            if (_mesh.normals[i].x < 0f)                    
            {
                // 180도가 넘어가는 경우인데 180보다 작다면 이미 180도를 지나서 그 만큼 감소했다는 의미이므로
                // Angle함수로 나온 값과 180도간의 차(감소한 각도)를 두번 더해 시계방향으로 180도를 넘는 각도를 계산합니다.
                angle1 += (180f - angle1) * 2f;             
            }

            // 2번 조건을 위한 Up, Down 벡터와의 비교 각
            float angle2 = Vector3.Angle(_mesh.normals[i], Vector3.up);
            float angle3 = Vector3.Angle(_mesh.normals[i], Vector3.down);

            // 소수점 오차는 잘라내기 위해 int로 형 변환

            if ((int)angle2 == 0f || (int)angle3 == 0f) // 조건이 확실한 2번 조건 먼저 체크
            {
                u = Mathf.Lerp(0.75f, 1.0f, u);         // u값의 기본조건을 1,2번 파라미터에 (최소,최대) 맞춰 제한, 이하 동일
            }
            else
            {
                if (angle1 >= 180f && angle1 <= 220f) // 과제 제시 1번 조건식
                //if (angle1 <= 10 || angle1 >= 170f) // 별도 구현 방법 조건식
                {
                    u = Mathf.Lerp(0.0f, 0.5f, u);
                }
                else                                  // 1, 2번 조건에 해당하지 않는 모든경우
                {
                    u = Mathf.Lerp(0.5f, 0.75f, u);
                }
            }

            Vector2 uv = new Vector2(u, v);           // uv Vector2 생성
            _UVs.Add(uv);                             // uv 목록 리스트에 추가
        }
        _mesh.uv = _UVs.ToArray();
    }
}
