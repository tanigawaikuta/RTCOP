cmake_minimum_required(VERSION 3.0.2)
project(rtcop)

find_package(catkin REQUIRED)

catkin_package(
  INCLUDE_DIRS Include
  LIBRARIES libRTCOP.a
)

#############
## Install ##
#############

install(FILES Library/Linux/x64/libRTCOP.a
   DESTINATION ${CATKIN_PACKAGE_LIB_DESTINATION}
)

install(FILES Tool/LayerCompiler.exe Tool/Sprache.dll
    DESTINATION ${CATKIN_PACKAGE_SHARE_DESTINATION}
)

install(DIRECTORY Include/
  DESTINATION ${CATKIN_PACKAGE_INCLUDE_DESTINATION}
  FILES_MATCHING PATTERN "*.h"
  PATTERN ".git" EXCLUDE
)

