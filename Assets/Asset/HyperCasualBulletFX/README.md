# Hyper Casual Bullet FX

Unity `6000.3.17f1`용 저비용 시각 전용 총알 이펙트입니다. 총구 섬광, 여러 갈래의 tracer, 피격 spark를 표현합니다. 게임 판정과 독립적으로 사용할 수 있습니다.

## 설치

### 권장: Package Manager에서 로컬 패키지 추가

1. 이 폴더를 팀 프로젝트 밖의 유지할 위치에 둡니다.
2. Unity에서 `Window > Package Management > Package Manager`를 엽니다.
3. 좌측 상단 `+ > Install package from disk...`를 선택합니다.
4. 이 폴더의 `package.json`을 선택합니다.

또는 `HyperCasualBulletFX` 폴더 전체를 프로젝트의 `Packages/com.gptasset.hypercasual-bullet-fx`로 복사해도 됩니다.

## 사용

1. 빈 GameObject에 `HyperCasualBulletFx`를 추가합니다.
2. 발사할 때 위치와 시각적 진행 방향만 전달합니다.

```csharp
bulletFx.Play(muzzlePosition, fireDirection, visualDistance: 25f);
```

피격 연출이 필요한 위치에서는 별도로 호출합니다.

```csharp
bulletFx.PlayImpact(hitPosition, surfaceNormal);
```

두 API는 서로 독립적입니다. `Play`의 첫 번째 tracer는 입력 방향과 일치합니다. 나머지는 같은 방향 주변으로 시각적으로 퍼져 실제 발사 수보다 많은 탄환처럼 보입니다. `PlayImpact`는 전달받은 위치에서 spark만 재생합니다.

## 주요 설정

| 항목 | 의미 | 권장 범위 |
|---|---|---|
| Pool Size | 동시에 재생 가능한 발사 수 | 12–30 |
| Visual Bullet Count | 한 번 재생할 때 보이는 tracer 수 | 4–8 |
| Tracer Speed | tracer의 화면 이동 속도 | 90–160 |
| Tracer Length | 밝은 선분의 길이 | 1.5–3.0 |
| Visual Spread | 보조 tracer의 끝점 분산 | 0.1–0.35 |
| Impact Spark Count | 명중 시 시각 spark 수 | 3–6 |

## 성능 특성

- 초기화 후 발사 시 `Instantiate`/`Destroy`를 호출하지 않습니다.
- 시각 효과는 물리 질의나 충돌 계산을 전혀 수행하지 않습니다.
- 모든 LineRenderer는 고정 크기 pool에서 재사용합니다.
- pool을 모두 사용하면 가장 오래된 slot을 즉시 재사용합니다.
- 런타임에 URP Unlit shader를 우선 선택하고 Built-in에서는 `Sprites/Default`로 fallback합니다.

모바일에서는 `Pool Size 12`, `Visual Bullet Count 4`, `Impact Spark Count 3`부터 시작하는 것을 권장합니다. 화면에 동시에 보이는 LineRenderer가 많으면 draw call과 CPU 갱신 비용이 증가하므로 실제 기기에서 Profiler로 확인하세요.

## 샘플

Package Manager에서 이 패키지의 `Samples > Quick Start`를 Import하면 순수 시각 효과 호출 예제를 확인할 수 있습니다. 새 Input System을 강제 의존하지 않기 위해 샘플은 기존 `Input` API만 사용합니다.
