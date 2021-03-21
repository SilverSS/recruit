# ZIGBANG ZED Assignment

안녕하세요. 직방 ZED 팀 지원자 여러분. 지원해주셔서 감사합니다.

과제 설명 전 간단한 안내 말씀 드리겠습니다.

직방 ZED 팀은 유니티 엔진을 이용하여 직방에 있는 부동산 빅데이터들을

3D 정보로 시각화해주는 일을 하고 있습니다.

이와 관련하여 과제도 간단한 데이터를 이용해 3D 시각화로 진행하고 있습니다.

Github 의 fork 기능을 이용하여 개인 저장소로 복사해주신 다음 

코드를 작성 및 README.MD 파일 수정 후 개인 저장소의 URL을 채용 담당자에게 공유해주시면 됩니다.

최하단의 [문제 해결 방법] 섹션은 면접에 도움이 될 수 있도록

과제를 하면서 고민했던 부분, 해결방안 등에 대해 자유롭게 작성해주시면 됩니다.

## 과제 설명

- 과제는 유니티 2019.4.21f1 으로 작성합니다.

- Scenes/ZedScene.unity 씬에서 확인 가능하도록 작성부탁드립니다.

### 문제1) 단지에 있는 동의 폴리곤 데이터를 이용하여 런타임에 메쉬를 구성하여 렌더링해주세요.(40점)

샘플 데이터는 아래의 경로에 위치해 있습니다.

Assets/Samples/json/dong.json

다음과 같이 구성되어 있습니다.

```
API Response 구성
{
    success: boolean, // API 성공 여부
    code: number, // API 결과 코드
    data: [] // 동 데이터 배열
}

동 데이터 구성
{
    roomtypes: [], // 타입 데이터의 배열
    meta: {
        bd_id: number, // 동 id
        동: string, // 동 이름
        지면높이: number, // 지면으로 부터의 높이
    } // 메타 정보
}

타입 데이터 구성
{
    coordinatesBase64s: string[], // 폴리곤 데이터의 배열
    meta: {
        룸타입id: number // 타입 id
    }
}

폴리곤 데이터의 구성 (*중요*)
- float[] 을 byte[] 로 변환후 Base64 로 인코딩된 데이터
- float[] 는 3개씩 나누어 Vector3 의 x,z,y 에 매핑됩니다.
  ex) [1.0, 2.0, 3.0, 4.0, 5.0, 6.0]
  -> [Vector3(1.0, 3.0, 2.0), Vector3(4.0, 6.0, 5.0)]
- C# 의 Buffer.BlockCopy, Convert.FromBase64String 등의 함수를 이용하시면 됩니다.
```
**폴리곤 데이터 1당 유니티 좌표계의 1m 를 의미합니다.**

아래의 그림과 유사한 단지 구성이 되어야 합니다. 평형별 색상은 고려하지 않으셔도 됩니다.

![결과1](https://i.imgur.com/vcmmiid.png)



### 문제2) 문제1의 결과를 바탕으로 생성된 메쉬에 텍스쳐를 매핑합니다.(60점)

텍스쳐의 위치는 아래와 같습니다.

Assets/Samples/texture/buildingTester_d.png

텍스쳐는 아래와 같이 구성되어 있습니다.
![Texture](https://i.imgur.com/srIPHBq.png)


텍스쳐 매핑은 다음과 같습니다.

1번(앞) : 정점의 normal과 Vector3.forward의 각도가 y축을 기준으로 180 <= n && n <= 220 에 해당하는 경우

2번 : 1, 3을 제외한 모든 경우

3번(위, 아래) : 정점의 normal이 y축을 기준으로 Vector3.up || Vector3.down과 일치하는 경우


텍스쳐 파일을 자르지 않고 한장으로 그려야합니다. 

**한 층을 3m 로 가정하여 Material 의 SetTextureScale 값을 조정해주세요.**

**ex) 한 동의 높이가 10m 인 경우 = 10 / 3 = 3.3333 = 3층**

**Material.SetTextureScale("_BaseMap", new Vector2(1f, 3));**

아래의 그림과 비슷한 모양이 나와야합니다. 실 데이터와 차이가 있으므로 정확하게 일치하지는 않습니다.

![결과2](https://i.imgur.com/gbjA93E.png)

#### 평가기준

1. <b>아파트 단지에 텍스쳐 맵핑을 적용 (10점)

2. 주어진 텍스쳐 이미지를 문제에서 제시한 영역만큼 구분하여 적용 (20점)
   * 정점의 uv 좌표를 이용하여 구현해야 합니다. 텍스쳐 파일 나누어 구현하는 것은 인정되지 않습니다.

3. 적용된 텍스쳐를 문제에 제시한 방향 조건에 맞추어 적용 (30점)</b>

### 코드 설계시 우대되는 부분

유니티 라이프 사이클 함수 (Awake, Start, OnEnable, OnDisable, OnDestroy, Update) 이외의 직접 라이플 사이클을 구현 <b>(점수 획득한 문제에 가산점 +20점)</b>

MyFramework.cs 파일에 GameObject / Material 등의 리소스 생성 및 삭제가 구현되어야 합니다.

MyFramework.cs 파일에만 유니티 라이프 사이클 함수를 사용합니다.

리소스 생성 및 삭제의 순서가 명확해야합니다.

# 문제 해결 방법 (자유롭게 작성)

각 파일과 코드 항목은 로직별로 세부 주석을 달았습니다.
먼저 구현 방법을 간략히 다음과 같이 정리 하겠습니다. 

JsonMeshImpoter.cs : 샘플 데이터를 Json Deserialize하는 static Utility클래스 입니다.
BuildingData.cs : 샘플 데이터 로드를 통해 건물 Mesh 정보 및 Meta 데이터를 구성하는 클래스 입니다.
BuildingRenderer.cs : BuildingData 의 메쉬 정보를 이용헤 Unity3D의 Renderer Component를 구성하는 MonoBehaviour 클래스 입니다.
MyFramework.cs :
 Start() : 초기화 함수 OnInitilize()를 호출, 비동기함수인 Initialize 를 호출한다. 리소스를(샘플데이터, 텍스쳐)로딩 후, GenerateBuildingObjects를 통해 실제 Scene에 그려질 오브젝트를 구성한다.
 Update() : Initilize 과정이 끝나고 isInitialized = true 상태가 되면 구성된 buildingRenderers (BuildingRenderer.OnUpdate()를 차례로 실행
 
 
 문제를 해결하는데 주어진 정보 만으로는 예시에 가까운 형태를 정확히 매핑하기는 힘든 상황이었습니다. 
 그래서 문제의 조건에만 집중을 하였습니다.
 예시처럼 정확하게 방향에 따른 매핑을 하려면 몇가지 조건들이 더 필요했는데, (예를 들면) 건물 데이터 별로 넓은 면쪽을 가르키는 Local Axis가 하나씩 샘플 데이터에 있다던지 등.)
 만약 그랬다면 정점 노멀 방향 체크를 통해 매핑을 하면 조금더 예시 이미지에 가깝게 결과를 낼 수 있을 것이라 판단되었습니다.
 
 결과물은 주어진 조건에만 맞추어 실행되도록 구현 하였습니다.
 
 또한 
 주어진 샘플 데이터 만으로 조금 더 예시와 가깝게 보여질 수 있도록 (문제조건 외) 구현한 로직이 코드 안에 주석 처리 되어있고 설명도 포함하였습니다. 
 (넓은 면을 최대한 검사하는 로직입니다. 샘플에서 제공된 정점 형태나 건물별 정점 구성 방법이 서로 상이 함에 따라 정확한 적용은 아닐 수 있습니다)
 
 
 
