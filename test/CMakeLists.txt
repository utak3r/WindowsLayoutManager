cmake_minimum_required (VERSION 3.30)

# project (
# 			u3WindowsManagerTest
# 			DESCRIPTION "Windows Layout Manager Tests"
# 			LANGUAGES CSharp
# 		)

enable_testing()

add_executable (
			u3WindowsManagerTest
			WndManagerTest.cs
		)

set_target_properties (
			u3WindowsManagerTest PROPERTIES
			DOTNET_SDK "Microsoft.NET.Sdk"
			DOTNET_TARGET_FRAMEWORK "net8.0-windows"
			VS_GLOBAL_TestProjectType "UnitTest"
			VS_GLOBAL_PROJECT_TYPES "{3AC096D0-A1C2-E12C-1390-A8335801FDAB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
			VS_DOTNET_REFERENCES "Microsoft.CSharp;System;System.Core"
			VS_PACKAGE_REFERENCES "Microsoft.NET.Test.Sdk_17.12.0;NUnit_4.2.2;NUnit3TestAdapter_4.6.0;NUnit.Analyzers_4.4.0;Moq_4.20.72;NUnit.ConsoleRunner_3.18.3;NUnit.Console_3.18.3"
			CMAKE_VS_NUGET_PACKAGE_RESTORE ON
			RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
			VS_GLOBAL_ImplicitUsings "enable"
		)

add_dependencies(
			u3WindowsManagerTest
			u3WindowsManager
		)

#target_link_libraries (
#			u3WindowsManagerTest u3WindowsManager
#			)

add_test(NAME WndManagerTest COMMAND WndManagerTest)
