CC				= aarch64-linux-gnu-g++
CFLAGS			= -MMD -MP -Wall -std=c++11 -D LINUX_ARM64
ARCHITECTURE	= 

ROOTDIR			= ./../../..
LIBDIR			= -L$(ROOTDIR)/Test/TestSpace/Linux/Library/arm64
LIBS			= -lRTCOP
INCLUDE			= -I$(ROOTDIR)/Include -I$(ROOTDIR)/Source/Library/RTCOP_DebugApplication
TARGET			= $(ROOTDIR)/Test/TestSpace/Linux/Library/arm64/RTCOP_DebugApplication.out
SRCDIR			= $(ROOTDIR)/Source/Library/RTCOP_DebugApplication
EXCLUDEDIRS		= -path $(SRCDIR)/RTCOP_Generated/DependentCode/Windows -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/macOS -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Linux/x64 -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Linux/x86 -prune -o \
					-path $(SRCDIR)/RTCOP_Generated/DependentCode/Linux/arm -prune -o
SOURCES			= $(shell find $(SRCDIR) $(EXCLUDEDIRS) -type f -name *.cpp -print)
OBJDIR			= $(ROOTDIR)/Object/Linux/Library/RTCOP_DebugApplication/arm64
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
	qemu-aarch64 -L /usr/aarch64-linux-gnu/ ./$(TARGET)

clean:
	-rm -f $(OBJECTS) $(DEPENDS) $(TARGET)

-include $(DEPENDS)
