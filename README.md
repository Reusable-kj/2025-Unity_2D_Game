# 2025-Unity_2D_Game
This repository contains a 2D side-scrolling game created as a team project for a university academic exhibition. The game was implemented at a low level, with core systems such as rendering, input handling, and game logic built from scratch.

## Repository Note
This repository includes only the final version of the project.
During development, collaboration and version control were handled using Unity's internal branching system(Unity Version Control). As a result, the full commit history and intermediate branches are not reflected in this repository.

<유니티 입문의 개발 도전기>
1. 주요 기술적 도전 및 문제 해결 과정

1) 씬(Scene) 분할을 통한 구조 및 메모리 최적화

[문제점]: 초기에는 하나의 씬에서 게임의 모든 진행 과정을 처리하도록 설계했습니다. 이로 인해 시작/엔딩 씬 및 맵별 고유 기믹을 개별적으로 구현하기 어려웠고, 씬 로드 시 과도한 데이터가 메모리에 적재되어 로딩 지연 및 메모리 관리 문제(오브젝트의 잦은 생성 및 파괴로 인한 과부하)가 발생했습니다.

[해결 과정]: 목적과 기능에 따라 씬을 여러 개로 분리하여 오브젝트의 생명 주기를 효율적으로 관리하도록 구조를 변경했습니다. 또한, 카메라 객체를 싱글톤으로 고정하고 좌표 기반으로 이동하도록 수정하여 보스 맵 전환 시 발생하던 충돌 버그를 해결했습니다.

2) 컴포넌트 구조의 이해 및 예외 처리

[문제점]: 스크립트 작성 후 프리팹(Prefab)에 컴포넌트를 할당하지 않거나, 인스펙터 상에서 초기값(공격력, 유지 시간 등)을 설정하지 않아 NullReferenceException이 빈번하게 발생했습니다.

[해결 과정]: 데이터와 로직을 분리하는 유니티의 컴포넌트 기반 구조를 적용했습니다. [SerializeField] 속성을 활용해 인스펙터에서 직관적으로 데이터를 할당하고 참조 객체를 연결함으로써 안전하고 효율적인 워크플로우를 구축했습니다.

3) 플레이어 데이터 유지를 위한 싱글톤(Singleton) 패턴 도입

[문제점]: 플레이어의 핵심 데이터(인벤토리, 체력 등)가 씬 전환 시마다 초기화되는 문제가 발생했습니다.

[해결 과정]: 싱글톤 패턴을 도입하여 단일 인스턴스를 보장했습니다. 이를 통해 씬이 변경되어도 플레이어 데이터가 유지되며, 전역에서 접근 가능한 중앙 집중형 데이터 관리 구조를 설계했습니다.

4) 생명 주기(Life Cycle)를 활용한 DDOL 객체 위치 동기화

[문제점]: 플레이어 객체에 DontDestroyOnLoad(DDOL)를 적용하여 데이터는 유지했으나, 게임 재시작 시 이전 씬의 마지막 위치에 캐릭터가 그대로 남아있는 현상이 발생했습니다.

[해결 과정]: SceneManager.sceneLoaded 이벤트를 구독하여 새로운 씬이 로드될 때마다 플레이어의 좌표를 시작 지점으로 강제 초기화하는 로직을 구현하여 문제를 해결했습니다.

5) 유지보수성 향상을 위한 컴포넌트 기반 설계(CBD)

[문제점]: 초기 플레이어 스크립트에 모든 기능을 통합하여 구현하다 보니 코드가 비대해지고 유지보수가 어려운 스파게티 코드가 되었습니다.

[해결 과정]: 피격(PlayerDamageReceiver), 이동(Movement), 체력 관리(PlayerInventory) 등 기능별로 스크립트를 모듈화했습니다. 특정 기능에 오류가 발생해도 해당 컴포넌트만 수정하면 되도록 유지보수성을 크게 개선했습니다.

6) Branch 형상 관리 및 충돌(Conflict) 해결 프로세스 정립

[문제점]: 버전 관리를 통한 협업 과정에서 동일한 파일을 수정하여 발생하는 병합 충돌(Merge Conflict) 문제가 잦았습니다.

[해결 과정]: 유니티 스크립트의 특성상 로직 순서가 실행 결과에 영향을 미치므로, 병합 도구를 활용해 코드 변경 사항을 수동으로 검토 후 병합(Manual Merge)했습니다. 또한, Push 전 팀원 간 작업 중인 브랜치와 수정 내용을 교차 검증하는 프로세스를 생활화했습니다.

2. 주요 기능별 로직 구현 및 트러블 슈팅

1) 중앙 집중식 데미지 관리 시스템

파일: PlayerDamageReceiver.cs

핵심 함수: public void TakeDamage(int damageAmount)

해결한 문제: 다수의 공격체(투사체, 트랩 등)가 각각 플레이어 체력을 차감하면서 발생하던 로직 중복 및 무적 시간 무시 문제를 해결했습니다.

구현 내용: 모든 피격 처리를 해당 함수로 일원화했습니다. 함수 내부에서 무적 상태(canTakeDamage)를 선행 검사한 후 체력을 감소시키고 무적 코루틴을 실행하도록 안전하게 설계했습니다.

2) DDOL 플레이어의 씬 로드 시 위치 초기화

파일: Movement.cs

핵심 함수: private void OnSceneLoaded(Scene scene, LoadSceneMode mode)

해결한 문제: DontDestroyOnLoad 처리된 플레이어가 재시작 시에도 이전 사망 위치에 남아있는 현상을 해결했습니다.

구현 내용: SceneManager.sceneLoaded 이벤트를 구독했습니다. 재시작 플래그(IsRestarting)가 활성화된 상태로 씬이 로드되면, 플레이어의 좌표(transform.position)를 스폰 지점으로 강제 이동시키고 물리 속도(Rigidbody2D.velocity)를 초기화했습니다.

3) 유연한 재시작 상태 관리

파일: PlayerHealthWatcher.cs, RestartButton.cs

핵심 함수: public void OnClickRestart(), public void ResetDeadFlag()

해결한 문제: 사망 후 재시작 시 사망 플래그(isDead)가 해제되지 않아 조작이 불가능하거나 UI가 비정상적으로 노출되는 문제를 해결했습니다.

구현 내용: 재시작 버튼 클릭 시 이동 플래그(IsRestarting)를 활성화하고, ResetDeadFlag()를 호출해 사망 판정 상태를 초기화했습니다. 이와 동시에 비활성화된 인벤토리 UI를 복구하여 정상적인 게임 흐름을 재개하도록 구현했습니다.

4) 사운드 연출의 타이밍 제어

파일: BossSceneLoad.cs, BossController.cs

핵심 함수: IEnumerator MoveToBossScene(), public void PlayEntranceSound()

해결한 문제: 보스 씬 진입 시 시각적 연출과 사운드의 싱크가 어긋나고, 사운드 재생 관리 주체가 불분명한 문제를 해결했습니다.

구현 내용: 시네마틱 연출용 코루틴 내에서 카메라 이동 완료 후 yield return new WaitForSeconds(1.5f)를 통해 딜레이를 부여했습니다. 이후 BossController에 위임된 사운드 재생 함수를 호출하여 시각적 연출과 청각적 효과를 동기화했습니다.

5) 불꽃 잔상의 자동 소멸 및 1회성 피해

파일: Fireball.cs, FireTrap.cs

핵심 함수: void SpawnTrap(), void OnTriggerEnter2D(Collider2D other)

해결한 문제: 공격 잔상이 맵에 영구적으로 남아 메모리를 점유하거나, 플레이어가 장판 위에 있을 때 프레임당 무한정 데미지를 입는 문제를 해결했습니다.

구현 내용: Destroy(gameObject, duration)을 사용해 객체의 자동 소멸 주기를 설정했습니다. 또한, OnTriggerEnter2D 이벤트에서만 데미지를 가하도록 설계하여 무적 시스템과 연계된 1회성 타격만 적용되도록 최적화했습니다.
