//
// OrarioIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "OrarioIntent.h"

#if __has_include(<Intents/Intents.h>) && (!TARGET_OS_OSX || TARGET_OS_IOSMAC) && !TARGET_OS_TV

@implementation OrarioIntent

@end

@interface OrarioIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) OrarioIntentResponseCode code;

@end

@implementation OrarioIntentResponse

@synthesize code = _code;

@dynamic giorno, lezioni;

- (instancetype)initWithCode:(OrarioIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithGiorno:(NSString *)giorno lezioni:(NSString *)lezioni {
    OrarioIntentResponse *intentResponse = [[OrarioIntentResponse alloc] initWithCode:OrarioIntentResponseCodeSuccess userActivity:nil];
    intentResponse.giorno = giorno;
    intentResponse.lezioni = lezioni;
    return intentResponse;
}

+ (instancetype)failureIntentResponseWithGiorno:(NSString *)giorno {
    OrarioIntentResponse *intentResponse = [[OrarioIntentResponse alloc] initWithCode:OrarioIntentResponseCodeFailure userActivity:nil];
    intentResponse.giorno = giorno;
    return intentResponse;
}

@end

#endif
