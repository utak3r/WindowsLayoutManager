cmake_minimum_required (VERSION 3.31)

set (VERSION_MAJOR 1)
set (VERSION_MINOR 0)
set (VERSION_REVISION 1)
set (PRODUCT_AUTHORS "Piotr 'utak3r' Borys")
set (PRODUCT_COPYRIGHT "Piotr 'utak3r' Borys")

find_package(Git)
if(GIT_FOUND)
# Get the current working branch
execute_process(
  COMMAND ${GIT_EXECUTABLE} rev-parse --abbrev-ref HEAD
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE GIT_BRANCH
  OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Get the latest abbreviated commit hash of the working branch
execute_process(
  COMMAND ${GIT_EXECUTABLE} log -1 --format=%h
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE GIT_COMMIT_HASH
  OUTPUT_STRIP_TRAILING_WHITESPACE
)

# Get the number of commits since the commit_counter tag
# (which references the first commit in master branch)
execute_process(
  COMMAND ${GIT_EXECUTABLE} describe --match commit_counter
  WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
  OUTPUT_VARIABLE DESCRIBE_BUILD_COUNTER
  OUTPUT_STRIP_TRAILING_WHITESPACE
)
string(REGEX MATCH "[0-9]+" VERSION_BUILD ${DESCRIBE_BUILD_COUNTER})
endif()

# Get the current date and time
execute_process(
        COMMAND "cmd" " /c date /t"
        OUTPUT_VARIABLE BUILD_DATE
        OUTPUT_STRIP_TRAILING_WHITESPACE
)
execute_process(
        COMMAND "cmd" " /c echo %TIME%"
        OUTPUT_VARIABLE TIME
        OUTPUT_STRIP_TRAILING_WHITESPACE
)
string(REGEX REPLACE "[^0-9]*(..:..:..).*" "\\1"
        BUILD_TIME "${TIME}")

# Configure the Version.cs file
configure_file(
        Version.cs.in
        Version.cs
)


project (
			u3WindowsManagerApp
			VERSION ${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_REVISION}.${VERSION_BUILD}
			DESCRIPTION "Windows Layout Manager Application"
			LANGUAGES CSharp
		)

include (CSharpUtilities)
set (CMAKE_CSharp_FLAGS "/langversion:latest")
set (CMAKE_CSharp_FLAGS "/platform:anycpu")

set (RESOURCES_APP_ICON "${CMAKE_SOURCE_DIR}/res/TrayAppIcon.ico")

add_subdirectory(src)
add_subdirectory(test)
