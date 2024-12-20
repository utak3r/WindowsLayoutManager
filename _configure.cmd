set VS_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise
set CMAKE_PATH=C:\devel\CMake\bin
@call _set_user_paths.cmd
set PATH=%CMAKE_PATH%;%PATH%

@call "%VS_PATH%\Common7\Tools\vsdevcmd.bat" -no_ext -winsdk=none %*

mkdir build-mvs22
cd build-mvs22
cmake -G "Visual Studio 17 2022" -A x64 ..
