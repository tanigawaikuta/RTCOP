all:
	make -f RTCOP_x86.mk all
	make -f RTCOPDebugApplication_x86.mk all
	make -f RTCOP_x64.mk all
	make -f RTCOPDebugApplication_x64.mk all

x86:
	make -f RTCOP_x86.mk all
	make -f RTCOPDebugApplication_x86.mk all

x64:
	make -f RTCOP_x64.mk all
	make -f RTCOPDebugApplication_x64.mk all

run_x86:
	make -f RTCOPDebugApplication_x86.mk run

run_x64:
	make -f RTCOPDebugApplication_x64.mk run

distribute_x86:
	make -f RTCOP_x86.mk distribute

distribute_x64:
	make -f RTCOP_x64.mk distribute

distribute:
	make -f RTCOP_x86.mk distribute
	make -f RTCOP_x64.mk distribute

clean:
	make -f RTCOP_x86.mk clean
	make -f RTCOPDebugApplication_x86.mk clean
	make -f RTCOP_x64.mk clean
	make -f RTCOPDebugApplication_x64.mk clean
