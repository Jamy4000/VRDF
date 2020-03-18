/*
 
 Copyright (C) 2016 Apple Inc. All Rights Reserved.
 See LICENSE.txt for this sample’s licensing information
 
 Abstract:
 This class demonstrates the audio APIs used to capture audio data from the microphone and play it out to the speaker. It also demonstrates how to play system sounds
 
 */

#import <AudioToolbox/AudioToolbox.h>
#import <AVFoundation/AVFoundation.h>

extern "C" {
    typedef void (*Photon_IOSAudio_PushCallback)(int, float*, int);
}

@interface Photon_Audio_In : NSObject {
}

@property (nonatomic, assign, readonly) BOOL audioChainIsBeingReconstructed;

- (OSStatus)    startIOUnit;
- (OSStatus)    stopIOUnit;
- (double)      sessionSampleRate;

@end

extern "C" {
    Photon_Audio_In* Photon_Audio_In_CreateReader(int sessionCategory, int sessionMode, int sessionCategoryOptions);
    bool Photon_Audio_In_Read(Photon_Audio_In* handle, float* buf, int len);
    
    Photon_Audio_In* Photon_Audio_In_CreatePusher(int hostID, Photon_IOSAudio_PushCallback pushCallback,  int sessionCategory, int sessionMode, int sessionCategoryOptions);
    
    void Photon_Audio_In_Destroy(Photon_Audio_In* handle);
}
