CC				= clang++
CFLAGS			= -MMD -MP -Wall -std=c++11 -D MACOS_X64
ARCHITECTURE	= -m64

ROOTDIR			= ./../../..
LIBDIR			= -L$(ROOTDIR)/Test/TestSpace/macOS/Library/x64
LIBS			= -lRTCOP
INCLUDE			= -I$(ROOTDIR)/Include -I$(ROOTDIR)/Source/Library/RTCOP_DebugApplication
TARGET			= $(ROOTDIR)/Test/TestSpace/macOS/Library/x64/RTCOP_DebugApplication.out
SRCDIR			= $(ROOTDIR)/Source/Library/RTCOP_DebugApplication
EXCLUDEDIRS		= -path $(SRCDIR)/RTCOP_Generated/DependentCode/Windows -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Linux -prune -o
SOURCES			= $(shell find $(SRCDIR) $(EXCLUDEDIRS) -type f -name *.cpp -print)
OBJDIR			= $(ROOTDIR)/Object/macOS/Library/RTCOP_DebugApplication/x64
OBJECTS			= $(subst $(SRCDIR),$(OBJDIR), $(SOURCES:.cpp=.o))
DEPENDS			= $(OBJECTS:.o=.d)

$(TARGET): $(OBJECTS)
	@if [ ! -d $(shell dirname $@) ]; \
		then mkdir -p $(shell dirname $@); \
		fi
	$(CC) $(ARCHITECTURE) $(LIBDIR) -mdynamic-no-pic -o $@ $^ $(LIBS)

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
