cmake_minimum_required (VERSION 3.30)

project (
			u3WindowsManagerApp
			DESCRIPTION "Windows Layout Manager Application"
			LANGUAGES CSharp
		)


include (CSharpUtilities)
set (CMAKE_CSharp_FLAGS "/langversion:latest")
set (CMAKE_CSharp_FLAGS "/platform:anycpu")

set (VERSION_MAJOR 1)
set (VERSION_MINOR 0)
set (VERSION_REVISION 0)
set (VERSION_BUILD 1)

add_subdirectory(src)
