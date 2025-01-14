cmake_minimum_required (VERSION 3.30)

add_executable (
			u3WindowsManager
			${CMAKE_BINARY_DIR}/Version.cs
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
			VS_GLOBAL_Version "${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_REVISION}.${VERSION_BUILD}" FILE_CONTENTS "${FILE_CONTENTS}"
			VS_GLOBAL_Authors ${PRODUCT_AUTHORS}
			VS_GLOBAL_Copyright ${PRODUCT_COPYRIGHT}
			VS_GLOBAL_GenerateRuntimeConfigurationFiles "true"
	)
			#VS_GLOBAL_Version "${VERSION_MAJOR}.${VERSION_MINOR}.${VERSION_REVISION}.${VERSION_BUILD}" FILE_CONTENTS "${FILE_CONTENTS}"
			#VS_GLOBAL_PublishSingleFile true
			#VS_GLOBAL_SelfContained true

set_property(SOURCE ${RESOURCES_APP_ICON} PROPERTY VS_TOOL_OVERRIDE "EmbeddedResource")

set (EXE_NAME u3WindowsManager)
set_property (TARGET ${EXE_NAME} PROPERTY VS_DOTNET_REFERENCES
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

set(CMAKE_VS_INCLUDE_INSTALL_TO_DEFAULT_BUILD 0)

install(TARGETS ${EXE_NAME} RUNTIME DESTINATION .)
install(FILES 
			${CMAKE_BINARY_DIR}/bin/Release/${EXE_NAME}.dll
			${CMAKE_BINARY_DIR}/bin/Release/${EXE_NAME}.runtimeconfig.json
		DESTINATION .
	)

set (CPACK_SYSTEM_NAME ${PACKAGE_ARCH})
set (CPACK_PACKAGE_NAME ${PACKAGE_NAME})
set (CPACK_WIX_VERSION 4)
set (CPACK_WIX_ARCHITECTURE "x64")
set (CPACK_WIX_UPGRADE_GUID ${PACKAGE_UPGRADE_GUID})
set (CPACK_WIX_LICENSE_RTF "${CMAKE_SOURCE_DIR}/LICENSE.rtf")
set (CPACK_WIX_PRODUCT_ICON "${CMAKE_SOURCE_DIR}/res/TrayAppIcon.ico")
set (CPACK_WIX_PROGRAM_MENU_FOLDER ${PACKAGE_NAME})
set (CPACK_GENERATOR WIX)
include(CPack)
