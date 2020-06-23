IF (USE_DOUBLE_PRECISION)
    ADD_DEFINITIONS( -DBT_USE_DOUBLE_PRECISION)
ENDIF (USE_DOUBLE_PRECISION)

IF(MSVC)
    IF (NOT USE_MSVC_RUNTIME_LIBRARY_DLL)
        #We statically link to reduce dependancies
        FOREACH(flag_var CMAKE_CXX_FLAGS CMAKE_CXX_FLAGS_DEBUG CMAKE_CXX_FLAGS_RELEASE CMAKE_CXX_FLAGS_MINSIZEREL CMAKE_CXX_FLAGS_RELWITHDEBINFO CMAKE_C_FLAGS CMAKE_C_FLAGS_DEBUG CMAKE_C_FLAGS_RELEASE CMAKE_C_FLAGS_MINSIZEREL CMAKE_C_FLAGS_RELWITHDEBINFO )
            IF(${flag_var} MATCHES "/MD")
                STRING(REGEX REPLACE "/MD" "/MT" ${flag_var} "${${flag_var}}")
            ENDIF(${flag_var} MATCHES "/MD")
            IF(${flag_var} MATCHES "/MDd")
                STRING(REGEX REPLACE "/MDd" "/MTd" ${flag_var} "${${flag_var}}")
            ENDIF(${flag_var} MATCHES "/MDd")
        ENDFOREACH(flag_var)
    ENDIF()

    IF (CMAKE_CL_64)
        ADD_DEFINITIONS(-D_WIN64)
    ELSE()
        IF (USE_MSVC_SSE)
            ADD_DEFINITIONS(/arch:SSE)
        ENDIF()
    ENDIF()
    IF (USE_MSVC_FAST_FLOATINGPOINT)
        ADD_DEFINITIONS(/fp:fast)
    ENDIF()
ENDIF(MSVC)

INCLUDE_DIRECTORIES("${libbulletc_SOURCE_DIR}/src")

ADD_EXECUTABLE(bulletctest bulletctest.c hacd_data.h)

TARGET_LINK_LIBRARIES(bulletctest ${BULLETC_LIB})
