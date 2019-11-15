//
// TrenoIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "TrenoIntent.h"

#if __has_include(<Intents/Intents.h>) && (!TARGET_OS_OSX || TARGET_OS_IOSMAC) && !TARGET_OS_TV

@implementation TrenoIntent

@end

@interface TrenoIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) TrenoIntentResponseCode code;

@end

@implementation TrenoIntentResponse

@synthesize code = _code;

@dynamic citta, ora;

- (instancetype)initWithCode:(TrenoIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithCitta:(NSString *)citta ora:(NSString *)ora {
    TrenoIntentResponse *intentResponse = [[TrenoIntentResponse alloc] initWithCode:TrenoIntentResponseCodeSuccess userActivity:nil];
    intentResponse.citta = citta;
    intentResponse.ora = ora;
    return intentResponse;
}

@end

#endif
