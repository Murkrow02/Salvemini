XBUILD=xcodebuild
PROJECT_ROOT=./
PROJECT=$(PROJECT_ROOT)/TrainIntentStaticLib.xcodeproj
TARGET= TrainIntentStaticLib

all: bind

lib$(TARGET)-i386.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

lib$(TARGET)-arm64.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch arm64 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

lib$(TARGET).a: lib$(TARGET)-i386.a lib$(TARGET)-arm64.a
	xcrun -sdk iphoneos lipo -create -output $@ $^

bind: lib$(TARGET).a
	sharpie bind -scope TrainIntentStaticLib -output bo -namespace SalveminiApp -sdk iphoneos$(shell xcrun --show-sdk-platform-version --sdk iphoneos) TrainIntentStaticLib/TrainIntent.h

clean:
	-rm -rf bo build *.a *.dll
