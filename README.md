# Troubleshooting

## 1. Unity3D, Gamebase 연동시 firebase 관련

- firebase 에서 발급받은 google-services.json 파일은 예전이라면 Assets 폴더 내에만 있었어도 되는거 같은데..
  firebase 공식 문서에도 제대로 나와있지가 않다.

  어쨌든 결론은 .androidlib 확장자로 폴더를 만들고, 내부에 기본 AndroidManifest.xml, project.properties 파일을 넣고
  res/values 에 google-services.xml 파일을 넣는다.
  google-services.json 을 xml로 변환한 것.

  [firebase-cpp-sdk](https://github.com/firebase/firebase-cpp-sdk/tree/main)
  [unity-android-library-project-and-aar-plugins-introducing](https://docs.unity3d.com/kr/2022.3/Manual/android-library-project-and-aar-plugins-introducing.html)

##2. GooglePlay 내부테스트 버전으로 인앱 테스트 할 때 구입하려는 항목을 찾을 수 없습니다 해결.

- 올라가있는 appBundle 버전 코드와 빌드 apk의 버전 코드 확인
- 테스트 할 계정으로 내부 테스트 프로그램에 참여 했는지 (개별 링크)확인
- 테스트 및 출시 - 설정 - 고급 설정 에서 앱 이용 가능 여부가 출시 됨으로 되어있는지 확인.

##3. Unity Android 빌드 시 directory is not writable 오류
Failed to install the following SDK components:
build-tools;30.0.3 Android SDK Build-Tools 30.0.3
The SDK directory is not writable (C:\Program Files\Unity\Hub\Editor\2022.3.19f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK)

시스템 환경변수에 JAVA_OPTIONS : -Xmx512M 추가

##3. GooglePlay 로그인 시도 시 com.google.android.gms.common.api.ApiException: 10 이런 비슷한 오류 뜨는 경우.

- FingerPrint가 일치하지 않을 때 발생.
  작성 시점의 경우는 build 할 때 사용하는 인증서의 SHA-1 는 등록 해서 빌드머신에서 바로 apk를 뽑아서 실행 할 때는 동작했지만
  Google Play Console에 aab를 올릴 때 서명되는 앱 서명 키를 등록하지 않았었기 때문에
  Google Play Console의 내부테스트를 이용해 실행 할 때는 정상 동작하지 않았던 것.
  해결 방법

1. Google Cloud Console - API 및 서비스 접속.
2. 사용자 인증 정보 - 사용자 인증 정보 만들기 - OAuth 클라이언트 ID 생성
3. 애플리케이션 유형 : 안드로이드, Google Play Console 에 있는 앱 서명 인증키 SHA-1 등록.

Firebase를 사용 한다면 Firebase Console에도 등록 해야한다.
Firebase Console - 프로젝트 설정 - 디지털 지문 추가

##4. Rendering Debugger
https://docs.unity3d.com/kr/Packages/com.unity.render-pipelines.universal%4014.0/manual/features/rendering-debugger.html
UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
