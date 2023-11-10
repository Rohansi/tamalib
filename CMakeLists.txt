# CMakeList.txt : CMake project for tamalib, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

# Enable Hot Reload for MSVC compilers if supported.
if (POLICY CMP0141)
  cmake_policy(SET CMP0141 NEW)
  set(CMAKE_MSVC_DEBUG_INFORMATION_FORMAT "$<IF:$<AND:$<C_COMPILER_ID:MSVC>,$<CXX_COMPILER_ID:MSVC>>,$<$<CONFIG:Debug,RelWithDebInfo>:EditAndContinue>,$<$<CONFIG:Debug,RelWithDebInfo>:ProgramDatabase>>")
endif()

project ("tamalib")
add_library (tamalib SHARED
	"cpu.c"
	"cpu.h"
	"hal.h"
	"hal_types.h"
	"hw.c"
	"hw.h"
	"rom.h"
	"tamalib.c"
	"tamalib.h")
target_include_directories(tamalib PRIVATE "/")

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET tamalib PROPERTY C_STANDARD 11)
endif()

set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
