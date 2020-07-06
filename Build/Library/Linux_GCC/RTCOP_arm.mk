CC				= arm-linux-gnueabihf-g++
CFLAGS			= -MMD -MP -Wall -std=c++11 -D LINUX_ARM
ARCHITECTURE	= 

AR				= arm-linux-gnueabihf-ar
ARFLAG			= crsv

ROOTDIR			= ./../../..
LIBS			=
INCLUDE			= -I$(ROOTDIR)/Include -I$(ROOTDIR)/Source/Library/RTCOP
TARGET			= $(ROOTDIR)/Test/TestSpace/Linux/Library/arm/libRTCOP.a
DIST_TARGET		= $(ROOTDIR)/Library/Linux/arm/libRTCOP.a
SRCDIR			= $(ROOTDIR)/Source/Library/RTCOP
EXCLUDEDIRS		= -path $(SRCDIR)/DependentCode/Windows -prune -o \
					-path $(SRCDIR)/DependentCode/macOS -prune -o \
					-path $(SRCDIR)/DependentCode/Linux/x64 -prune -o \
					-path $(SRCDIR)/DependentCode/Linux/x86 -prune -o \
					-path $(SRCDIR)/DependentCode/Linux/arm64 -prune -o
SOURCES			= $(shell find $(SRCDIR) $(EXCLUDEDIRS) -type f -name *.cpp -print)
OBJDIR			= $(ROOTDIR)/Object/Linux/Library/RTCOP/arm
OBJECTS			= $(subst $(SRCDIR),$(OBJDIR), $(SOURCES:.cpp=.o))
DEPENDS			= $(OBJECTS:.o=.d)

$(TARGET): $(OBJECTS) $(LIBS)
	@if [ ! -d $(shell dirname $@) ]; \
		then mkdir -p $(shell dirname $@); \
		fi
	$(AR) $(ARFLAG) $@ $^

$(OBJDIR)/%.o: $(SRCDIR)/%.cpp
	@if [ ! -d $(shell dirname $@) ]; \
		then mkdir -p $(shell dirname $@); \
		fi
	$(CC) $(ARCHITECTURE) $(CFLAGS) $(INCLUDE) -o $@ -c $<

all: clean $(TARGET)

distribute: $(TARGET)
	@if [ ! -d $(shell dirname $(DIST_TARGET)) ]; \
		then mkdir -p $(shell dirname $(DIST_TARGET)); \
		fi
	cp -f $(TARGET) $(DIST_TARGET)

clean:
	-rm -f $(OBJECTS) $(DEPENDS) $(TARGET)

-include $(DEPENDS)
