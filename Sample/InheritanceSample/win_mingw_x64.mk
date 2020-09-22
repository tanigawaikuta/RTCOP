CC				= x86_64-w64-mingw32-g++
CFLAGS			= -MMD -MP -Wall -std=c++11
ARCHITECTURE	= -m64

ROOTDIR			= ./../..
LIBDIR			= -L$(ROOTDIR)/Library/Windows/x64_mingw
LIBS			= -lRTCOP
INCLUDE			= -I$(ROOTDIR)/Include -I./src
TARGET			= ./bin/Windows_MinGW/x64/InheritanceSample.exe
SRCDIR			= ./src
SOURCES			= $(shell find $(SRCDIR) -type f -name *.cpp -print)
OBJDIR			= ./obj/Windows_MinGW/x64
OBJECTS			= $(subst $(SRCDIR),$(OBJDIR), $(SOURCES:.cpp=.o))
DEPENDS			= $(OBJECTS:.o=.d)
LCC				= ./$(ROOTDIR)/Tool/LayerCompiler.exe
LSOURCES		= $(shell find $(SRCDIR) -type f -name *.lcpp -print)
LTARGET			= ./src/Generated/

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

all: clean lcl $(TARGET)

run: $(TARGET)
	./$(TARGET)

lcl:
	$(LCC) $(LSOURCES) -r ./src/ -i ./src -t win64 -e mingw -o $(LTARGET)

clean:
	-rm -f $(OBJECTS) $(DEPENDS) $(TARGET)

-include $(DEPENDS)
