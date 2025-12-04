#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C" {
    void _ShowSnackbar(const char* message, int duration) {
        NSString *msg = [NSString stringWithUTF8String:message];
        float displayDuration = (duration == 1) ? 3.5f : 2.0f;
        
        dispatch_async(dispatch_get_main_queue(), ^{
            UIWindow *window = nil;
            
            // For iOS 13+
            if (@available(iOS 13.0, *)) {
                for (UIWindowScene* windowScene in [UIApplication sharedApplication].connectedScenes) {
                    if (windowScene.activationState == UISceneActivationStateForegroundActive) {
                        window = windowScene.windows.firstObject;
                        break;
                    }
                }
            }
            
            // Fallback for older iOS versions
            if (window == nil) {
                window = [UIApplication sharedApplication].keyWindow;
            }
            
            if (window == nil) {
                NSLog(@"[SnackbarPlugin] Could not find window to display snackbar");
                return;
            }
            
            // Create label
            UILabel *label = [[UILabel alloc] init];
            label.text = msg;
            label.textColor = [UIColor whiteColor];
            label.backgroundColor = [[UIColor blackColor] colorWithAlphaComponent:0.85];
            label.textAlignment = NSTextAlignmentCenter;
            label.numberOfLines = 0;
            label.font = [UIFont systemFontOfSize:16];
            label.layer.cornerRadius = 10;
            label.clipsToBounds = YES;
            
            // Calculate size
            CGFloat padding = 20;
            CGFloat maxWidth = window.frame.size.width - 40;
            CGSize textSize = [msg boundingRectWithSize:CGSizeMake(maxWidth - padding * 2, CGFLOAT_MAX)
                                                options:NSStringDrawingUsesLineFragmentOrigin
                                             attributes:@{NSFontAttributeName: label.font}
                                                context:nil].size;
            
            CGFloat width = MIN(textSize.width + padding * 2, maxWidth);
            CGFloat height = MAX(50, textSize.height + padding);
            
            // Position at bottom
            label.frame = CGRectMake((window.frame.size.width - width) / 2,
                                    window.frame.size.height - height - 50,
                                    width,
                                    height);
            label.alpha = 0;
            
            [window addSubview:label];
            
            // Animate in
            [UIView animateWithDuration:0.3 animations:^{
                label.alpha = 1;
            } completion:^(BOOL finished) {
                // Animate out after delay
                [UIView animateWithDuration:0.3 
                                      delay:displayDuration 
                                    options:0 
                                 animations:^{
                    label.alpha = 0;
                } completion:^(BOOL finished) {
                    [label removeFromSuperview];
                }];
            }];
        });
    }
}