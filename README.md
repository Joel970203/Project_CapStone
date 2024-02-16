# Project_CapStone
![Capstone](https://github.com/Joel970203/Project_CapStone/assets/121085543/d30ebcb6-4f28-4659-b33f-86cdded075a8)

게임이름 : Project_CapStone

개발 환경 : Unity 
</br>

개발 언어 : C# , ShaderLab , HLSL
</br>


개발 기간 : 3개월 
</br>


개발 동기 : 
1. 보스레이드 게임을 유니티로 구현하고자 함.
2. 소스코드를 복사하는 것 만으로는, 의미가 없다고 판단하여 직접 쉐이더를 구현함.
3. 멀티 게임의 기초중 기초인 동기화 개념에 대해 정확히 이해하고, 모든 플레이어에게 동기화 되도록 구현하고자 함.



<h3 align="center">🥇 멤버 구성  🥇 </h3>

  ### [나영빈](https://github.com/Rubbe1124)
    업무 : 서버 접속, 로비, 대기실 기능 구현. 씬 로딩 및 오브젝트 동기화 진행. 캐릭터 기능 개발. 
  ### [이충민](https://github.com/1CM98)
    업무 : 보스 행동 패턴 개발, 아이템 시스템 개발, 커스텀 쉐이더 개발 후 지형 및 보스 등에 적용.
  ### [한상혁](https://github.com/Joel970203)
    업무 : 맵 디자인 및 캐릭터 기능 개발. 캐릭터 이동 및 스킬 동기화 진행. 보스 패턴 동기화 진행.
    


</br>


</br>


<h3 align="center">🔑 주요 기술 🔑  </h3>

![그림2](https://github.com/Joel970203/Project_CapStone/assets/121085543/8f3a74d7-3f27-4ea2-a542-628eb917fe74)

<div>
네트워크 - 로비 시스템
- 포톤 엔진의 PUN2 라이브러리
- 서버를 경유한 데이터를 가져와 화면에 출력하는 것을 목표로 함

기능
-서버 접속 및 로비 리스트 출력
-마우스 클릭을 통한 간단한 방 생성 및 참여 기능
</div>

![그림3](https://github.com/Joel970203/Project_CapStone/assets/121085543/3190fe76-8cdd-40ca-acd3-29e06ad60450)
네트워크 – 대기실
-캐릭터 선택 및 준비 완료 설정 여부

기능
-캐릭터 선택 및 중복 방지 설정
-유저 상태에 따른 권한 부여
-태그를 통한 유저 정보 공유 및 씬 로드, 동기화 준비
![그림4](https://github.com/Joel970203/Project_CapStone/assets/121085543/c94494a7-d93c-4149-9193-060320273fe8)


네트워크 ㅡ 동기화
기능 ㅡ   플레이어,보스,아이템,이펙트 등 실시간으로 처리되는 모든 효과와 게임내 정보를 모든 유저에게 실시간으로 동기화.
![그림5](https://github.com/Joel970203/Project_CapStone/assets/121085543/db9f1e70-e33c-4fb7-b6b3-f96cc6e3fddb)


쉐이더 
물과 용암의 공통 특징을 잡아 픽셀 쉐이더에서 보로노이 텍스처를 만들고 
변형을 거쳐 용암과 물을 하나의 쉐이더로 표현  
정점의 위치에 동적인 변화를 주어 상하 출렁임을 구현
C# 스크립트를 통해 해당 쉐이더를 조작하여 지형이 변화할때 색상 변화를 주고
용암일 때는 출렁임이 적고 물일 때는 빠르게 변화 시킴

![그림7](https://github.com/Joel970203/Project_CapStone/assets/121085543/24ca078f-a33a-4f96-a04f-af81b652f31c)

![그림8](https://github.com/Joel970203/Project_CapStone/assets/121085543/c836e2a8-6d7f-4013-bb99-4872e369257f)

선형 보간을 이용한 텍스처 혼합을 통해 지형과 몬스터의 외형 변화 구현

![그림10](https://github.com/Joel970203/Project_CapStone/assets/121085543/6b4a4b12-5546-4945-861f-5dad6ae20816)
![그림11](https://github.com/Joel970203/Project_CapStone/assets/121085543/e3e78a33-7cec-4029-a2fa-53cb36377ec7)

노멀 벡터, 방향벡터 값과 굴절률을 통해 굴절 벡터를 계산하고 
스크린 UV와 더하여 
굴절 벡터를 구현 
![그림12](https://github.com/Joel970203/Project_CapStone/assets/121085543/fb18524e-c63c-4e47-adde-1f86e530b104)

</br>

설계서 : 
</br>


다운로드 : 
