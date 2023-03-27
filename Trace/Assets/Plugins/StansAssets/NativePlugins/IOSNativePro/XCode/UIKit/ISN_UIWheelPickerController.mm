#if !TARGET_OS_TV

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "ISN_Foundation.h"
#import "ISN_UICommunication.h"
#import "ISN_UIWheelPickerDelegate.h"

static UnityAction *WheelPickerCallback;

@interface ISN_UIWheelPickerController : NSObject
@property (nonatomic)  ISN_UIWheelPickerDelegate *m_pickerDelegate;
@property (nonatomic) UIView *inputView;
@end

@implementation ISN_UIWheelPickerController


//--------------------------------------
//  Initialization
//--------------------------------------


static ISN_UIWheelPickerController * s_sharedInstance;
+ (id)sharedInstance {
    if (s_sharedInstance == nil)  {
        s_sharedInstance = [[self alloc] init];
    }
    return s_sharedInstance;
}

-(id) init {
    self = [super init];
    if(self) {
        self.m_pickerDelegate = [[ISN_UIWheelPickerDelegate alloc] init];
    }
    return self;
}

- (void)disableTouchesOnView:(UIView *)view {
    UIButton *ghostButton = [[UIButton alloc] initWithFrame:CGRectMake(0, 0, view.frame.size.width, view.frame.size.height)];
    [ghostButton setBackgroundColor:[UIColor clearColor]];
    ghostButton.tag = 42; // Any random number. Use #define to avoid putting numbers in code.
    
    [view addSubview:ghostButton];
}


- (void) showWheelPicker: (ISN_UIWheelPickerRequest *) request
{
    UIViewController *vc =  UnityGetGLViewController();
    
    [self.m_pickerDelegate init:WheelPickerCallback data:request.m_Values];
    [self disableTouchesOnView:vc.view];
    
    UIPickerView *picker = [[UIPickerView alloc] init];
    
    picker.delegate = self.m_pickerDelegate;
    picker.dataSource = self.m_pickerDelegate;
    picker.showsSelectionIndicator = YES;
    [picker selectRow:0 inComponent:0 animated:YES];
    
    UIToolbar *toolBar = [[UIToolbar alloc] initWithFrame:CGRectMake(0, 0, [UIScreen mainScreen].bounds.size.width, 44)];
    [toolBar setBarStyle:UIBarStyleDefault];
    
    UIBarButtonItem *done = [[UIBarButtonItem alloc] initWithTitle:@"Done" style:UIBarButtonItemStylePlain target:self action:@selector(doneButton)];
    
    UIBarButtonItem *flex = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemFlexibleSpace target: self action:nil];
    
    UIBarButtonItem *cancel = [[UIBarButtonItem alloc] initWithTitle:@"Cancel" style:UIBarButtonItemStylePlain target:self action:@selector(cancelButton)];
    
    toolBar.items = @[cancel, flex, done];
    done.tintColor = [UIColor blackColor];
    
    [picker addSubview:toolBar];
    picker.backgroundColor = [UIColor whiteColor];
    self.inputView = [[UIView alloc] initWithFrame:CGRectMake(0,[UIScreen mainScreen].bounds.size.height-picker.frame.size.height-22, [UIScreen mainScreen].bounds.size.width, picker.frame.size.height + 44)];
    
    [picker setFrame:CGRectMake(0, 0, self.inputView.frame.size.width, self.inputView.frame.size.height)];
    
    [self.inputView addSubview:picker];
    [self.inputView addSubview:toolBar];
    
    [vc.view addSubview:self.inputView];
}

- (void) doneButton
{
    ISN_UIWheelPickerResult *result = [[ISN_UIWheelPickerResult alloc] init];
    result.m_Value = self.m_pickerDelegate.m_value;
    result.m_State = @"DONE";
    ISN_SendCallbackToUnity(WheelPickerCallback, [result toJSONString]);
    [self.inputView removeFromSuperview];
    
     UIViewController *vc =  UnityGetGLViewController();
     [[vc.view viewWithTag:42] removeFromSuperview];
}

- (void) cancelButton
{
    ISN_UIWheelPickerResult *result = [[ISN_UIWheelPickerResult alloc] init];
    result.m_Value = self.m_pickerDelegate.m_value;
    result.m_State = @"CANCELED";
    ISN_SendCallbackToUnity(WheelPickerCallback, [result toJSONString]);
    [self.inputView removeFromSuperview];
    
    UIViewController *vc =  UnityGetGLViewController();
    [[vc.view viewWithTag:42] removeFromSuperview];
}


@end

extern "C" {
    
    void _ISN_UIWheelPicker(UnityAction *callback, char* data)
    {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_UIWheelPicker" data:data];
        
        NSError *jsonError;
        ISN_UIWheelPickerRequest *request = [[ISN_UIWheelPickerRequest alloc] initWithChar:data error:&jsonError];
        if (jsonError) {
            [ISN_Logger LogError:@"_ISN_UIWheelPicker JSON parsing error: %@", jsonError.description];
        }
        WheelPickerCallback = callback;
        [[ISN_UIWheelPickerController sharedInstance] showWheelPicker:request];
    }
}

#endif
