//
//  D2mIOS.m
//  iosplugin
//
//  Created by Nam Ngo Hoang on 7/24/19.
//  Copyright © 2019 D2M. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface D2mIOS : NSObject
{
}
@end

@implementation D2mIOS

static D2mIOS *_sharedInstance;

+(D2mIOS*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        NSLog(@"Creating D2M Plugin");
        _sharedInstance = [[D2mIOS alloc] init];
    });
    
    return _sharedInstance;
}

-(id)init
{
    self = [super init];
    if(self)
        [self initHelper];
    return self;
}

-(void)initHelper
{
    NSLog(@"D2M iOS: initHelper");
}

- (BOOL) isSchemeAvailable:(NSString*) url
{
    NSString* str = [url stringByAppendingString:@"://"];
    
    if([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:str]])
    {
        NSLog(@"D2M iOS: isSchemeAvailable %@ -> Success", str);
        return true;
    }
    
    NSLog(@"D2M iOS: isSchemeAvailable %@ -> Failed", str);
    
    return false;
}

@end    

extern "C"
{
    bool IsSchemeAvailable(const char* urlScheme)
    {
        return [[D2mIOS sharedInstance] isSchemeAvailable:[NSString stringWithUTF8String:urlScheme]];
    }
}
