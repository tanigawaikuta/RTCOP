all:
	make -f RTCOP_x86.mk all
	make -f RTCOPDebugApplication_x86.mk all
	make -f RTCOP_x64.mk all
	make -f RTCOPDebugApplication_x64.mk all
	make -f RTCOP_arm.mk all
	make -f RTCOPDebugApplication_arm.mk all
	make -f RTCOP_arm64.mk all
	make -f RTCOPDebugApplication_arm64.mk all

x86:
	make -f RTCOP_x86.mk all
	make -f RTCOPDebugApplication_x86.mk all

x64:
	make -f RTCOP_x64.mk all
	make -f RTCOPDebugApplication_x64.mk all

arm:
	make -f RTCOP_arm.mk all
	make -f RTCOPDebugApplication_arm.mk all

arm64:
	make -f RTCOP_arm64.mk all
	make -f RTCOPDebugApplication_arm64.mk all

run_x86:
	make -f RTCOPDebugApplication_x86.mk run

run_x64:
	make -f RTCOPDebugApplication_x64.mk run

run_arm:
	make -f RTCOPDebugApplication_arm.mk run

run_arm64:
	make -f RTCOPDebugApplication_arm64.mk run

distribute_x86:
	make -f RTCOP_x86.mk distribute

distribute_x64:
	make -f RTCOP_x64.mk distribute

distribute_arm:
	make -f RTCOP_arm.mk distribute

distribute_arm64:
	make -f RTCOP_arm64.mk distribute

distribute:
	make -f RTCOP_x86.mk distribute
	make -f RTCOP_x64.mk distribute
	make -f RTCOP_arm.mk distribute
	make -f RTCOP_arm64.mk distribute

clean:
	make -f RTCOP_x86.mk clean
	make -f RTCOPDebugApplication_x86.mk clean
	make -f RTCOP_x64.mk clean
	make -f RTCOPDebugApplication_x64.mk clean
	make -f RTCOP_arm.mk clean
	make -f RTCOPDebugApplication_arm.mk clean
	make -f RTCOP_arm64.mk clean
	make -f RTCOPDebugApplication_arm64.mk clean
