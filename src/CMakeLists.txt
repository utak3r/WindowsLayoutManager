cmake_minimum_required (VERSION 3.30)

# Configure the Version.cs file
configure_file(
        Version.cs.in
        Version.cs
)

project (
			u3WindowsManager
			VERSION ${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_REVISION}.${VERSION_BUILD}
			DESCRIPTION "Windows Layout Manager"
			LANGUAGES CSharp
		)

set (RESOURCES_APP_ICON "${CMAKE_SOURCE_DIR}/res/TrayAppIcon.ico")

add_executable (
			u3WindowsManager
			${CMAKE_CURRENT_BINARY_DIR}/Version.cs
			Program.cs
			TrayApp.cs
			WndManager.cs
			WindowsSelectionList.cs
			${RESOURCES_APP_ICON}
		)

set_target_properties (
			u3WindowsManager PROPERTIES
			DOTNET_SDK "Microsoft.NET.Sdk"
			DOTNET_TARGET_FRAMEWORK "net8.0-windows"
			WIN32_EXECUTABLE TRUE
			RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
			VS_GLOBAL_UseWindowsForms "true"
			VS_GLOBAL_ImplicitUsings "enable"
			VS_GLOBAL_ApplicationIcon "${CMAKE_SOURCE_DIR}/res/TrayAppIcon.ico"
	)
			#VS_GLOBAL_Version "${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_REVISION}.${VERSION_BUILD}" FILE_CONTENTS "${FILE_CONTENTS}"
			#VS_GLOBAL_PublishSingleFile true
			#VS_GLOBAL_SelfContained true

set_property(SOURCE ${RESOURCES_APP_ICON} PROPERTY VS_TOOL_OVERRIDE "EmbeddedResource")

set_property (TARGET u3WindowsManager PROPERTY VS_DOTNET_REFERENCES
			"Microsoft.CSharp"
			"System"
			"System.Core"
			"System.Data"
			"System.Drawing"
			"System.Collections"
			"System.Windows.Forms"
			"System.Reflection"
			"System.IO"
			"System.Resources"
			"System.Diagnostics"
			"System.Text.Json"
	  )
