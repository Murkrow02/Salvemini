//
//  IntentHandler.m
//  Intent
//
//  Created by Valerio Pio De Nicola on 29/09/2019.
//  Copyright © 2019 Codex. All rights reserved.
//

#import "IntentHandler.h"

@interface IntentHandler ()

@end

@implementation IntentHandler

- (id)handlerForIntent:(INIntent *)intent {
    // This is the default implementation.  If you want different objects to handle different intents,
    // you can override this and return the handler you want for that particular intent.
    
    return self;
}

@end
