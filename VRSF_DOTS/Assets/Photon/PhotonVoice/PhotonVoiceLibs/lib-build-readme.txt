[Windows]
https://trac.ffmpeg.org/wiki/CompilationGuide/MSVC

git clone https://chromium.googlesource.com/webm/libvpx {VPX_PROJ_DIR}
git clone git://source.ffmpeg.org/ffmpeg.git {FFMPEG_PROJ_DIR}

# start vs command prompt
cd "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC"
vcvarsall.bat amd64
C:/MinGW/msys/1.0/msys.bat

# e.g. {MinGW} = D:/Dev/MinGW
export INCLUDE=$INCLUDE\;"/c/MinGW/include/"
export LIB=$LIB\;"/c/MinGW/lib/x64"
export PKG_CONFIG_PATH="/c/MinGW/msys/1.0/local/lib/pkgconfig"
*(install pkg-config)

(fix opus VS project: add celt/x86/vq_sse2.c to sources)
(build opus from VS project)
cp .../opus.lib {MinGW}/msys/1.0/local/lib/x64/
cp {OPUS_PROJ_DIR}/include/* {MinGW}/msys/1.0/local/include/

[vpx.lib]
cd {VPX_PROJ_DIR}
./configure --disable-examples --disable-unit-tests --disable-docs --enable-static-msvcrt --target=x86_64-win64-vs14 # x86_64-win64-gcc x86_64-win64-vs14 x86-win32-vs14
make
make install
(check vpx.lib name and location)

[vpx.dll (vpx w/o fmpeg)]
(after doing previous step w/o make install)
- open solution
- set project type to dll
- set vpx.def in Librarian/Module Definitiopn file
- build project

[libyuv dll (vpx w/o fmpeg)]
- install gyp
cd {libyuv_PROJ_DIR}
- edit libyuv.gyp:
       # Change type to 'shared_library' to build .so or .dll files.
-      'type': 'static_library',
+      'type': 'shared_library',
       # Allows libyuv.a redistributable library without external dependencies.
-      'standalone_static_library': 1,
+      'standalone_static_library': 0,
       'defines': [
+        'LIBYUV_BUILDING_SHARED_LIBRARY',
../gyp/gyp libyuv.gyp --depth . -D target_arch=whatever
- open solution, add x64 platform and build

cd {FFMPEG_PROJ_DIR}
# (? patch /libavutil/hwcontext_dxva2.c: replace GetDesktopWindow() with 0)
# (or add --disable-dxva2 to command below)
# (--disable-decoder=opus - switch to libopus w/o AV_CODEC_CAP_DELAY capability)
./configure --enable-asm --enable-yasm --disable-avdevice --disable-doc --disable-ffplay --disable-ffprobe --disable-ffmpeg --enable-shared --disable-static --disable-bzlib --disable-libopenjpeg --disable-iconv --disable-zlib --prefix=/c/ffmpeg --toolchain=msvc --extra-ldflags="/NODEFAULTLIB:libcmt" --enable-libopenh264 --enable-libvpx --enable-libopus --arch=amd64 --enable-debug --extra-cflags="-MTd" --disable-dxva2 --disable-avformat --disable-avfilter
make

[Android on Ubuntu]
(install Android NDK)
(? install: sudo apt-get --quiet --yes install build-essential git autoconf libtool pkg-config gperf gettext yasm python-lxml)
export NDK={NDK-DIR}
git clone https://chromium.googlesource.com/webm/libvpx {VPX_PROJ_DIR}
cd {VPX_PROJ_DIR}
./configure --target=armv7-android-gcc --disable-examples --sdk-path=/home/vadim/Dev/android-ndk/ --disable-runtime-cpu-detect --extra-cflags="-mfloat-abi=softfp -mfpu=neon" --enable-error-concealment --disable-docs

make
cp -rf libvpx.a $NDK/platforms/android-24/arch-arm/usr/lib/
cp -rf vpx/ $NDK/platforms/android-24/arch-arm/usr/include/
cd ..
mkdir ffmpeg-build
cp {.}/build-ffmpeg-android.sh .
git clone git://source.ffmpeg.org/ffmpeg.git {FFMPEG_PROJ_DIR}
patch ffmpeg/configure
============
@@ -3216,10 +3216,10 @@
 SLIBSUF=".so"
 SLIBNAME='$(SLIBPREF)$(FULLNAME)$(SLIBSUF)'
 SLIBNAME_WITH_VERSION='$(SLIBNAME).$(LIBVERSION)'
-SLIBNAME_WITH_MAJOR='$(SLIBNAME).$(LIBMAJOR)'
+SLIBNAME_WITH_MAJOR='$(SLIBPREF)$(FULLNAME)-$(LIBMAJOR)$(SLIBSUF)'
 LIB_INSTALL_EXTRA_CMD='$$(RANLIB) "$(LIBDIR)/$(LIBNAME)"'
-SLIB_INSTALL_NAME='$(SLIBNAME_WITH_VERSION)'
-SLIB_INSTALL_LINKS='$(SLIBNAME_WITH_MAJOR) $(SLIBNAME)'
+SLIB_INSTALL_NAME='$(SLIBNAME_WITH_MAJOR)'
+SLIB_INSTALL_LINKS='$(SLIBNAME)'
 VERSION_SCRIPT_POSTPROCESS_CMD="cat"
============
(paste content above, Ctrl-D)
./ffmpeg-build-android.sh
(check build/ffmpeg/armeabi-v7a/lib)
(ffmpeg build script is based on http://bambuser.com/r/opensource/ffmpeg-4f7d2fe-android-2011-03-07.tar.gz at http://bambuser.com/opensource)

[iOS]
https://github.com/kewlbear/FFmpeg-iOS-build-script


[Windows]
*(install pkg-config)
go to http://ftp.gnome.org/pub/gnome/binaries/win32/dependencies/
download the file pkg-config_0.26-1_win32.zip
extract the file bin/pkg-config.exe to C:\MinGW\bin
download the file gettext-runtime_0.18.1.1-2_win32.zip
extract the file bin/intl.dll to C:\MinGW\bin
go to http://ftp.gnome.org/pub/gnome/binaries/win32/glib/2.28
download the file glib_2.28.8-1_win32.zip
extract the file bin/libglib-2.0-0.dll to C:\MinGW\bin
http://stackoverflow.com/questions/1710922/how-to-install-pkg-config-in-windows

	[VPx Android shared (+static)] (remove all files but Android.mk in jni folder before switching target!)
(install Android NDK)
mkdir jni
cd jni
{clone}
Create Android.mk
===
LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)
include libvpx/build/make/Android.mk
===
	[ARM]
./libvpx/configure --target=armv7-android-gcc --sdk-path=/home/vadim/Dev/android-ndk/ --disable-runtime-cpu-detect --extra-cflags="-mfloat-abi=softfp -mfpu=neon" --enable-error-concealment --disable-docs --disable-examples --disable-tools
/// Shared lib is not suported. We can either hack mk file (does not work for x86{_64}) ...
// Enable line in libvpx/build/make/Android.mk:
// include $(BUILD_SHARED_LIBRARY)
// <NDK>/ndk-build TARGET_ARCH_ABI=armeabi-v7a
/// ... or convert static libarary to shared (choose proper toolchain):
<NDK>/ndk-build TARGET_ARCH_ABI=armeabi-v7a
pushd ../obj/local/armeabi-v7a
<NDK>/toolchains/arm-linux-androideabi-4.9/prebuilt/linux-x86_64/bin/arm-linux-androideabi-gcc -shared -o libvpx.so --sysroot=/home/vadim/Dev/android-ndk/platforms/android-9/arch-arm -Wl,--whole-archive libvpx.a -lc -lm -Wl,--no-whole-archive
<NDK>/toolchains/arm-linux-androideabi-4.9/prebuilt/linux-x86_64/bin/arm-linux-androideabi-strip libvpx.so
	[x86{_64}]
./libvpx/configure --target=x86{_64}-android-gcc --sdk-path=/home/vadim/Dev/android-ndk/ --disable-runtime-cpu-detect --extra-cflags="-fvisibility=protected" --enable-pic --enable-error-concealment --disable-docs --disable-examples --disable-tools --disable-ssse3 --disable-sse4_1 --disable-avx2
<NDK>/ndk-build TARGET_ARCH_ABI=x86{_64}
pushd ../obj/local/x86{_64}
<NDK>/toolchains/x86_64-4.9/prebuilt/linux-x86_64/bin/x86_64-linux-android-gcc -shared -o libvpx.so --sysroot=/home/vadim/Dev/android-ndk/platforms/android-21/arch-x86{_64} -Wl,--whole-archive libvpx.a -lc -lm -Wl,--no-whole-archive
<NDK>/toolchains/x86_64-4.9/prebuilt/linux-x86_64/bin/x86_64-linux-android-strip libvpx.so

[VPX]
[Prerequisites]
git clone https://chromium.googlesource.com/webm/libvpx
git fetch
//git fetch --tags ?
git checkout v1.8.0

[Win32/Linux+Windows]
Ubuntu machine:
mkdir build
cd build
../libvpx/configure --disable-examples --disable-unit-tests --disable-docs --target=x86_64-win64-vs15
make
Windows machine:
#copy 'build' from Ubuntu
cd build 
open vpx.sln
Update project properties: output name and type (dll)

[iOS/MacOS]
cd libvpx/build/make
Comment out devnull='> /dev/null 2>&1' in iosbuild.sh
./iosbuild.sh
 


https://github.com/jb-alvarado/media-autobuild_suite
https://ffmpeg.zeranoe.com/builds/