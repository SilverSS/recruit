using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFramework : MonoBehaviour
{
    [SerializeField]
    string jsonFileName;    // Resource - Text File Name

    [SerializeField]
    string textureFileName; // Resource - Texture File Name

    List<BuildingReneder> buildingRenderers = new List<BuildingReneder>();      // 각 건물 오브젝트 렌더링 관련
    Dictionary<string, GameObject> buildingObjectDic = new Dictionary<string, GameObject>();  // 검색용 Dictionary
    GameObject buildObjecs;     // 건물 오브젝트들의 상위 오브젝트 인스턴스

    bool isInitialized = false;     // 초기화 완료 체크 변수

    // Start is called before the first frame update
    void Start()
    {
        // 초기화 함수 (건물 정보 리소스 로드 후 오브젝트 구성
        OnInitilize();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInitialized)  // 초기화가 끝났다면 매프레임 마다
        {
            // 각 구성된 건물 렌더 오브젝트 OnUpdate 호출
            foreach (var buildingRenderer in buildingRenderers)
            {
                buildingRenderer.OnUpdate();
            }
        }
    }

    // 초기화 함수 구현
    public void OnInitilize()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        // Json text asset 로드
        ResourceRequest request = Resources.LoadAsync<TextAsset>("JsonTexts/" + jsonFileName);
        while (request.isDone == false)
        {
            yield return null;
        }
        TextAsset asset = request.asset as TextAsset;
        if (asset == null)
        {
            Debug.Log("Can't load json asset!!");
            yield break;
        }

        // Texture 로드
        request = Resources.LoadAsync<Texture2D>("Textures/" + textureFileName);
        while (request.isDone == false)
        {
            yield return null;
        }
        Texture2D texture = request.asset as Texture2D;
        if (asset == null)
        {
            Debug.Log("Can't load texture asset!!");
            yield break;
        }

        // 바닥 Plane 오브젝트 생성
        yield return GenerateGround();

        // Scene에 건물 오브젝트 구성
        yield return GenerateBuildingObjects(asset, texture);

        isInitialized = true;
        yield break;
    }

    IEnumerator GenerateGround()
    {
        GameObject groundObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        groundObject.transform.position = Vector3.zero;
        groundObject.transform.localScale = new Vector3(200f, 0f, 200f);
        yield break;
    }

    // 건물 GameObject및 Renderer 관련 컴포넌트 구성
    IEnumerator GenerateBuildingObjects(TextAsset textAsset, Texture2D texture)
    {
        // 건물 오브젝트 상위 오브젝트 생성 (null일경우)
        if (buildObjecs == null)
            buildObjecs = new GameObject("BuildingObjects");

        // Text 리소스로 부터 데이터 구성
        List<BuildingData> buildingDatas = JsonMeshImpoter.ImportMesh(textAsset);

        // 기본 머터리얼 생성 및 구성
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetTexture("_BaseMap", texture);

        // BuildingData 단위(meta 데이터의 '동' 단위)로 BuildingObject 구성
        for (int i = 0; i < buildingDatas.Count; i++)
        {
            // GameObject 생성 및 구성
            string objectName = string.Format(buildingDatas[i].meta.bd_id + "_" + buildingDatas[i].meta.동, i);
            GameObject buildingObject = new GameObject(objectName);
            buildingObject.transform.parent = transform;
            buildingObject.transform.position = Vector3.zero;

            // 렌더링 관련 BuildingRenerer 구성
            BuildingReneder buildingRenderer = buildingObject.AddComponent<BuildingReneder>();
            buildingObject.AddComponent<MeshFilter>();
            buildingObject.AddComponent<MeshRenderer>();
            buildingRenderer.SetMeshData(buildingDatas[i], mat);
            buildingRenderers.Add(buildingRenderer);
            buildingObjectDic.Add(objectName, buildingObject);

            // 동별 건물 하나 구성후 한 프레임 대기 (생략가능)
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
