CC				= arm-linux-gnueabihf-g++
CFLAGS			= -MMD -MP -Wall -std=c++11
ARCHITECTURE	= 

ROOTDIR			= ./../..
LIBDIR			= -L$(ROOTDIR)/Library/Linux/arm
LIBS			= -lRTCOP
INCLUDE			= -I$(ROOTDIR)/Include -I./src
TARGET			= ./bin/Linux_GCC/arm/ProceedSample.out
SRCDIR			= ./src
SOURCES			= $(shell find $(SRCDIR) -type f -name *.cpp -print)
OBJDIR			= ./obj/Linux_GCC/arm
OBJECTS			= $(subst $(SRCDIR),$(OBJDIR), $(SOURCES:.cpp=.o))
DEPENDS			= $(OBJECTS:.o=.d)
LCC				= mono $(ROOTDIR)/Tool/LayerCompiler.exe
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
	qemu-arm -L /usr/arm-linux-gnueabihf/ ./$(TARGET)

lcl:
	$(LCC) $(LSOURCES) -r ./src/ -i ./src -t linuxarm -e gcc -o $(LTARGET)

clean:
	-rm -f $(OBJECTS) $(DEPENDS) $(TARGET)

-include $(DEPENDS)
