#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#ifdef __cplusplus
extern "C" {
#endif

    void _OpenURL(const char* url) {
        NSString* urlString = [NSString stringWithUTF8String:url];
        NSURL* nsUrl = [NSURL URLWithString:urlString];

        if (@available(iOS 10.0, *)) {
            [[UIApplication sharedApplication] openURL:nsUrl options:@{} completionHandler:^(BOOL success) {
                if (!success) {
                    NSLog(@"Failed to open URL: %@", urlString);
                }
            }];
        } else {
            BOOL success = [[UIApplication sharedApplication] openURL:nsUrl];
            if (!success) {
                NSLog(@"Failed to open URL: %@", urlString);
            }
        }
    }

#ifdef __cplusplus
}
#endif

/*
정확하지는 않지만 Unity 2022.x 버전 빌드에서 iOS 18버전 이상의 기기에서 Application.OpenURL이 정상동작하지 않는 문제가 있음.
해당 파일을 Assets/Plugins/iOS 경로에 추가 후 아래 코드로 _OpenURL 함수 정의 후 사용. 
#if UNITY_IOS
        // Assets/Plugins/iOS/OpenURLPlugin.m
        [DllImport("__Internal")]
        public static extern void _OpenURL(string url);
#endif
*/