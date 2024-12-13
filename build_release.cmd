set VS_PATH=C:\Program Files\Microsoft Visual Studio\2022\Enterprise
set CMAKE_PATH=C:\devel\CMake\bin
set PATH=%CMAKE_PATH%;%PATH%

@call "%VS_PATH%\Common7\Tools\vsdevcmd.bat" -no_ext -winsdk=none
mkdir build-release
cd build-release
cmake -G "Visual Studio 17 2022" ..
cmake --build . --config Release
