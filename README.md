# Troubleshooting
## 1. Unity3D, Gamebase 연동시 firebase 관련
   - firebase 에서 발급받은 google-services.json 파일은 예전이라면 Assets 폴더 내에만 있었어도 되는거 같은데..
     firebase 공식 문서에도 제대로 나와있지가 않다.
     
     어쨌든 결론은 .androidlib 확장자로 폴더를 만들고, 내부에 기본 AndroidManifest.xml, project.properties 파일을 넣고
     res/values 에 google-services.xml 파일을 넣는다.
     google-services.json 을 xml로 변환한 것.
     
     [firebase-cpp-sdk](https://github.com/firebase/firebase-cpp-sdk/tree/main)
     [unity-android-library-project-and-aar-plugins-introducing](https://docs.unity3d.com/kr/2022.3/Manual/android-library-project-and-aar-plugins-introducing.html)
