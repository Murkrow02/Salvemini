//
// TrainIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "TrainIntent.h"

#if __has_include(<Intents/Intents.h>) && (!TARGET_OS_OSX || TARGET_OS_IOSMAC) && !TARGET_OS_TV

@implementation TrainIntent

@end

@interface TrainIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) TrainIntentResponseCode code;

@end

@implementation TrainIntentResponse

@synthesize code = _code;

@dynamic citta, ora;

- (instancetype)initWithCode:(TrainIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithCitta:(NSString *)citta ora:(NSString *)ora {
    TrainIntentResponse *intentResponse = [[TrainIntentResponse alloc] initWithCode:TrainIntentResponseCodeSuccess userActivity:nil];
    intentResponse.citta = citta;
    intentResponse.ora = ora;
    return intentResponse;
}

@end

#endif
