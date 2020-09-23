CC				= clang++
CFLAGS			= -MMD -MP -Wall -std=c++11
ARCHITECTURE	= -m64

ROOTDIR			= ./../..
LIBDIR			= -L$(ROOTDIR)/Library/macOS/x64
LIBS			= -lRTCOP
INCLUDE			= -I$(ROOTDIR)/Include -I./src
TARGET			= ./bin/macOS_Clang/x64/ProceedSample.out
SRCDIR			= ./src
SOURCES			= $(shell find $(SRCDIR) $(EXCLUDEDIRS) -type f -name *.cpp -print)
OBJDIR			= ./obj/macOS_Clang/x64
OBJECTS			= $(subst $(SRCDIR),$(OBJDIR), $(SOURCES:.cpp=.o))
DEPENDS			= $(OBJECTS:.o=.d)
LCC				= mono ./$(ROOTDIR)/Tool/LayerCompiler.exe
LSOURCES		= $(shell find $(SRCDIR) -type f -name *.lcpp -print)
LTARGET			= ./src/Generated/

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

all: clean lcl $(TARGET)

lcl:
	$(LCC) $(LSOURCES) -r ./src/ -i ./src -t macos -e clang -o $(LTARGET)

run: $(TARGET)
	./$(TARGET)

clean:
	-rm -f $(OBJECTS) $(DEPENDS) $(TARGET)

-include $(DEPENDS)
