CC				= x86_64-w64-mingw32-g++
CFLAGS			= -MMD -MP -Wall -std=c++11 -D WIN64_MINGW
ARCHITECTURE	= -m64

AR				= x86_64-w64-mingw32-gcc-ar
ARFLAG			= crsv

ROOTDIR			= ./../../..
LIBS			=
INCLUDE			= -I$(ROOTDIR)/Include -I$(ROOTDIR)/Source/Library/RTCOP
TARGET			= $(ROOTDIR)/Test/TestSpace/Windows/Library/x64_mingw/libRTCOP.a
DIST_TARGET		= $(ROOTDIR)/Library/Windows/x64_mingw/libRTCOP.a
SRCDIR			= $(ROOTDIR)/Source/Library/RTCOP
EXCLUDEDIRS		= -path $(SRCDIR)/DependentCode/Linux -prune -o \
					-path $(SRCDIR)/DependentCode/macOS -prune -o \
					-path $(SRCDIR)/DependentCode/Windows/x64 -prune -o \
					-path $(SRCDIR)/DependentCode/Windows/x86 -prune -o \
					-path $(SRCDIR)/DependentCode/Windows/x86_mingw -prune -o
SOURCES			= $(shell find $(SRCDIR) $(EXCLUDEDIRS) -type f -name *.cpp -print)
OBJDIR			= $(ROOTDIR)/Object/Windows/Library/RTCOP/x64_mingw
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
