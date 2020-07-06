CC				= i686-w64-mingw32-g++
CFLAGS			= -MMD -MP -Wall -std=c++11 -D WIN32_MINGW
ARCHITECTURE	= -m32

ROOTDIR			= ./../../..
LIBDIR			= -L$(ROOTDIR)/Test/TestSpace/Windows/Library/x86_mingw
LIBS			= -lRTCOP
INCLUDE			= -I$(ROOTDIR)/Include -I$(ROOTDIR)/Source/Library/RTCOP_DebugApplication
TARGET			= $(ROOTDIR)/Test/TestSpace/Windows/Library/x86_mingw/RTCOP_DebugApplication.exe
SRCDIR			= $(ROOTDIR)/Source/Library/RTCOP_DebugApplication
EXCLUDEDIRS		= -path $(SRCDIR)/RTCOP_Generated/DependentCode/Linux -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/macOS -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Windows/x64 -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Windows/x86 -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Windows/x64_mingw -prune -o
SOURCES			= $(shell find $(SRCDIR) $(EXCLUDEDIRS) -type f -name *.cpp -print)
OBJDIR			= $(ROOTDIR)/Object/Windows/Library/RTCOP_DebugApplication/x86_mingw
OBJECTS			= $(subst $(SRCDIR),$(OBJDIR), $(SOURCES:.cpp=.o))
DEPENDS			= $(OBJECTS:.o=.d)

$(TARGET): $(OBJECTS)
	@if [ ! -d $(shell dirname $@) ]; \
		then mkdir -p $(shell dirname $@); \
		fi
	$(CC) $(ARCHITECTURE) $(LIBDIR) -no-pie -o $@ $^ $(LIBS)

$(OBJDIR)/%.o: $(SRCDIR)/%.cpp
	@if [ ! -d $(shell dirname $@) ]; \
		then mkdir -p $(shell dirname $@); \
		fi
	$(CC) $(ARCHITECTURE) $(CFLAGS) $(INCLUDE) -o $@ -c $<

all: clean $(TARGET)

run: $(TARGET)
	./$(TARGET)

clean:
	-rm -f $(OBJECTS) $(DEPENDS) $(TARGET)

-include $(DEPENDS)
